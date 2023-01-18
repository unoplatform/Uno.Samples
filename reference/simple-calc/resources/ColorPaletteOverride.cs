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
			.Add<Color>(Theme.Colors.OnPrimary.Default.Key, "#FFFFFF", "#FFFFFF")
			.Add<Color>(Theme.Colors.Primary.Container.Key, "#D2D3DA", "#4E505F")
			.Add<Color>(Theme.Colors.OnPrimary.Container.Key, "#1B192C", "#E5DEFF")
			.Add<Color>(Theme.Colors.Primary.Inverse.Key, "#C8BFFF", "#5946D2")
			.Add<Color>(Theme.Colors.Primary.VariantLight.Key, "#007AFF", "#0A84FF")
			.Add<Color>(Theme.Colors.Primary.VariantDark.Key, "#4B5EFC", "#4B5EFC")
			.Add<Color>(Theme.Colors.Secondary.Default.Key, "#D2D3DA", "#5C5659")
			.Add<Color>(Theme.Colors.OnSecondary.Default.Key, "#FFFFFF", "#003823")
			.Add<Color>(Theme.Colors.Secondary.Container.Key, "#FFFFFF", "#2E2F38")
			.Add<Color>(Theme.Colors.OnSecondary.Container.Key, "#281B192C", "#28FFFFFF")
			.Add<Color>(Theme.Colors.Surface.Default.Key, "#E5E5EA", "#777274")
			.Add<Color>(Theme.Colors.OnSurface.Default.Key, "#1C1B1F", "#E6E1E5")
			.Add<Color>(Theme.Colors.Surface.Inverse.Key, "#313033", "#E6E1E5")
			.Add<Color>(Theme.Colors.OnSurface.Inverse.Key, "#F4EFF4", "#1C1B1F")
			.Add<Color>(Theme.Colors.Surface.Variant.Key, "#F2EFF5", "#49454F")
			.Add<Color>(Theme.Colors.OnSurface.Variant.Key, "#8A8494", "#CAC4D0")
			.Add<Color>(Theme.Colors.Tertiary.Default.Key, "#FF9500", "#FF9F0A")
			.Add<Color>(Theme.Colors.OnTertiary.Default.Key, "#FFFFFF", "#003259")
			.Add<Color>(Theme.Colors.Tertiary.Container.Key, "#CFE4FF", "#00497E")
			.Add<Color>(Theme.Colors.OnTertiary.Container.Key, "#001D36", "#CFE4FF")
			.Add<Color>(Theme.Colors.Error.Default.Key, "#B3261E", "#F2B8B5")
			.Add<Color>(Theme.Colors.OnError.Default.Key, "#FFFFFF", "#601410")
			.Add<Color>(Theme.Colors.Error.Container.Key, "#F9DEDC", "#8C1D18")
			.Add<Color>(Theme.Colors.OnError.Container.Key, "#410E0B", "#F9DEDC")
			.Add<Color>(Theme.Colors.Outline.Default.Key, "#79747E", "#938F99"));
	}
}