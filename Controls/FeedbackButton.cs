using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Controls;

public sealed partial class FeedbackButton : Button
{
    private Storyboard _animation;
    public FeedbackButton()
    {
        DefaultStyleKey = typeof(FeedbackButton);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Click += FeedbackButton_Click;
    }

    private void FeedbackButton_Click(object sender, RoutedEventArgs e)
    {
        _animation ??= GetTemplateChild("FeedbackAnimation") as Storyboard;

        var args = new FeedbackButtonClickEventArgs();
        FeedbackClick?.Invoke(this, args);

        if (args.BeginStoryboard)
        {
            _animation?.Begin();
        }
    }

    public object FeedBackContent
    {
        get => GetValue(FeedBackContentProperty);
        set => SetValue(FeedBackContentProperty, value);
    }

    public static readonly DependencyProperty FeedBackContentProperty = DependencyProperty.Register(nameof(FeedBackContent), typeof(object), typeof(FeedbackButton), new(null));

    public event TypedEventHandler<FeedbackButton, FeedbackButtonClickEventArgs> FeedbackClick;
}
