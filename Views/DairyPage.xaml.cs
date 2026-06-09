using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.AccessControl;
using System.Text.Json;
using System.Threading.Tasks;
using ABI.Windows.Foundation.Collections;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using MyDairy.Attached;
using MyDairy.Common;
using MyDairy.Controls;
using MyDairy.Helpers;
using MyDairy.Models;
using MyDairy.Services;
using MyDairy.Settings;
using MyDairy.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.AppBroadcasting;
using Windows.UI.WebUI;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DairyPage : Page
{
    public Brush MenuBarBrush
    {
        get;
    }
    public Brush ContentBrush
    {
        get;
    }

    private static DairyPage _instance = null;
    public static DairyPage Instance => _instance;

    //private DairyFile _file = null;
    //private DairyText _text = null;
    //private DairyDay _currentTextForDay = null;
    //private readonly ObservableCollection<DairyText> _tabTexts = [];
    public readonly DairyViewModel ViewModel = new();

    private FileOpenPicker _openPicker = null;
    private FileSavePicker _savePicker = null;
    private readonly RadioMenuService<DairyView> _dairyPane = null;
    private readonly RadioMenuService<DairyPaneDisplayMode> _paneDisplayMode = null;
    private readonly RadioMenuService<QuickNoteDisplayMode> _quickNoteDisplayMode = null;

    public DairyPage()
    {
        InitializeComponent();

        _instance = this;
        MenuBarBrush = (Brush)Resources["MenuBarBackgroundBrush"];
        ContentBrush = (Brush)Resources["ContentBackgroundBrush"];

        MenuBarBrush.Opacity = AppSettings.Instance.MenuBarOpacity;
        ContentBrush.Opacity = AppSettings.Instance.ContentOpacity;

        AppSettings.Instance.PropertyChanged += OnSettingsPropertyChanged;

        _dairyPane = new([MenuViewPane0, MenuViewPane1, MenuViewPane2], AppSettings.Instance.DefaultDairyView, "MenuViewPane");
        _dairyPane.RadioChanged += MenuViewPane_RadioChanged;

        _paneDisplayMode = new([MenuViewPaneDisplayMode0, MenuViewPaneDisplayMode1], AppSettings.Instance.DairyPaneDisplayMode, "MenuViewPaneDisplayMode");
        _paneDisplayMode.RadioChanged += PaneDisplayModeView_RadioChanged;

        _quickNoteDisplayMode = new([MenuViewQuickNote0, MenuViewQuickNote1, MenuViewQuickNote2], AppSettings.Instance.QuickNoteDisplayMode, "MenuViewQuickNote");
        _quickNoteDisplayMode.RadioChanged += QuickNoteDisplayMode_RadioChanged;

        MainTabView.TabItemsSource = ViewModel.OpenedTexts;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        App.Window.PaneToggleRequested += OnTitleBarPaneToggleRequested;
        App.Window.AppWindow.Closing += AppWindow_Closing;

        SelectTabView();
        SelectSplitViewDisplayMode();
        SelectQuickNoteDisplayMode();
        //SelectPresentView();
    }

    #region Menu Event
    private void MenuViewPane_RadioChanged(RadioMenuService<DairyView> sender, DairyView args)
    {
        AppSettings.Instance.DefaultDairyView = args;
    }
    private void PaneDisplayModeView_RadioChanged(RadioMenuService<DairyPaneDisplayMode> sender, DairyPaneDisplayMode args)
    {
        AppSettings.Instance.DairyPaneDisplayMode = args;
    }
    private void QuickNoteDisplayMode_RadioChanged(RadioMenuService<QuickNoteDisplayMode> sender, QuickNoteDisplayMode args)
    {
        AppSettings.Instance.QuickNoteDisplayMode = args;
    }
    private void OnSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(AppSettings.MenuBarOpacity):
                MenuBarBrush.Opacity = AppSettings.Instance.MenuBarOpacity;
                break;
            case nameof(AppSettings.ContentOpacity):
                ContentBrush.Opacity = AppSettings.Instance.ContentOpacity;
                break;
            case nameof(AppSettings.DefaultDairyView):
                SelectPresentView();
                break;
            case nameof(AppSettings.ShowTab):
                SelectTabView();
                break;
            case nameof(AppSettings.DairyPaneDisplayMode):
                SelectSplitViewDisplayMode();
                break;
            case nameof(AppSettings.QuickNoteDisplayMode):
                SelectQuickNoteDisplayMode();
                break;
        }
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        App.Window.OpenPage(typeof(SettingsPage));
    }
    private void OnDebugClick(object sender, RoutedEventArgs e)
    {
        App.Window.OpenPage(typeof(DebugPage));
    }

    private async void OnMenuFileOpenClick(object sender, RoutedEventArgs e)
    {
        if (_openPicker is null)
        {
            _openPicker = new(ContentGrid.XamlRoot.ContentIslandEnvironment.AppWindowId);
            _openPicker.FileTypeChoices.Add("MyDairyFile".GetLocalized(), [".myd"]);
            _openPicker.FileTypeChoices.Add("JsonFile".GetLocalized(), [".json"]);
            _openPicker.FileTypeChoices.Add("AllFiles".GetLocalized(), ["*"]);
        }

        if (await _openPicker.PickSingleFileAsync() is PickFileResult result)
        {
            await OpenDairyFile(result.Path);
        }
    }
    private async void OnMenuFileNewClick(object sender, RoutedEventArgs e)
    {
        NewDairyNameBox.Text = null;
        NewDairyAuthorBox.Text = null;
        NewDairyDescriptionBox.Text = null;
        NewSetPasswordBox.IsChecked = false;

        NewPasswordControl.ClearPassword();

        if (await NewDairyDialog.ShowAsync() == ContentDialogResult.Primary)
        {
            var file = new DairyFile()
            {
                Name = NewDairyNameBox.Text,
                Author = NewDairyAuthorBox.Text,
                Description = NewDairyDescriptionBox.Text,
                CreateTime = DateTime.Now,
                LastEditTime = DateTime.Now,
            };

            if (await TrySaveAndCloseCurrentFile())
            {
                ViewModel.Manager.AttachToNewContent(file);

                if (NewSetPasswordBox.IsChecked == true && NewPasswordControl.TryGetVerifyPassword(out var newPassword))
                {
                    ViewModel.Manager.ChangePassword(newPassword);
                }
            }
        }
    }
    private async void OnMenuFileSaveClick(object sender, RoutedEventArgs e)
    {
        await SaveCurrentFile(false);
    }
    private async void OnMenuFileSaveAsClick(object sender, RoutedEventArgs e)
    {
        if (await RequestSaveAsFilePath() is string filePath)
        {
            await ViewModel.Manager.SaveAsFileAsync(filePath);
        }
    }
    private void OnMenuFileInfoClick(object sender, RoutedEventArgs e)
    {
        LoadFileInfo();
    }

    // Content Dialog Initialize
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DialogExtension.SetKind(JsonDialog, ContentDialogKind.Close);
        DialogExtension.SetKind(ErrorDialog, ContentDialogKind.Close);
        DialogExtension.SetKind(NewDairyDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(IsSaveToFileDialog, ContentDialogKind.SaveUnsaveCancel);
        DialogExtension.SetKind(CreateNewDayDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(CreateNewTextDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(DeleteConfirmDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(EditFileInfoDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(RequestPasswordDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(SetPasswordDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(ChangePasswordDialog, ContentDialogKind.OkClose);
        DialogExtension.SetKind(RemovePasswordDialog, ContentDialogKind.OkClose);
    }
    #endregion

    #region UI Shell
    private void OnTitleBarPaneToggleRequested(object sender, EventArgs args)
    {
        MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
    }
    private void OnViewAllClicked(object sender, RoutedEventArgs e)
    {
        App.Window.OpenPage(typeof(NotePage));
    }
    private void OnCreateNewNote(FeedbackButton sender, FeedbackButtonClickEventArgs e)
    {
        var note = QuickNoteBox.Text;

        if (string.IsNullOrEmpty(note))
        {
            e.BeginStoryboard = false;

            return;
        }
        QuickNoteHelper.AddNote(note);
        //AppSettings.Instance.Notes.Add(new(note, DateTime.Now));
    }
    private void SelectTabView()
    {
        MainTabView.Visibility = XamlHelper.ToVisible(AppSettings.Instance.ShowTab);
    }
    private void SelectPresentView()
    {
        _dairyPane.Value = AppSettings.Instance.DefaultDairyView;
        switch (_dairyPane.Value)
        {
            case DairyView.TreeView:
                ContentViewPresenter.Content = ContentViewTreeView;
                break;
            case DairyView.DoubleListView:
                ContentViewPresenter.Content = ContentViewDoubleListView;
                ContentViewDoubleListView.DisplayMode = DoubleListViewDisplayMode.Split;
                break;
            default:
                ContentViewPresenter.Content = ContentViewDoubleListView;
                ContentViewDoubleListView.DisplayMode = DoubleListViewDisplayMode.Compact;
                break;
        }
    }
    private void SelectSplitViewDisplayMode()
    {
        _paneDisplayMode.Value = AppSettings.Instance.DairyPaneDisplayMode;
        switch (_paneDisplayMode.Value)
        {
            case DairyPaneDisplayMode.Overlay:
                VisualStateManager.GoToState(MainUserControl, "PaneModeOverlay", false);
                //MainSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                break;
            default:
                VisualStateManager.GoToState(MainUserControl, "PaneModeInline", false);
                //MainSplitView.DisplayMode = SplitViewDisplayMode.Inline;
                break;
        }
    }
    private void SelectQuickNoteDisplayMode()
    {
        _quickNoteDisplayMode.Value = AppSettings.Instance.QuickNoteDisplayMode;
        switch (_quickNoteDisplayMode.Value)
        {
            case QuickNoteDisplayMode.Bottom:
                VisualStateManager.GoToState(MainUserControl, "QuickNoteBottom", false);
                break;
            case QuickNoteDisplayMode.Hidden:
                VisualStateManager.GoToState(MainUserControl, "QuickNoteHidden", false);
                break;
            default:
                VisualStateManager.GoToState(MainUserControl, "QuickNoteLeftPane", false);
                break;
        }
    }

    private static string GetMessage(Exception exception)
    {
        return exception.GetBaseException().ToString() + ':' + exception.Message;
    }
    public async ValueTask ShowJsonException(JsonException jsonException)
    {
        JsonDialogMessage.Text = GetMessage(jsonException);
        JsonDialogStackTrace.Text = jsonException.StackTrace;

        JsonDialogExpander.IsExpanded = false;
        await JsonDialog.ShowAsync();
    }
    public async ValueTask ShowException(Exception exception)
    {
        ErrorDialogMessage.Text = GetMessage(exception);
        ErrorDialogStackTrace.Text = exception.StackTrace;

        ErrorDialogExpander.IsExpanded = false;
        await ErrorDialog.ShowAsync();
    }
    private async ValueTask<ContentDialogResult> ShowIsSaveDialog()
    {
        string fileText;
        if (ViewModel.Manager.HasRelativeFile)
        {
            fileText = "IsSaveToFilePath".GetLocalized() + ' ' + ViewModel.Manager.RelativeFilePath;
        }
        else
        {
            fileText = "IsSaveToNewFilePath".GetLocalized();
        }

        IsSaveToFileDialog.Content = fileText;

        return await IsSaveToFileDialog.ShowAsync();
    }
    private async ValueTask ShowEditFileInfoDialog()
    {
        var file = ViewModel.CurrentFile;
        EditDairyNameBox.Text = file.Name;
        EditDairyAuthorBox.Text = file.Author;
        EditDairyDescriptionBox.Text = file.Description;

        if (ContentDialogResult.Primary == await EditFileInfoDialog.ShowAsync())
        {
            ViewModel.Manager.EditFileInfo(EditDairyNameBox.Text, EditDairyAuthorBox.Text, EditDairyDescriptionBox.Text);

            UpdateFileInfoToTitleBar();
            LoadFileInfo();
        }
    }

    private async ValueTask ShowRequestPasswordDialog()
    {
        RequestPasswordControl.ClearPassword();

        await RequestPasswordDialog.ShowAsync();
        //if (ContentDialogResult.None == await RequestPasswordDialog.ShowAsync())
        //{
        //    ViewModel.Manager.ReleaseCurrentFile();
        //}
    }
    private async ValueTask ShowSetPasswordDialog()
    {
        SetPasswordControl.ClearPassword();

        if (ContentDialogResult.Primary == await SetPasswordDialog.ShowAsync())
        {
            if (SetPasswordControl.TryGetVerifyPassword(out var verifyPassword))
            {
                ViewModel.Manager.ChangePassword(verifyPassword);
            }
        }
    }
    private async ValueTask ShowChangePasswordDialog()
    {
        ChangePasswordControl.ClearPassword();

        if (ContentDialogResult.Primary == await ChangePasswordDialog.ShowAsync())
        {
            if (ChangePasswordControl.TryGetVerifyPassword(out var verifyPassword))
            {
                ViewModel.Manager.ChangePassword(verifyPassword);
            }
        }
    }
    private async ValueTask ShowRemovePasswordDialog()
    {
        RemovePasswordControl.ClearPassword();

        if (ContentDialogResult.Primary == await RemovePasswordDialog.ShowAsync())
        {
            ViewModel.Manager.ChangePassword(null);
        }
    }

    private async void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        args.Cancel = true;

        if (DialogExtension.TryGetOpenedContentDialog() is ContentDialog dialog)
        {
            dialog.Hide();
        }

        if (AppSettings.Instance.NotShowCloseAgain)
        {
            App.Window.Close();
            return;
        }

        if (await TrySaveAndCloseCurrentFile())
        {
            App.Window.Close();
        }
    }
    #endregion

    #region Dairy File Core
    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DairyViewModel.CurrentFile):
                OnCurrentFileChanged();
                break;
            case nameof(DairyViewModel.IsSaved):
                OnIsSavedChanged();
                break;
            case nameof(DairyViewModel.CurrentPassword):
                LoadFilePasswordInfo();
                break;
        }
    }
    private void OnCurrentFileChanged()
    {
        PresentDairyFile();
    }
    private void OnIsSavedChanged()
    {
        UpdateFileInfoToTitleBar();
    }

    private void PresentDairyFile()
    {
        ContentViewTreeView.ItemsSource = ViewModel.CurrentFile.DairyDays;
        ContentViewDoubleListView.ItemsSource = ViewModel.CurrentFile.DairyDays;
        NewTextDateBox.ItemsSource = ViewModel.CurrentFile.DairyDays;

        if (AppSettings.Instance.DairyPaneDisplayMode == DairyPaneDisplayMode.Inline && MainSplitView.IsPaneOpen == false)
        {
            MainSplitView.IsPaneOpen = true;
        }

        SelectPresentView();
        // Update SubTitle
        UpdateFileInfoToTitleBar();
    }
    private void UpdateFileInfoToTitleBar()
    {
        var filePath = ViewModel.Manager.RelativeFilePath;
        string subtitle;
        if (Path.Exists(filePath))
        {
            subtitle = $"{XamlHelper.IsSavedToGlyph(ViewModel.IsSaved)}{Path.GetFileName(filePath)} - {ViewModel.CurrentFile.Name} ({ViewModel.CurrentFile.Author})";
        }
        else
        {
            subtitle = $"{"Unsaved".GetLocalized()} - {ViewModel.CurrentFile.Name} ({ViewModel.CurrentFile.Author})";
        }

        App.Window.SetSubtitle(subtitle);
    }
    private void LoadFileInfo()
    {
        if (!ViewModel.HasCurrentFile)
        {
            return;
        }
        ViewModel.CurrentText = null;

        var file = ViewModel.CurrentFile;
        var totalText = 0;
        foreach (var d in file.DairyDays)
        {
            totalText += d.Texts.Count;
        }

        FileInfoBaseName.Text = file.Name;
        FileInfoBaseAuthor.Text = file.Author;
        FileInfoBaseDescription.Text = file.Description;
        FileInfoBaseTotalText.Text = totalText.ToString();

        FileInfoTimeCreate.Text = XamlHelper.DateTimeToString(file.CreateTime);
        FileInfoTimeLastEdit.Text = XamlHelper.DateTimeToString(file.LastEditTime);

        LoadFilePasswordInfo();

        MainPresenter.ShowExtraContent();
    }
    private void LoadFilePasswordInfo()
    {
        var encrypted = ViewModel.Manager.IsEncrypted;
        FileInfoEncryptInfo.Text = (encrypted ? "FileInfoEncrypted" : "FileInfoNotEncrypted").GetLocalized();

        FileInfoEncryptedPart.Visibility = XamlHelper.ToVisible(encrypted);
        FileInfoNotEncryptedPart.Visibility = XamlHelper.ToCollapsed(encrypted);
    }
    private async void OnEditFileInfo(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.HasCurrentFile)
        {
            return;
        }

        await ShowEditFileInfoDialog();
    }

    private async ValueTask<bool> SaveCurrentFile(bool requestUserToConfirm)
    {
        if (!ViewModel.HasCurrentFile)
        {
            return true;
        }

        if (ViewModel.IsSaved)
        {
            return true;
        }
        else
        {
            if (requestUserToConfirm)
            {
                var result = await ShowIsSaveDialog();

                if (result == ContentDialogResult.Secondary)
                {
                    return true;
                }
                else if (result == ContentDialogResult.None)
                {
                    return false;
                }
            }
        }

        if (ViewModel.Manager.HasRelativeFile)
        {
            await ViewModel.Manager.SaveAsync();
            return true;
        }
        else
        {
            if (await RequestSaveAsFilePath() is string filePath)
            {
                await ViewModel.Manager.CreateAndSaveAsync(filePath);
                return true;
            }
            return false;
        }
    }
    private async ValueTask<string> RequestSaveAsFilePath()
    {
        if (!ViewModel.HasCurrentFile)
        {
            return null;
        }

        if (_savePicker is null)
        {
            _savePicker = new(ContentGrid.XamlRoot.ContentIslandEnvironment.AppWindowId);
            _savePicker.FileTypeChoices.Add("MyDairyFile".GetLocalized(), [".myd"]);
            _savePicker.FileTypeChoices.Add("JsonFile".GetLocalized(), [".json"]);
            _savePicker.FileTypeChoices.Add("AllFiles".GetLocalized(), ["*"]);
        }

        if (await _savePicker.PickSaveFileAsync() is PickFileResult result)
        {
            //await ViewModel.Manager.SaveAsFileAsync(result.Path);
            return result.Path;
        }

        return null;
    }
    private async ValueTask<bool> TrySaveAndCloseCurrentFile()
    {
        if (ViewModel.HasCurrentFile)
        {
            return await SaveCurrentFile(true);
        }
        return true;
    }
    private async ValueTask OpenDairyFile(string filePath)
    {
        try
        {
            if (await TrySaveAndCloseCurrentFile())
            {
                await ViewModel.Manager.LoadFileAsync(filePath);
                if (ViewModel.Manager.IsEncrypted)
                {
                    await ShowRequestPasswordDialog();
                }
                else
                {
                    await ViewModel.Manager.TryParseFileAsync(null);
                }
            }
        }
        catch (JsonException jsonException)
        {
            await ShowJsonException(jsonException);
        }
        catch (Exception ex)
        {
            await ShowException(ex);
        }
    }

    private void ChangePasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!ViewModel.HasCurrentFile)
        {
            return;
        }

        if (ChangePasswordControl.TryGetRequestPassword(out var requestPassword))
        {
            var succeed = requestPassword.Equals(ViewModel.CurrentPassword);
            ChangePasswordControl.ShowIncorrectPrompt(succeed);
            args.Cancel = !succeed;
        }
    }
    private void RemovePasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!ViewModel.HasCurrentFile)
        {
            return;
        }

        if (RemovePasswordControl.TryGetRequestPassword(out var requestPassword))
        {
            var succeed = requestPassword.Equals(ViewModel.CurrentPassword);
            RemovePasswordControl.ShowIncorrectPrompt(succeed);
            args.Cancel = !succeed;
        }
    }
    private async void RequestPasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;

        if (!RequestPasswordControl.TryGetRequestPassword(out var password))
        {
            return;
        }

        RequestPasswordDialog.IsPrimaryButtonEnabled = false;
        RequestPasswordDialog.Closing += PasswordDialog_Closing;

        var succeed = await ViewModel.Manager.TryParseFileAsync(password);
        RequestPasswordControl.ShowIncorrectPrompt(succeed);

        RequestPasswordDialog.Closing -= PasswordDialog_Closing;
        RequestPasswordDialog.IsPrimaryButtonEnabled = true;

        if (succeed)
        {
            RequestPasswordDialog.Hide();
        }
    }
    private void PasswordDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        args.Cancel = true;
    }
    private void NewPasswordBox_Checked(object sender, RoutedEventArgs e)
    {
        NewPasswordControl.Visibility = XamlHelper.ToVisible(NewSetPasswordBox.IsChecked == true);
    }

    private void RequestPasswordDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel.Manager.ReleaseCurrentFile();
    }
    private async void OnSetPassword(object sender, RoutedEventArgs e)
    {
        await ShowSetPasswordDialog();
    }
    private async void OnChangePassword(object sender, RoutedEventArgs e)
    {
        await ShowChangePasswordDialog();
    }
    private async void OnRemovePassword(object sender, RoutedEventArgs e)
    {
        await ShowRemovePasswordDialog();
    }
    #endregion

    #region Dairy User Interface Core
    private void ContentViewDoubleListView_InnerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ContentViewDoubleListView.InnerSelectedItem is DairyText text)
        {
            ViewModel.CurrentText = text;
        }
    }
    private void ContentViewTreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is DairyText text)
        {
            ViewModel.CurrentText = text;
        }
    }

    private void MainTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        ViewModel.CloseText((DairyText)args.Item);
    }
    private void MainTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.CurrentText = MainTabView.SelectedItem as DairyText;
        //if (MainTabView.SelectedItem is DairyText text)
        //{
        //    ViewModel.CurrentText = text;
        //}
        //else
        //{
        //    ViewModel.CurrentText = null;
        //}
    }

    private async void OnCreateNewDay(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.HasCurrentFile)
        {
            return;
        }

        CreateNewDayBox.PlaceholderText = string.Format("CreateNewDayFormat".GetLocalized(), XamlHelper.DateOnlyToString(DateOnly.FromDateTime(DateTime.Today)));

        if (ContentDialogResult.Primary == await CreateNewDayDialog.ShowAsync())
        {
            var text = CreateNewDayBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                ViewModel.Manager.CreateNewDay(DateOnly.FromDateTime(DateTime.Today));
                return;
            }

            if (DateOnly.TryParseExact(CreateNewDayBox.Text, GlobalConstants.DateOnlyFormat, out var date))
            {
                ViewModel.Manager.CreateNewDay(date);
            }
        }
    }
    private async void OnCreateNewTextContext(object sender, RoutedEventArgs e)
    {
        var context = ((FrameworkElement)sender).DataContext;

        if (context is not DairyDay day)
        {
            day = ViewModel.CurrentFile.GetDayFromText(context as DairyText);
        }

        await CreateNewText(day);
    }
    private async void OnCreateNewTextInDoubleList(object sender, RoutedEventArgs e)
    {
        if (ContentViewDoubleListView.IsLoaded)
        {
            await CreateNewText(ContentViewDoubleListView.SelectedItem as DairyDay);
        }
    }
    private async void OnCreateNewText(object sender, RoutedEventArgs e)
    {
        DairyDay GetDay()
        {
            if (ViewModel.CurrentFile.GetDayFromText(ViewModel.CurrentText) is DairyDay day)
            {
                return day;
            }

            if (ContentViewDoubleListView.IsLoaded && ContentViewDoubleListView.SelectedItem is DairyDay day1)
            {
                return day1;
            }

            if (ContentViewTreeView.IsLoaded && ContentViewTreeView.SelectedItem is DairyDay day2)
            {
                return day2;
            }

            return null;
        }

        await CreateNewText(GetDay());
    }
    private async ValueTask CreateNewText(DairyDay day)
    {
        if (!ViewModel.HasCurrentFile)
        {
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        NewTextDateBox.PlaceholderText = string.Format("CreateNewDayFormat".GetLocalized(), XamlHelper.DateOnlyToString(today));

        if (day != null && NewTextDateBox.ItemsSource != null)
        {
            NewTextDateBox.SelectedItem = day;
        }

        NewTextMoreOptionsExpander.IsExpanded = false;
        NewTextApproximateBox.IsChecked = false;

        if (ContentDialogResult.Primary == await CreateNewTextDialog.ShowAsync())
        {
            var now = DateTime.Now;
            var title = NewTextTitle.Text;
            var isApproximate = NewTextApproximateBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(title))
            {
                title = string.Format("NewTextDefaultTitleFormat".GetLocalized(), now.ToString(GlobalConstants.TimeOnlyFormat));
            }

            TimeOnly? timeOnly;
            if (isApproximate)
            {
                timeOnly = TimeOnly.TryParseExact(NewTextApproximateTextBox.Text, GlobalConstants.TimeOnlyFormat, out var time) ? time : null;
            }
            else
            {
                timeOnly = TimeOnly.FromDateTime(now);
            }

            DairyDay GetDay()
            {
                if (NewTextDateBox.SelectedItem is DairyDay selectedDay)
                {
                    return selectedDay;
                }

                var text = NewTextDateBox.Text;
                if (string.IsNullOrEmpty(text))
                {
                    return ViewModel.Manager.CreateNewDay(today);
                }

                if (DateOnly.TryParseExact(NewTextDateBox.Text, GlobalConstants.DateOnlyFormat, out var parsedDate))
                {
                    return ViewModel.Manager.CreateNewDay(parsedDate);
                }

                return null;
            }

            var newDay = GetDay();
            if (newDay != null)
            {
                ViewModel.Manager.CreateNewText(newDay, title, NewTextDescription.Text, isApproximate, timeOnly);
            }
        }
    }

    private async ValueTask TryDelete(Func<string> promptText, Action action)
    {
        if (AppSettings.Instance.NotShowDeleteAgain)
        {
            action();
        }
        else
        {
            DeleteConfirmTextBlock.Text = promptText();
            DeleteConfirmCheckBox.IsChecked = false;

            if (ContentDialogResult.Primary == await DeleteConfirmDialog.ShowAsync())
            {
                if (DeleteConfirmCheckBox.IsChecked == true)
                {
                    AppSettings.Instance.NotShowDeleteAgain = true;
                }

                action();
            }
        }
    }
    private async void OnDeleteDayContext(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is not DairyDay day)
        {
            return;
        }

        await TryDelete(() => string.Format("DeleteConfirmDayFormat".GetLocalized(), day.Date), () => ViewModel.Manager.DeleteDay(day));
    }
    private async void OnDeleteTextContext(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is not DairyText text)
        {
            return;
        }

        await TryDelete(() => string.Format("DeleteConfirmTextFormat".GetLocalized(), text.SourceDate, text.Title), () => ViewModel.Manager.DeleteText(text));
    }
    private async void OnDeleteText(object sender, RoutedEventArgs e)
    {
        await TryDelete(() => string.Format("DeleteConfirmTextFormat".GetLocalized(), ViewModel.CurrentText.SourceDate, ViewModel.CurrentText.Title), () => ViewModel.Manager.DeleteText(ViewModel.CurrentText));
    }
    private void OnViewDairyText(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is DairyText text)
        {
            ViewModel.CurrentText = text;
        }
    }

    private void OnUndo(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CanUndo)
        {
            ViewModel.Manager.Undo();
        }
    }
    private void OnRedo(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CanRedo)
        {
            ViewModel.Manager.Redo();
        }
    }
    #endregion
}

// Next : Encrypt Service
// Next : Multiple Window Manager