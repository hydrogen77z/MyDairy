using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Models;
using MyDairy.Services;
using MyDairy.Settings;

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

    private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
}
