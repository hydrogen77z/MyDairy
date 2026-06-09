using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MyDairy.Controls;
using MyDairy.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MyDairy.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DebugPage : Page
{
    private Window _child;

    public DebugPage()
    {
        InitializeComponent();

        MainPresenter.Content = new DairyText()
        {
            Body = "TEST",
        };

        //PureTabView.TabItemsSource = _items;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var passwordChar = PasswordBox.PasswordChar;
        Presenter.Content = null;
        MainPresenter.Content = null;
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        _child ??= new Window();

        _child.Activate();
    }
}
