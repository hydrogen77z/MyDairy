using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Settings;
using MyDairy.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Storage.Streams;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using MyDairy.Services;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy;

public sealed partial class MainWindow : Window
{
    private readonly WindowService _service;
    private readonly MultipleWindowManager _multipleWindowManager;

    public WindowExtendedConfiguration ExtendedConfiguration
    {
        get;
    }

    public Brush MenuBarBrush
    {
        get;
    }
    public Brush ContentBrush
    {
        get;
    }

    public event EventHandler PaneToggleRequested;

    public MainWindow()
    {
        InitializeComponent();

        //AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(MainTitleBar);

        ComponentHelper.SetWindowIcon(this);

        // Reversed
        //PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE) | (int)WINDOW_EX_STYLE.WS_EX_LAYERED);

        ExtendedConfiguration = new()
        {
            BackdropType = AppSettings.Instance.BackdropType,
            RequestedTheme = AppSettings.Instance.Theme,
            UseInterceptTitleBarCaptionAreaChanged = true,
        };
        _service = new(this, ExtendedConfiguration)
        {
            MinHeight = 600,
            MinWidth = 600,
        };
        _multipleWindowManager = new();

        AppWindow.Closing += OnClosing;
        if (AppSettings.Instance.WindowPosition is RectInt32 rect)
        {
            AppWindow.MoveAndResize(rect);
        }

        MenuBarBrush = Application.Current.Resources["MenuBarBackgroundBrush"].As<Brush>();
        ContentBrush = Application.Current.Resources["ContentBackgroundBrush"].As<Brush>();

        MenuBarBrush.Opacity = AppSettings.Instance.MenuBarOpacity;
        ContentBrush.Opacity = AppSettings.Instance.ContentOpacity;

        AppSettings.Instance.PropertyChanged += OnSettingPropertyChanged;
    }

    private void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(AppSettings.MenuBarOpacity):
                MenuBarBrush.Opacity = AppSettings.Instance.MenuBarOpacity;
                break;
            case nameof(AppSettings.ContentOpacity):
                ContentBrush.Opacity = AppSettings.Instance.ContentOpacity;
                break;
            case nameof(AppSettings.BackdropType):
                ExtendedConfiguration.BackdropType = AppSettings.Instance.BackdropType;
                break;
            case nameof(AppSettings.Theme):
                ExtendedConfiguration.RequestedTheme = AppSettings.Instance.Theme;
                break;
        }
    }

    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        _multipleWindowManager.CloseAll();

        AppSettings.Instance.WindowPosition = new(sender.Position.X, sender.Position.Y, sender.Size.Width, sender.Size.Height);
        AppSettings.Instance.Save();
    }

    private void OnPaneToggleRequested(TitleBar sender, object args)
    {
        PaneToggleRequested?.Invoke(this, null);
    }

    private void OnTitleBarBackRequested(TitleBar sender, object args)
    {
        ReturnToDairyPage();
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        MainTitleBar.IsBackButtonVisible = e.Content is not DairyPage;
        MainTitleBar.RecomputeDragRegions();
    }

    public void ReturnToDairyPage()
    {
        ContentFrame.Navigate(typeof(DairyPage));
    }

    // In C++, No need to make it so complicated.
    public void OpenPageWithInstance<T>(bool inWindow, SizeInt32 size, T instance) where T : ISupportContainedInWindow, new()
    {
        // The Window existed
        foreach (var contentWindow in _multipleWindowManager.Windows)
        {
            if (contentWindow.GetContent() is T)
            {
                if (inWindow)
                {
                    contentWindow.Activate();
                    return;
                }
                contentWindow.Close();
                break;
            }
        }

        if (inWindow)
        {
            if (ContentFrame.Content is T)
            {
                ReturnToDairyPage();
            }

            ContentWindow contentWindow = new();
            contentWindow.SetContent(instance ?? new());
            contentWindow.AppWindow.Resize(size);
            OpenContentWindow(contentWindow);

            return;
        }

        ContentFrame.Navigate(typeof(T));
        if (ContentFrame.Content is ISupportContainedInWindow window)
        {
            window.SetContainToWindow(false);
        }
    }
    public void OpenNotePage(bool inWindow)
    {
        OpenPageWithInstance(inWindow, AppWindow.Size, NotePage.Instance);
    }
    public void OpenSettingsPage(bool inWindow)
    {
        OpenPageWithInstance(inWindow, AppWindow.Size, SettingsPage.Instance);
    }
    public void OpenAboutPage(bool inWindow)
    {
        OpenPageWithInstance(inWindow, new(600, 400), AboutPage.Instance);
    }

    public void OpenContentWindow(ContentWindow contentWindow)
    {
        _multipleWindowManager.CreateWindowWithConfiguration(contentWindow);
    }
    public void SetSubtitle(string text)
    {
        MainTitleBar.Subtitle = text;
    }
}
