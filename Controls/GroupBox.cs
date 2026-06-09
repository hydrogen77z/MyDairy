using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Controls;

public sealed partial class GroupBox : ContentControl
{
    private TextBlock _headerText;

    private Border _mainBorder;
    private Border _topLeftBorder;
    private Border _topRightBorder;

    public GroupBox()
    {
        DefaultStyleKey = typeof(GroupBox);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _headerText = GetTemplateChild("HeaderText").As<TextBlock>();
        _mainBorder = GetTemplateChild("MainBorder").As<Border>();
        _topLeftBorder = GetTemplateChild("TopLeftBorder").As<Border>();
        _topRightBorder = GetTemplateChild("TopRightBorder").As<Border>();

        SizeChanged += GroupBox_SizeChanged;
    }

    private void GroupBox_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        CalculateWidth();
    }

    private void CalculateWidth()
    {
        var borderMargin = new Thickness(0)
        {
            Top = _headerText.ActualHeight / 2,
        };

        _mainBorder.Margin = borderMargin;
        _topLeftBorder.Margin = borderMargin;
        _topRightBorder.Margin = borderMargin;

        _headerText.Margin = new(Padding.Left + 1, 0, 0, 0);

        _topLeftBorder.Width = Padding.Left;
        _topLeftBorder.CornerRadius = new(CornerRadius.TopLeft, 0, 0, 0);

        _topRightBorder.Width = ActualWidth - Padding.Left - _headerText.ActualWidth - 2;
        _topRightBorder.CornerRadius = new(0, CornerRadius.TopRight, 0, 0);
    }

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(GroupBox), new(null));

    public double HeaderLeft
    {
        get => (double)GetValue(HeaderLeftProperty);
        set => SetValue(HeaderLeftProperty, value);
    }

    public static readonly DependencyProperty HeaderLeftProperty = DependencyProperty.Register(nameof(HeaderLeft), typeof(double), typeof(GroupBox), new(null));

}
