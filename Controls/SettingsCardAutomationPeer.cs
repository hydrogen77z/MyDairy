using Microsoft.UI.Xaml.Automation.Peers;

namespace MyDairy.Controls;

public partial class SettingsCardAutomationPeer(SettingsCard owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;

    protected override string GetClassNameCore() => nameof(SettingsCard);
}
