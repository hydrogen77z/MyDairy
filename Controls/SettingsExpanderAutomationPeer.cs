using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace MyDairy.Controls;

public partial class SettingsExpanderAutomationPeer(SettingsExpander owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Button;

    protected override string GetClassNameCore() => nameof(SettingsExpander);

    public void RaiseExpandedChangedEvent(bool newValue)
    {
        var newState = (ExpandCollapseState)(newValue ? 1 : 0);
        var oldState = (ExpandCollapseState)(newValue ? 0 : 1);

        RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
    }
}
