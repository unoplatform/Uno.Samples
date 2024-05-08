namespace UnoPongWars.Styles;
public sealed partial class ColorPaletteOverride : ResourceDictionary
{
    public ColorPaletteOverride()
    {
        this.Build
        (
            r => r
                // Override the Primary.Default Color
                //.Add(Uno.Themes.Markup.Theme.Colors.Primary.Default, light: "#5946D2", dark: "#C7BFFF")
                .Add(Uno.Themes.Markup.Theme.Colors.Primary.Default, light: "#1B9AF9", dark: "#1B9AF9")

                .Add(Uno.Themes.Markup.Theme.Colors.Primary.Container, light: "#E5DEFF", dark: "#4129BA")
                .Add(Uno.Themes.Markup.Theme.Colors.OnPrimary.Default, light: "#FFFFFF", dark: "#2A009F")
                .Add(Uno.Themes.Markup.Theme.Colors.OnPrimary.Container, light: "#170065", dark: "#E4DFFF")
                .Add(Uno.Themes.Markup.Theme.Colors.Secondary.Default, light: "#6B4EA2", dark: "#CDC2DC")
                .Add(Uno.Themes.Markup.Theme.Colors.Secondary.Container, light: "#EBDDFF", dark: "#433C52")
                .Add(Uno.Themes.Markup.Theme.Colors.OnSecondary.Default, light: "#FFFFFF", dark: "#332D41")
                .Add(Uno.Themes.Markup.Theme.Colors.OnSecondary.Container, light: "#220555", dark: "#EDDFFF")
                .Add(Uno.Themes.Markup.Theme.Colors.Tertiary.Default, light: "#0061A4", dark: "#9FCAFF")
                .Add(Uno.Themes.Markup.Theme.Colors.Tertiary.Container, light: "#CFE4FF", dark: "#00497D")
                .Add(Uno.Themes.Markup.Theme.Colors.OnTertiary.Default, light: "#FFFFFF", dark: "#003258")
                .Add(Uno.Themes.Markup.Theme.Colors.OnTertiary.Container, light: "#001D36", dark: "#D1E4FF")
                .Add(Uno.Themes.Markup.Theme.Colors.Error.Default, light: "#B3261E", dark: "#FFB4AB")
                .Add(Uno.Themes.Markup.Theme.Colors.Error.Container, light: "#F9DEDC", dark: "#93000A")
                .Add(Uno.Themes.Markup.Theme.Colors.OnError.Default, light: "#FFFFFF", dark: "#690005")
                .Add(Uno.Themes.Markup.Theme.Colors.OnError.Container, light: "#410E0B", dark: "#FFDAD6")
                .Add(Uno.Themes.Markup.Theme.Colors.Background.Default, light: "#FCFBFF", dark: "#1C1B1F")
                .Add(Uno.Themes.Markup.Theme.Colors.OnBackground.Default, light: "#1C1B1F", dark: "#E5E1E6")
                .Add(Uno.Themes.Markup.Theme.Colors.Surface.Default, light: "#FFFFFF", dark: "#302D37")

                // Override the Surface.Variant Color
                //.Add(Uno.Themes.Markup.Theme.Colors.Surface.Variant, light: "#F2EFF5", dark: "#47464F")
                .Add(Uno.Themes.Markup.Theme.Colors.Surface.Variant, light: "#79747E", dark: "#928F99")

                .Add(Uno.Themes.Markup.Theme.Colors.Surface.Inverse, light: "#313033", dark: "#E6E1E5")
                .Add(Uno.Themes.Markup.Theme.Colors.Surface.Tint, light: "#5946D2", dark: "#C7BFFF")
                .Add(Uno.Themes.Markup.Theme.Colors.OnSurface.Default, light: "#1C1B1F", dark: "#E6E1E5")
                .Add(Uno.Themes.Markup.Theme.Colors.OnSurface.Variant, light: "#8B8494", dark: "#C9C5D0")
                .Add(Uno.Themes.Markup.Theme.Colors.OnSurface.Inverse, light: "#F4EFF4", dark: "#1C1B1F")
                .Add(Uno.Themes.Markup.Theme.Colors.Outline.Default, light: "#79747E", dark: "#928F99")
                .Add(Uno.Themes.Markup.Theme.Colors.Outline.Variant, light: "#C9C5D0", dark: "#57545D")
        );
    }
}
