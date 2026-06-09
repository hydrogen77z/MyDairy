using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MyDairy.Controls;

public partial class SettingsCard
{
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty
      = DependencyProperty.Register(nameof(Header), typeof(object), typeof(SettingsCard), new(null, (d, e) => ((SettingsCard)d).OnHeaderChanged()));

    public object Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty
      = DependencyProperty.Register(nameof(Description), typeof(object), typeof(SettingsCard), new(null, (d, e) => ((SettingsCard)d).OnDescriptionChanged()));

    public IconElement HeaderIcon
    {
        get => (IconElement)GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    public static readonly DependencyProperty HeaderIconProperty
      = DependencyProperty.Register(nameof(HeaderIcon), typeof(IconElement), typeof(SettingsCard), new(null, (d, e) => ((SettingsCard)d).OnHeaderIconChanged()));

    public IconElement ActionIcon
    {
        get => (IconElement)GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }

    public static readonly DependencyProperty ActionIconProperty
      = DependencyProperty.Register(nameof(ActionIcon), typeof(IconElement), typeof(SettingsCard), new("\uE974"));

    public string ActionIconToolTip
    {
        get => (string)GetValue(ActionIconToolTipProperty);
        set => SetValue(ActionIconToolTipProperty, value);
    }

    public static readonly DependencyProperty ActionIconToolTipProperty
      = DependencyProperty.Register(nameof(ActionIconToolTip), typeof(string), typeof(SettingsCard), new("More"));

    public bool IsClickEnabled
    {
        get => (bool)GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }

    public static readonly DependencyProperty IsClickEnabledProperty
      = DependencyProperty.Register(nameof(IsClickEnabled), typeof(bool), typeof(SettingsCard), new(false, (d, e) => ((SettingsCard)d).OnIsClickEnabledChanged()));

    public ContentAlignment ContentAlignment
    {
        get => (ContentAlignment)GetValue(ContentAlignmentProperty);
        set => SetValue(ContentAlignmentProperty, value);
    }

    public static readonly DependencyProperty ContentAlignmentProperty
      = DependencyProperty.Register(nameof(ContentAlignment), typeof(ContentAlignment), typeof(SettingsCard), new(ContentAlignment.Right, (d, e) => ((SettingsCard)d).OnContentAlignmentChanged()));

    public double WrapThresholdWidth
    {
        get => (double)GetValue(WrapThresholdWidthProperty);
        set => SetValue(WrapThresholdWidthProperty, value);
    }

    public static readonly DependencyProperty WrapThresholdWidthProperty
      = DependencyProperty.Register(nameof(WrapThresholdWidth), typeof(double), typeof(SettingsCard), new(460d, (d, e) => ((SettingsCard)d).OnWrapThresholdWidthChanged(((FrameworkElement)d).ActualWidth)));
}

public enum ContentAlignment
{
    Right,
    Left,
    Vertical
}