using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using MyDairy.Services;
using Windows.Foundation;

namespace MyDairy.Common;

[ContentProperty(Name = nameof(Content))]
public partial class ContentDialogInfo : DependencyObject
{
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ContentDialogInfo), new(null));

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ContentDialogInfo), new(null));

    public ContentDialogKind Kind
    {
        get => (ContentDialogKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof(Kind), typeof(ContentDialogKind), typeof(ContentDialogInfo), new(ContentDialogKind.Close));

    public bool IsPrimaryButtonEnabled
    {
        get => (bool)GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(nameof(IsPrimaryButtonEnabled), typeof(bool), typeof(ContentDialogInfo), new(true));

    public event TypedEventHandler<ContentDialogService, ContentDialogButtonClickEventArgs> Validating;

    internal void RaiseValidatingEvent(ContentDialogService service, ContentDialogButtonClickEventArgs args)
    {
        Validating?.Invoke(service, args);
    }
}
