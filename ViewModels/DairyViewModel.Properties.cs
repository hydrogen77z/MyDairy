using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.UI.Xaml;
using MyDairy.Models;
using MyDairy.Services;

namespace MyDairy.ViewModels;

public partial class DairyViewModel
{
    public DairyManager Manager
    {
        get;
    }

    public bool CanUndo => Manager.CanUndo;
    public bool CanRedo => Manager.CanRedo;

    public DairyFile CurrentFile => Manager.CurrentFile;
    public string CurrentPassword => Manager.CurrentPassword;

    private DairyText _currentText = null;
    public DairyText CurrentText
    {
        get => _currentText;
        set
        {
            if (_currentText != value)
            {
                if (value != null && !OpenedTexts.Contains(value))
                {
                    OpenedTexts.Add(value);
                }

                _currentText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCurrentText));
            }
        }
    }

    public bool HasCurrentFile => CurrentFile != null;
    public bool HasCurrentText => _currentText != null;

    private bool _isSaved = true;
    public bool IsSaved
    {
        get => _isSaved;
        set => SetProperty(ref _isSaved, value);
    }

    public readonly ObservableCollection<DairyText> OpenedTexts = [];
}
