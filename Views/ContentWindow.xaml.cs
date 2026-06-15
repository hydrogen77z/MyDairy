using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Views;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ContentWindow : Window
{
    private ISupportContainedInWindow _windowInterface = null;
    private readonly WindowService _service;

    public ContentWindow()
    {
        InitializeComponent();

        AppWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(MainTitleBar);

        ComponentHelper.SetWindowIcon(this);

        _service = new(this, App.Window.ExtendedConfiguration);
    }

    public object GetContent()
    {
        return RootPresenter.Content;
    }

    public void SetContent(ISupportContainedInWindow content)
    {
        _windowInterface = content;
        RootPresenter.Content = _windowInterface;

        SetTitle(_windowInterface.Title);

        _windowInterface.SetContainToWindow(true);
        _windowInterface.TitleChanged += ContainedInterface_TitleChanged;

        MainTitleBar.IsBackButtonVisible = _windowInterface.GetIsBackButtonVisible();
        MainTitleBar.BackRequested += MainTitleBar_BackRequested;
    }

    private void SetTitle(string title)
    {
        Title = title;
        MainTitleBar.Title = title;
    }

    private void MainTitleBar_BackRequested(TitleBar sender, object args)
    {
        _windowInterface?.OnBackRequested();

        Close();
    }

    private void ContainedInterface_TitleChanged(object sender, EventArgs e)
    {
        SetTitle(_windowInterface.Title);
    }
}
