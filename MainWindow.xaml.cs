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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy;

public sealed partial class MainWindow : Window
{
    public WindowService Service
    {
        get;
    }

    public event EventHandler PaneToggleRequested;

    public unsafe MainWindow()
    {
        InitializeComponent();

        //AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(MainTitleBar);

        HWND hWnd = new((nint)AppWindow.Id.Value);
        var hInstance = PInvoke.GetModuleHandle((PCWSTR)null);
        var bigIcon = PInvoke.LoadImage(hInstance, (char*)32512, GDI_IMAGE_TYPE.IMAGE_ICON, 32, 32, IMAGE_FLAGS.LR_DEFAULTCOLOR);
        var smallIcon = PInvoke.LoadImage(hInstance, (char*)32512, GDI_IMAGE_TYPE.IMAGE_ICON, 16, 16, IMAGE_FLAGS.LR_DEFAULTCOLOR);

        PInvoke.SendMessage(hWnd, PInvoke.WM_SETICON, PInvoke.ICON_SMALL, (nint)smallIcon.Value);
        PInvoke.SendMessage(hWnd, PInvoke.WM_SETICON, PInvoke.ICON_BIG, (nint)bigIcon.Value);

        // Reversed
        //PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE) | (int)WINDOW_EX_STYLE.WS_EX_LAYERED /* WS_EX_LAYERED */);

        RootGrid.RequestedTheme = AppSettings.Instance.Theme;
        Service = new(this)
        {
            MinHeight = 500,
            MinWidth = 600,
            BackdropType = AppSettings.Instance.BackdropType,
            UseInterceptTitleBarCaptionAreaChanged = true,
        };

        AppWindow.Closing += OnClosing;
        if (AppSettings.Instance.WindowPosition is RectInt32 rect)
        {
            AppWindow.MoveAndResize(rect);
        }

        AppSettings.Instance.PropertyChanged += OnSettingPropertyChanged;
    }

    private void RecomputeDragRegions()
    {
        MainTitleBar.RecomputeDragRegions();
    }

    private void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppSettings.Theme))
        {
            RootGrid.RequestedTheme = AppSettings.Instance.Theme;
        }
        else if (e.PropertyName == nameof(AppSettings.BackdropType))
        {
            Service.BackdropType = AppSettings.Instance.BackdropType;
        }
    }

    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
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

    public void ReturnToDairyPage()
    {
        ContentFrame.Navigate(typeof(DairyPage));

        MainTitleBar.IsBackButtonVisible = false;
        RecomputeDragRegions();
    }
    public void OpenPage(Type type)
    {
        ContentFrame.Navigate(type);

        MainTitleBar.IsBackButtonVisible = true;
        RecomputeDragRegions();
    }

    public void SetSubtitle(string text)
    {
        MainTitleBar.Subtitle = text;
    }
}
