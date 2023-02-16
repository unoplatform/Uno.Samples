using Uno.Themes.Markup;

namespace SimpleCalculator.Styles;

public sealed class ColorPaletteOverride : ResourceDictionary
{
	public ColorPaletteOverride()
	{
		this.Build(r => r
			.Add<Color>(Theme.Colors.Background.Default.Key, "#F2F2F7", "#17171C")
			.Add<Color>(Theme.Colors.OnBackground.Default.Key, "#1C1B1F", "#FFFFFF")
			.Add<Color>(Theme.Colors.Primary.Default.Key, "#28CD55", "#32D74B")
			.Add<Color>(Theme.Colors.Primary.Container.Key, "#D2D3DA", "#4E505F")
			.Add<Color>(Theme.Colors.OnPrimary.Container.Key, "#1B192C", "#E5DEFF")
			.Add<Color>(Theme.Colors.Primary.VariantLight.Key, "#007AFF", "#0A84FF")
			.Add<Color>(Theme.Colors.Primary.VariantDark.Key, "#4B5EFC", "#4B5EFC")
			.Add<Color>(Theme.Colors.Secondary.Default.Key, "#D2D3DA", "#5C5659")
			.Add<Color>(Theme.Colors.Secondary.Container.Key, "#FFFFFF", "#2E2F38")
			.Add<Color>(Theme.Colors.OnSecondary.Container.Key, "#661B192C", "#66FFFFFF")
			.Add<Color>(Theme.Colors.Surface.Default.Key, "#E5E5EA", "#777274")
			.Add<Color>(Theme.Colors.Surface.Variant.Key, "#F2EFF5", "#49454F")
			.Add<Color>(Theme.Colors.OnSurface.Variant.Key, "#8A8494", "#CAC4D0")
			.Add<Color>(Theme.Colors.Tertiary.Default.Key, "#FF9500", "#FF9F0A"));
	}
}