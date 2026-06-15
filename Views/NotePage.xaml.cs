using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Models;
using MyDairy.Services;
using MyDairy.Settings;
using MyDairy.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NotePage : Page, ISupportContainedInWindow
{
    private Note _currentNote = null;
    private bool _inWindow = false;

    public static NotePage Instance
    {
        get;
        private set;
    } = null;

    public NotePage()
    {
        InitializeComponent();

        Instance = this;
        UpdateButton();
    }

    private static void RemoveNote(Note note)
    {
        AppSettings.Instance.Notes.Remove(note);
    }

    private void UpdateButton()
    {
        var hasValue = _currentNote != null;
        CommandEdit.Visibility = XamlHelper.ToVisible(hasValue);
        CommandDelete.Visibility = XamlHelper.ToVisible(hasValue);
    }

    private void OnDeleteNoteContext(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is Note note)
        {
            RemoveNote(note);
        }
    }

    private async void OnEditNoteContext(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is Note note)
        {
            await EditNote(note);
        }
    }

    private async void OnNewNote(object sender, RoutedEventArgs e)
    {
        NewNoteBox.Text = string.Empty;
        if (ContentDialogResult.Primary == await ContentDialogService.ShowWithInfoAsync(NewNoteDialog, XamlRoot))
        {
            QuickNoteHelper.AddNote(NewNoteBox.Text);
        }
    }

    private async void OnEditNote(object sender, RoutedEventArgs e)
    {
        await EditNote(_currentNote);
    }

    private void OnDeleteNote(object sender, RoutedEventArgs e)
    {
        RemoveNote(_currentNote);
    }

    private async ValueTask EditNote(Note note)
    {
        EditNoteBox.Text = note.Content;
        EditNoteBox.Focus(FocusState.Keyboard);
        EditNoteBox.Select(note.Content.Length, 0);

        if (ContentDialogResult.Primary == await ContentDialogService.ShowWithInfoAsync(EditNoteDialog, XamlRoot))
        {
            QuickNoteHelper.EditNote(note, EditNoteBox.Text);
        }
    }

    private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _currentNote = NoteList.SelectedItem as Note;
        UpdateButton();
    }

    public string Title => "Header/Header".GetLocalized("Note");

    public event EventHandler TitleChanged;

    public bool GetIsBackButtonVisible() => _inWindow;
    public void OnBackRequested() => App.Window.OpenNotePage(false);
    public void SetContainToWindow(bool inWindow)
    {
        _inWindow = inWindow;
        CommandNewWindow.Visibility = XamlHelper.ToCollapsed(_inWindow);
        CommandReturn.Visibility = XamlHelper.ToVisible(_inWindow);
    }

    private void OnOpenInNewWindow(object sender, RoutedEventArgs e)
    {
        App.Window.OpenNotePage(true);
    }
    private void OnReturnMainPage(object sender, RoutedEventArgs e)
    {
        App.Window.OpenNotePage(false);
    }
}
