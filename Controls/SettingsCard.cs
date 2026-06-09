using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup; 

namespace MyDairy.Controls;

[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = ActionIconPresenter, Type = typeof(ContentControl))]
[TemplatePart(Name = HeaderPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = DescriptionPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = HeaderIconPresenterHolder, Type = typeof(Viewbox))]
public partial class SettingsCard : ButtonBase
{
    internal const string NormalState = "Normal";
    internal const string PointerOverState = "PointerOver";
    internal const string PressedState = "Pressed";
    internal const string DisabledState = "Disabled";

    internal const string ActionIconPresenter = "PART_ActionIconPresenter";
    internal const string HeaderPresenter = "PART_HeaderPresenter";
    internal const string DescriptionPresenter = "PART_DescriptionPresenter";
    internal const string HeaderIconPresenterHolder = "PART_HeaderIconPresenterHolder";

    public SettingsCard()
    {
        DefaultStyleKey = typeof(SettingsCard);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        OnHeaderChanged();
        OnHeaderIconChanged();
        OnDescriptionChanged();
        OnIsClickEnabledChanged();
        VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
        RegisterAutomation();
        IsEnabledChanged += OnIsEnabledChanged;
        SizeChanged += OnSizeChanged;
    }

    private void RegisterAutomation()
    {
        if (Header is string headerString && headerString != string.Empty)
        {
            AutomationProperties.SetName(this, headerString);
        }
    }

    private void EnableButtonInteraction()
    {
        DisableButtonInteraction();

        PointerEntered += ControlPointerEntered;
        PointerExited += ControlPointerExited;
        PreviewKeyDown += ControlPreviewKeyDown;
        PreviewKeyUp += ControlPreviewKeyUp;
    }
    private void DisableButtonInteraction()
    {
        PointerEntered -= ControlPointerEntered;
        PointerExited -= ControlPointerExited;
        PreviewKeyDown -= ControlPreviewKeyDown;
        PreviewKeyUp -= ControlPreviewKeyUp;
    }
    private void ControlPreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
        {
            VisualStateManager.GoToState(this, NormalState, true);
        }
    }
    private void ControlPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
        {
            VisualStateManager.GoToState(this, PressedState, true);
        }
    }
    private void ControlPointerExited(object sender, PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        VisualStateManager.GoToState(this, NormalState, true);
    }
    private void ControlPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        VisualStateManager.GoToState(this, PointerOverState, true);
    }
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        if (IsClickEnabled)
        {
            base.OnPointerPressed(e);
            VisualStateManager.GoToState(this, PressedState, true);
        }
    }
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        if (IsClickEnabled)
        {
            base.OnPointerReleased(e);
            VisualStateManager.GoToState(this, NormalState, true);
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new SettingsCardAutomationPeer(this);
    private void OnHeaderChanged()
    {
        if (GetTemplateChild(HeaderPresenter) is UIElement headerPresenter)
        {
            headerPresenter.Visibility = Header != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    private void OnDescriptionChanged()
    {
        if (GetTemplateChild(DescriptionPresenter) is UIElement descriptionPresenter)
        {
            descriptionPresenter.Visibility = Description != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    private void OnHeaderIconChanged()
    {
        if (GetTemplateChild(HeaderIconPresenterHolder) is UIElement headerIconPresenter)
        {
            headerIconPresenter.Visibility = HeaderIcon != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    private void OnButtonIconChanged()
    {
        if (GetTemplateChild(ActionIconPresenter) is UIElement buttonIconPresenter)
        {
            buttonIconPresenter.Visibility = IsClickEnabled ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    private void OnIsClickEnabledChanged()
    {
        OnButtonIconChanged();
        if (IsClickEnabled)
        {
            EnableButtonInteraction();
        }
        else
        {
            DisableButtonInteraction();
        }
    }
    private void OnContentAlignmentChanged()
    {
        VisualStateManager.GoToState(this, ContentAlignment switch
        {
            ContentAlignment.Left => "Left",
            ContentAlignment.Vertical => "Vertical",
            _ => "Right",
        }, true);
    }
    private void OnWrapThresholdWidthChanged(double newWidth)
    {
        VisualStateManager.GoToState(this, newWidth < WrapThresholdWidth ? "RightWrapped" : "Right", true);
    }
    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        OnWrapThresholdWidthChanged(e.NewSize.Width);
    }
}
