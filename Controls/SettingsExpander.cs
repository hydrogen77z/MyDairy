using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using WinRT;

namespace MyDairy.Controls; 

[ContentProperty(Name = nameof(Items))]
[GeneratedBindableCustomProperty]
public partial class SettingsExpander : ItemsControl
{
    public SettingsExpander()
    {
        DefaultStyleKey = typeof(SettingsExpander);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (Header is string headerString && headerString != string.Empty)
        {
            if (!string.IsNullOrEmpty(headerString) && string.IsNullOrEmpty(AutomationProperties.GetName(this)))
            {
                AutomationProperties.SetName(this, headerString);
            }
        }

        if (Items.Count > 0)
        {
            ((SettingsCard)Items[^1]).CornerRadius = new(0, 0, 4, 4);
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new SettingsExpanderAutomationPeer(this);

    protected override bool IsItemItsOwnContainerOverride(object item) => true;

    private void OnIsExpandedChanged(bool newValue)
    {
        (FrameworkElementAutomationPeer.FromElement(this) as SettingsExpanderAutomationPeer)?.RaiseExpandedChangedEvent(newValue);
    }
}
