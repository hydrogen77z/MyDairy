using System;
using System.Collections.Generic;
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
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Models;
using MyDairy.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Controls;

public sealed partial class DairyTextControl : UserControl, ISupportContainedInWindow
{
    private static DataTemplate _dairyTextPresentTemplate;

    public static void SetDairyTextTemplate(DataTemplate template)
    {
        _dairyTextPresentTemplate = template;
    }

    public DairyTextControl()
    {
        InitializeComponent();

        DairyPresenter.ContentTemplate = _dairyTextPresentTemplate;
    }

    public DairyText DairyText
    {
        get => (DairyText)DairyPresenter.Content;
        set
        {
            if (DairyPresenter.Content != value)
            {
                DairyText?.PropertyChanged -= OnDairyTextTitlePropertyChanged;
                value.PropertyChanged += OnDairyTextTitlePropertyChanged;

                DairyPresenter.Content = value;
            }
        }
    }

    public double GetExtraWidth()
    {
        return DairyPresenter.Padding.Left + DairyPresenter.Padding.Right;
    }

    public double GetExtraHeight()
    {
        return DairyPresenter.Padding.Top + DairyPresenter.Padding.Bottom + 32; 
    }

    public void SetContainToWindow(bool inWindow)
    {
    }

    private void OnDairyTextTitlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DairyText.Title) || e.PropertyName == nameof(DairyText.IsSaved))
        {
            TitleChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnSaveKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        DairyPage.Instance.SaveCurrent();
    }

    public bool GetIsBackButtonVisible()
    {
        return true;
    }
    public void OnBackRequested()
    {
        DairyPage.Instance.ViewModel.OpenText(DairyText);
    }

    public string Title => XamlHelper.IsSavedToGlyph(DairyText.IsSaved) + DairyText.Title;

    public event EventHandler TitleChanged;
}
