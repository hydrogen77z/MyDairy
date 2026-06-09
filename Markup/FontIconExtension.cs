using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace MyDairy.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
public partial class FontIconExtension : MarkupExtension
{
    internal static readonly FontFamily _symbolThemeFontFamily = new("Segoe Fluent Icons,Segoe MDL2 Assets");
    public string Glyph
    {
        get; set;
    }

    public FontFamily FontFamily
    {
        get; set;
    }

    protected override object ProvideValue() => new FontIcon
    {
        Glyph = Glyph ?? string.Empty,
        FontFamily = _symbolThemeFontFamily,
    };
}
