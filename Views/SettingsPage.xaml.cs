using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MyDairy.Common;
using MyDairy.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page, ISupportContainedInWindow
{
    private bool _inWindow;

    public static SettingsPage Instance
    {
        get;
        private set;
    } = null;

    public SettingsPage()
    {
        InitializeComponent();

        Instance = this;
    }

    public string Title => "Header/Text".GetLocalized("Settings");

    public event EventHandler TitleChanged;

    public bool GetIsBackButtonVisible() => _inWindow;
    public void OnBackRequested() => App.Window.OpenSettingsPage(false);
    public void SetContainToWindow(bool inWindow)
    {
        _inWindow = inWindow;
        CommandNewWindow.Visibility = XamlHelper.ToCollapsed(inWindow);
        CommandReturn.Visibility = XamlHelper.ToVisible(inWindow);
    }

    private void OnOpenInNewWindow(object sender, RoutedEventArgs e)
    {
        App.Window.OpenSettingsPage(true);
    }
    private void OnReturnMainPage(object sender, RoutedEventArgs e)
    {
        App.Window.OpenSettingsPage(false);
    }
}
