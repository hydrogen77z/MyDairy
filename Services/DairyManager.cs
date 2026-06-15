using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using MyDairy.Commands;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Models;
using MyDairy.Serialization;
using MyDairy.Settings;
using Windows.Foundation;
using Windows.Media.AppBroadcasting;

namespace MyDairy.Services;

public partial class DairyManager
{
    private DairyFile _currentFile = null;
    private string _currentFilePath = null;
    private string _currentPassword = null;
    private JsonDocument _jsonDocument = null;
    private EncryptedDairyFile _encryptedFile = null;

    private readonly Stack<ActionCommand> _undoCommands = new();
    private readonly Stack<ActionCommand> _redoCommands = new();

    public string CurrentPassword => _currentPassword;

    public bool CanUndo => _undoCommands.Count > 0;
    public bool CanRedo => _redoCommands.Count > 0;

    public DairyFile CurrentFile => _currentFile;
    public bool HasRelativeFile => _currentFilePath != null && Path.Exists(_currentFilePath);
    public string RelativeFilePath => _currentFilePath;

    public bool IsEncrypted
    {
        get; private set;
    }

    public async ValueTask CreateAndSaveAsync(string filePath)
    {
        _currentFilePath = filePath;
        await SaveAsFileAsync(_currentFilePath);
    }
    public async ValueTask SaveAsync()
    {
        if (HasRelativeFile)
        {
            await SaveAsFileAsync(_currentFilePath);
        }
    }
    public async ValueTask SaveAsFileAsync(string filePath)
    {
        if (CurrentFile is not null)
        {
            foreach (var day in CurrentFile.DairyDays)
            {
                foreach (var text in day.Texts)
                {
                    text.OnFileSaved();
                }
            }
            CurrentFile.LastEditTime = DateTime.Now;

            string serializedText;
            var dairyFileJsonText = JsonSerializer.Serialize(_currentFile, DairySerializerContext.Default.DairyFile);
            if (IsEncrypted && !string.IsNullOrEmpty(_currentPassword))
            {
                _encryptedFile ??= new()
                {
                    Salt = RandomNumberGenerator.GetBytes(EncryptHelper.SaltSize),
                    Nonce = RandomNumberGenerator.GetBytes(EncryptHelper.NonceSize),
                    Tag = new byte[EncryptHelper.TagSize],
                };

                _encryptedFile.EncryptedContent = EncryptHelper.EncryptToBase64(dairyFileJsonText, _currentPassword, _encryptedFile.Salt, _encryptedFile.Nonce, _encryptedFile.Tag);

                serializedText = JsonSerializer.Serialize(_encryptedFile, SourceGenerationContext.Default.EncryptedDairyFile);
            }
            else
            {
                serializedText = dairyFileJsonText;
            }
            await File.WriteAllTextAsync(filePath, serializedText);

            FileSaved?.Invoke(this, EventArgs.Empty);
        }
    }
    public async ValueTask ExportToFileAsync(string filePath)
    {
        var dairyFileJsonText = JsonSerializer.Serialize(_currentFile, DairySerializerContext.Default.DairyFile);
        await File.WriteAllTextAsync(filePath, dairyFileJsonText);
    }
    public async ValueTask LoadFileAsync(string filePath)
    {
        ReleaseCurrentFile();

        var fileContent = await File.ReadAllTextAsync(filePath);
        _jsonDocument = JsonDocument.Parse(fileContent);

        IsEncrypted = _jsonDocument.RootElement.TryGetProperty(GlobalConstants.EncryptedContentKey, out var element) && element.ValueKind == JsonValueKind.String;
        _currentFilePath = filePath;
    }
    public bool TryParseFile(string password)
    {
        if (_jsonDocument == null)
        {
            return true;
        }

        if (IsEncrypted == false)
        {
            _currentFile = JsonSerializer.Deserialize(_jsonDocument, DairySerializerContext.Default.DairyFile);
        }
        else
        {
            _encryptedFile = JsonSerializer.Deserialize(_jsonDocument, SourceGenerationContext.Default.EncryptedDairyFile);

            try
            {
                var dairyFileJsonText = EncryptHelper.DecryptFromBase64(_encryptedFile.EncryptedContent, password, _encryptedFile.Salt, _encryptedFile.Nonce, _encryptedFile.Tag);
                _currentFile = JsonSerializer.Deserialize(dairyFileJsonText, DairySerializerContext.Default.DairyFile);
            }
            catch (CryptographicException)
            {
                return false;
            }

            _currentPassword = password;
        }

        _jsonDocument = null;
        FileOpened?.Invoke(this, EventArgs.Empty);
        return true;
    }
    public void AttachToNewContent(DairyFile file)
    {
        ReleaseCurrentFile();
        _currentFile = file;

        FileOpened?.Invoke(this, EventArgs.Empty);
    }
    public void ChangePassword(string newPassword)
    {
        if (_currentPassword != newPassword)
        {
            if (!string.IsNullOrEmpty(newPassword) && _encryptedFile != null)
            {
                _encryptedFile.Salt = RandomNumberGenerator.GetBytes(EncryptHelper.SaltSize);
                _encryptedFile.Nonce = RandomNumberGenerator.GetBytes(EncryptHelper.NonceSize);
            }
            _currentPassword = newPassword;
            IsEncrypted = !string.IsNullOrEmpty(_currentPassword);

            UpdateFile();
        }
    }

    public void ReleaseCurrentFile()
    {
        _currentFile = null;
        _currentFilePath = null;
        _currentPassword = null;
        _jsonDocument = null;
        _encryptedFile = null;

        _undoCommands.Clear();
        _redoCommands.Clear();
    }

    public DairyDay GetDayFromDateOnly(DateOnly dateOnly)
    {
        foreach (var day in _currentFile.DairyDays)
        {
            if (day.Date == dateOnly)
            {
                return day;
            }
        }
        return null;
    }
    public DairyDay CreateNewDay(DateOnly dateOnly)
    {
        var index = 0;
        for (; index < _currentFile.DairyDays.Count; index++)
        {
            var day = _currentFile.DairyDays[index];
            if (day.Date == dateOnly)
            {
                return day;
            }
            if (day.Date > dateOnly)
            {
                break;
            }
        }

        var newDay = new DairyDay() { Date = dateOnly };

        ExecuteCommand(new(() => _currentFile.DairyDays.Insert(index, newDay), () =>
        {
            _currentFile.DairyDays.Remove(newDay);
            DairyDayRemoved?.Invoke(this, newDay);
        }));
        
        UpdateFile();
        return newDay;
    }
    public void CreateNewText(DairyDay day, string title, string description, bool isTimeApproximate, TimeOnly? time)
    {
        var newText = new DairyText(title, description, string.Empty, isTimeApproximate, time, DateTime.Now);

        //day.Texts.Add(newText);
        ExecuteCommand(new(() => day.Texts.Add(newText), () =>
        {
            day.Texts.Remove(newText);
            DairyTextRemoved?.Invoke(this, newText);
        }));
        
        UpdateFile();
    }

    public void DeleteDay(DairyDay day)
    {
        //_currentFile.DairyDays.Remove(day);
        ExecuteCommand(new(() =>
        {
            _currentFile.DairyDays.Remove(day);
            DairyDayRemoved?.Invoke(this, day);
        }, () => _currentFile.DairyDays.Add(day)));

        UpdateFile();
    }
    public void DeleteText(DairyText text)
    {
        var date = text.SourceDate;
        foreach (var day in _currentFile.DairyDays)
        {
            if (day.Date == date)
            {
                ExecuteCommand(new(() =>
                { 
                    day.Texts.Remove(text);
                    DairyTextRemoved?.Invoke(this, text);
                }, () => day.Texts.Add(text)));

                //DairyTextRemoved?.Invoke(this, text);
                UpdateFile();
            }
        }
    }

    public void EditFileInfo(string name, string author, string description)
    {
        CurrentFile.Name = name;
        CurrentFile.Author = author;
        CurrentFile.Description = description;
        UpdateFile();
    }

    private void UpdateFile()
    {
        FileUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteCommand(ActionCommand command)
    {
        command.Execute();
        _undoCommands.Push(command);

        // Can Undo Changed
        OnCanExecuteChanged();
    }
    private void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    public void Undo()
    {
        if (_undoCommands.Count == 0)
        {
            return;
        }
        var command = _undoCommands.Pop();
        command.Undo();
        UpdateFile();

        _redoCommands.Push(command);
        OnCanExecuteChanged();
    }
    public void Redo()
    {
        if (_redoCommands.Count == 0)
        {
            return;
        }
        var command = _redoCommands.Pop();
        command.Execute();
        UpdateFile();

        _undoCommands.Push(command);
        OnCanExecuteChanged();
    }

    #region Events
    public event EventHandler FileSaved;
    public event EventHandler FileOpened;
    public event EventHandler FileUpdated;

    public event EventHandler<DairyDay> DairyDayRemoved;
    public event EventHandler<DairyText> DairyTextRemoved;

    public event EventHandler CanExecuteChanged;
    #endregion
}
