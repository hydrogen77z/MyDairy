using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MyDairy.Controls;

public partial class SettingsExpanderItemStyleSelector : StyleSelector
{
    public Style DefaultStyle
    {
        get; set;
    }
    public Style ClickableStyle
    {
        get; set;
    }

    protected override Style SelectStyleCore(object item, DependencyObject container)
        => container is SettingsCard card && card.IsClickEnabled ? ClickableStyle : DefaultStyle;
}
