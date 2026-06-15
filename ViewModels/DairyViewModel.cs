using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MyDairy.Common;
using MyDairy.Controls;
using MyDairy.Helpers;
using MyDairy.Models;
using MyDairy.Services;
using MyDairy.Settings;
using MyDairy.Views;
using WinRT;

namespace MyDairy.ViewModels;

public partial class DairyViewModel : ObservableObject
{
    public DairyViewModel()
    {
        AppSettings.Instance.PropertyChanged += Instance_PropertyChanged;

        Manager = new();
        Manager.FileSaved += Manager_FileSaved;
        Manager.FileOpened += Manager_FileOpened;
        Manager.DairyDayRemoved += Manager_DairyDayRemoved;
        Manager.DairyTextRemoved += Manager_DairyTextRemoved;

        Manager.FileUpdated += Manager_FileUpdated;
        Manager.CanExecuteChanged += Manager_CanExecuteChanged;
    }

    internal void NoticeFileEdited()
    {
        if (CurrentFile != null)
        {
            IsSaved = false;
        }
    }

    private void Manager_CanExecuteChanged(object sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }
    private void Manager_FileUpdated(object sender, EventArgs e)
    {
        NoticeFileEdited();
        OnPropertyChanged(nameof(CurrentPassword));
    }
    private void Manager_FileSaved(object sender, EventArgs e)
    {
        IsSaved = true;
    }
    private void Manager_FileOpened(object sender, EventArgs e)
    {
        // release old resources
        CurrentText = null;
        OpenedTexts.Clear();

        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnPropertyChanged(nameof(CurrentFile));
        OnPropertyChanged(nameof(CurrentPassword));
        OnPropertyChanged(nameof(HasCurrentFile));

        IsSaved = Manager.HasRelativeFile;
    }
    private void Manager_DairyDayRemoved(object sender, DairyDay e)
    {
        foreach (var text in e.Texts)
        {
            CloseText(text);
        }
    }
    private void Manager_DairyTextRemoved(object sender, DairyText e)
    {
        CloseText(e);
        CloseWindowText(e);
        CloseResult(e);
    }

    private void UpdateShowDairyCount()
    {
        if (CurrentFile == null)
        {
            return;
        }

        foreach (var day in CurrentFile.DairyDays)
        {
            day.UpdateCountString();
        }
    }

    private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppSettings.ShowDairyCount))
        {
            UpdateShowDairyCount();
        }
    }

    public void CloseText(DairyText text)
    {
        OpenedTexts.Remove(text);

        if (OpenedTexts.Count == 0)
        {
            CurrentText = null;
        }
    }
    public void CloseWindowText(DairyText text)
    {
        if (OpenedInWindow.TryGetValue(text, out var window))
        {
            window.Close();
            OpenedInWindow.Remove(text);
        }
    }
    public void CloseResult(DairyText text)
    {
        ResultDate.Remove(text);
        ResultTitle.Remove(text);
    }

    public void OpenText(DairyText text)
    {
        if (text != null && !OpenedTexts.Contains(text))
        {
            OpenedTexts.Add(text);
        }

        CurrentText = text;
    }
    public void OpenTextInNewWindow(DairyText text)
    {
        CloseText(text);

        if (OpenedInWindow.TryGetValue(text, out var window))
        {
            window.Activate();
            return;
        }

        ContentWindow newWindow = new();
        DairyTextControl control = new()
        {
            DairyText = text,
        };

        var currentSize = DairyPage.Instance.GetCurrentDataPresenterSize();
        currentSize.Width += control.GetExtraWidth();
        currentSize.Height += control.GetExtraHeight();

        newWindow.AppWindow.Resize(currentSize.ToSizeInt32());
        newWindow.SetContent(control);

        App.Window.OpenContentWindow(newWindow);

        newWindow.Closed += OnContentWindowClosed;
    }

    public async ValueTask<bool> TrySearchAsync(string requestedText)
    {
        if (string.IsNullOrEmpty(requestedText))
        {
            return false;
        }

        await ValueTask.CompletedTask;

        List<DairyText> resultDate = [];
        List<DairyText> resultTitle = [];

        var hasResult = false;
        foreach (var day in CurrentFile.DairyDays)
        {
            var date = XamlHelper.DateOnlyToString(day.Date);

            foreach (var text in day.Texts)
            {
                if (date.Contains(requestedText))
                {
                    resultDate.Add(text);
                    hasResult = true;
                }

                if (text.Title.Contains(requestedText))
                {
                    resultTitle.Add(text);
                    hasResult = true;
                }
            }
        }

        CollectionHelper.UpdateCollectionNoClear(ResultDate, resultDate);
        CollectionHelper.UpdateCollectionNoClear(ResultTitle, resultTitle);

        return hasResult;
    }
   
    private void OnContentWindowClosed(object sender, WindowEventArgs args)
    {
        var window = sender.As<ContentWindow>();
        CloseWindowText(window.GetContent().As<DairyTextControl>().DairyText);
    }
}
