using SkiaSharp;

namespace QuoteCraft.Services;

/// <summary>
/// The printed quote's colours, used by <see cref="QuotePdf"/> and nothing else. A PDF is paper: it prints the same
/// whatever theme the app shows, so these are print constants, not theme resources, and they do not follow Dark mode
/// or the palette. They are the one place outside Styles/ColorPaletteOverride.xaml and App.xaml that names a colour.
/// </summary>
public static class PrintColors
{
    /// <summary>The page.</summary>
    public static readonly SKColor Paper = SKColors.White;

    /// <summary>Body text: the title, the client, line items, totals and notes.</summary>
    public static readonly SKColor Ink = SKColors.Black;

    /// <summary>The business name, the QUOTE heading, the table's header row and the total.</summary>
    public static readonly SKColor Brand = SKColor.Parse("#1565C0");

    /// <summary>Text on <see cref="Brand"/>: the table's column names.</summary>
    public static readonly SKColor OnBrand = SKColors.White;

    /// <summary>The header's rule and the photo frames.</summary>
    public static readonly SKColor Rule = SKColor.Parse("#E0E0E0");

    /// <summary>Every other line-item row.</summary>
    public static readonly SKColor Stripe = SKColor.Parse("#F5F5F5");

    /// <summary>The footer's lines.</summary>
    public static readonly SKColor Muted = SKColor.Parse("#757575");

    /// <summary>The generated-on line.</summary>
    public static readonly SKColor Faint = SKColor.Parse("#BDBDBD");
}
