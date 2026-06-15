using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MyDairy.Common;
using MyDairy.Helpers;
using Microsoft.UI.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AboutPage : Page, ISupportContainedInWindow
{
    public static AboutPage Instance
    {
        get;
        private set;
    } = null;

    public AboutPage()
    {
        InitializeComponent();

        var versionString = string.Format("AboutVersionFormat".GetLocalized(), GlobalConstants.CurrentVersion.ToString());
        AboutVersion.Text = versionString;
    }

    public string Title => "MenuHelpAbout/Text".GetLocalized();

    public event EventHandler TitleChanged;

    public bool GetIsBackButtonVisible() => false;
    public void SetContainToWindow(bool inWindow)
    {
    }
    public void OnBackRequested()
    {
    }
}
