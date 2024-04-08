namespace TubePlayer.Styles;

public partial class ColorPaletteOverride : ResourceDictionary
{
    public ColorPaletteOverride()
    {
        this
            .Build
            (
                r => r
                    .Add<Color>("StatusBarForegroundColor", light: "#000000", dark: "#FFFFFF")
                    .Add<Color>(Theme.Colors.Surface.Default, light: "#FFFFFF", dark: "#302D37")
                    .Add<Color>(Theme.Colors.OnSurface.Default, light: "#1C1B1F", dark: "#E6E1E5")
                    .Add<Color>(Theme.Colors.OnSurface.Variant, light: "#8A8494", dark: "#C9C5D0")
                    .Add<Color>(Theme.Colors.Primary.Default, light: "#5946D2", dark: "#C7BFFF")
                    .Add<Color>(Theme.Colors.Surface.Variant, light: "#F2EFF5", dark: "#47464F")
                    .Add<Color>(Theme.Colors.OnPrimary.Default, light: "#FFFFFF", dark: "#2A009F")
                    .Add<Color>(Theme.Colors.OnError.Default, light: "#FFFFFF", dark: "#690005")
                    .Add<Color>(Theme.Colors.Error.Default, light: "#B3261E", dark: "#FFB4AB")
                    .Add<Color>(Theme.Colors.OnPrimary.Container, light: "#170065", dark: "#E4DFFF")
                    .Add<Color>(Theme.Colors.Background.Default, light: "#FCFBFF", dark: "#1C1B1F")
                    .Add<Color>(Theme.Colors.Primary.Inverse, light: "#C8BFFF", dark: "#2A009F")
                    .Add<Color>(Theme.Colors.Primary.Container, light: "#E5DEFF", dark: "#4129BA")
                    .Add<Color>(Theme.Colors.Primary.VariantLight, light: "#9679FF", dark: "#C8BFFF")
                    .Add<Color>(Theme.Colors.Primary.VariantDark, light: "#0021C1", dark: "#4128BA")
                    .Add<Color>(Theme.Colors.Secondary.Default, light: "#6B4EA2", dark: "#CDC2DC")
                    .Add<Color>(Theme.Colors.Secondary.VariantDark, light: "#3B2574", dark: "#441F8A")
                    .Add<Color>(Theme.Colors.Secondary.VariantLight, light: "#9B7BD5", dark: "#EBE6F1")
                    .Add<Color>(Theme.Colors.OnSecondary.Default, light: "#FFFFFF", dark: "#332D41")
                    .Add<Color>(Theme.Colors.Secondary.Container, light: "#EBDDFF", dark: "#433C52")
                    .Add<Color>(Theme.Colors.OnSecondary.Container, light: "#220555", dark: "#EDDFFF")
                    .Add<Color>(Theme.Colors.OnBackground.Default, light: "#1C1B1F", dark: "#E5E1E6")
                    .Add<Color>(Theme.Colors.Surface.Inverse, light: "#313033", dark: "#E6E1E5")
                    .Add<Color>(Theme.Colors.Surface.Tint, light: "#5946D2", dark: "#C7BFFF")
                    .Add<Color>(Theme.Colors.OnSurface.Inverse, light: "#F4EFF4", dark: "#1C1B1F")
                    .Add<Color>(Theme.Colors.Outline.Default, light: "#79747E", dark: "#928F99")
                    .Add<Color>(Theme.Colors.Outline.Variant, light: "#C9C5D0", dark: "#57545D")
                    .Add<Color>(Theme.Colors.Error.Container, light: "#F9DEDC", dark: "#93000A")
                    .Add<Color>(Theme.Colors.OnError.Container, light: "#410E0B", dark: "#FFDAD6")
                    .Add<Color>(Theme.Colors.Tertiary.Default, light: "#0061A4", dark: "#9FCAFF")
                    .Add<Color>(Theme.Colors.OnTertiary.Default, light: "#FFFFFF", dark: "#003258")
                    .Add<Color>(Theme.Colors.Tertiary.Container, light: "#CFE4FF", dark: "#00497D")
                    .Add<Color>(Theme.Colors.OnTertiary.Container, light: "#001D36", dark: "#D1E4FF")
            )
            ;
    }
}
