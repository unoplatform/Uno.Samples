using Uno.Themes.Markup;

namespace SimpleCalculator.Styles;

public sealed class ColorPaletteOverride : ResourceDictionary
{
	public ColorPaletteOverride()
	{
		this.Build(r => r
			.Add<Color>(Theme.Colors.Background.Default.Key, "#F2F2F7", "#17171C")
			.Add<Color>(Theme.Colors.Primary.Default.Key, "#4B5EFC", "#4B5EFC")
			.Add<Color>(Theme.Colors.Secondary.Container.Key, "#FFFFFF", "#2E2F38")
			.Add<Color>(Theme.Colors.OnSecondary.Container.Key, "#9C9BA6", "#95969F")
			.Add<Color>(Theme.Colors.OnBackground.Default.Key, "#1B1B1F", "#E4E1E6")
			.Add<Color>(Theme.Colors.OnPrimary.Default.Key, "#FFFFFF", "#003259")
			.Add<Color>(Theme.Colors.Secondary.Default.Key, "#D2D3DA", "#4E505F")
			.Add<Color>(Theme.Colors.OnSurface.Default.Key, "#1C1B1F", "#E6E1E5")
			.Add<Color>(Theme.Colors.Surface.Default.Key, "#FFFFFF", "#2E2F38")
			.Add<Color>(Theme.Colors.Primary.Inverse.Key, "#B7C4FF", "#2B53CE")
			.Add<Color>(Theme.Colors.Primary.Container.Key, "#DCE1FF", "#0039B4")
			.Add<Color>(Theme.Colors.Primary.VariantLight.Key, "#007AFF", "#0A84FF")
			.Add<Color>(Theme.Colors.Primary.VariantDark.Key, "#4B5EFC", "#4B5EFC")
			.Add<Color>(Theme.Colors.OnSecondary.Default.Key, "#FFFFFF", "#2B3042")
			.Add<Color>(Theme.Colors.Surface.Inverse.Key, "#303034", "#E4E1E6")
			.Add<Color>(Theme.Colors.Surface.Variant.Key, "#F2EFF5", "#49454F")
			.Add<Color>(Theme.Colors.OnSurface.Inverse.Key, "#F2F0F4", "#1B1B1F")
			.Add<Color>(Theme.Colors.OnSurface.Variant.Key, "#8A8494", "#CAC4D0")
			.Add<Color>(Theme.Colors.Outline.Default.Key, "#767680", "#90909A")
			.Add<Color>(Theme.Colors.Error.Default.Key, "#BA1A1A", "#FFB4AB")
			.Add<Color>(Theme.Colors.OnError.Default.Key, "#FFFFFF", "#690005")
			.Add<Color>(Theme.Colors.Error.Container.Key, "#FFDAD6", "#93000A")
			.Add<Color>(Theme.Colors.OnError.Container.Key, "#410002", "#FFDAD6")
			.Add<Color>(Theme.Colors.Tertiary.Default.Key, "#75546F", "#E3BADA")
			.Add<Color>(Theme.Colors.OnTertiary.Default.Key, "#FFFFFF", "#43273F")
			.Add<Color>(Theme.Colors.OnTertiary.Container.Key, "#2C1229", "#FFD7F5")
			.Add<Color>(Theme.Colors.Tertiary.Container.Key, "#FFD7F5", "#5C3D57"));
	}
}