using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.UI.Xaml;
using MyDairy.Models;
using MyDairy.Services;
using MyDairy.Views;

namespace MyDairy.ViewModels;

public partial class DairyViewModel
{
    //internal DataTemplate _dairyTextPresentTemplate = null;

    public DairyManager Manager
    {
        get;
    }

    public bool CanUndo => Manager.CanUndo;
    public bool CanRedo => Manager.CanRedo;

    public DairyFile CurrentFile => Manager.CurrentFile;
    public string CurrentPassword => Manager.CurrentPassword;

    public bool HasCurrentFile => CurrentFile != null;
    public bool HasCurrentText => _currentText != null;

    private DairyText _currentText = null;
    public DairyText CurrentText
    {
        get => _currentText;
        set => SetProperty(ref _currentText, value, nameof(CurrentText), nameof(HasCurrentText));
    }

    private bool _isSaved = true;
    public bool IsSaved
    {
        get => _isSaved;
        set => SetProperty(ref _isSaved, value);
    }

    public readonly ObservableCollection<DairyText> OpenedTexts = [];

    public readonly Dictionary<DairyText, ContentWindow> OpenedInWindow = [];

    public readonly ObservableCollection<DairyText> ResultDate = [];
    public readonly ObservableCollection<DairyText> ResultTitle = [];
}
