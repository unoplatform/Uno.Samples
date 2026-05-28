using SkiaSharp;

namespace Liveline.Helpers;

public static class ColorHelper
{
    private static readonly SKColor DefaultColor = new(76, 175, 80);
    private static readonly SKColor MomentumUp = new(76, 175, 80);
    private static readonly SKColor MomentumDown = new(244, 67, 54);
    private static readonly SKColor MomentumFlat = new(158, 158, 158);

    public static SKColor ParseHex(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return DefaultColor;

        var span = hex.AsSpan();
        if (span[0] == '#') span = span[1..];

        if (span.Length == 6 &&
            byte.TryParse(span[..2], System.Globalization.NumberStyles.HexNumber, null, out var r) &&
            byte.TryParse(span.Slice(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var g) &&
            byte.TryParse(span.Slice(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var b))
        {
            return new SKColor(r, g, b);
        }

        if (span.Length == 8 &&
            byte.TryParse(span[..2], System.Globalization.NumberStyles.HexNumber, null, out var a2) &&
            byte.TryParse(span.Slice(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var r2) &&
            byte.TryParse(span.Slice(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var g2) &&
            byte.TryParse(span.Slice(6, 2), System.Globalization.NumberStyles.HexNumber, null, out var b2))
        {
            return new SKColor(r2, g2, b2, a2);
        }

        return DefaultColor;
    }

    public static SKColor WithAlpha(SKColor color, byte alpha)
        => new(color.Red, color.Green, color.Blue, alpha);

    public static ChartPalette DerivePalette(string hexColor, bool isDark)
    {
        var baseColor = ParseHex(hexColor);
        float luminance = (baseColor.Red * 0.299f + baseColor.Green * 0.587f + baseColor.Blue * 0.114f) / 255f;

        SKColor background, gridLine, textColor, textDim;
        if (isDark)
        {
            background = new SKColor(17, 17, 17);
            gridLine = new SKColor(255, 255, 255, 25);
            textColor = new SKColor(255, 255, 255, 200);
            textDim = new SKColor(255, 255, 255, 100);
        }
        else
        {
            background = new SKColor(255, 255, 255);
            gridLine = new SKColor(0, 0, 0, 25);
            textColor = new SKColor(0, 0, 0, 200);
            textDim = new SKColor(0, 0, 0, 100);
        }

        var badgeText = luminance > 0.55f ? new SKColor(0, 0, 0, 230) : new SKColor(255, 255, 255, 240);

        return new ChartPalette(
            baseColor,
            WithAlpha(baseColor, 80),
            WithAlpha(baseColor, 0),
            background, gridLine, textColor, textDim,
            baseColor, badgeText,
            MomentumUp, MomentumDown, MomentumFlat);
    }
}

public record ChartPalette(
    SKColor LineColor,
    SKColor FillTop,
    SKColor FillBottom,
    SKColor Background,
    SKColor GridLine,
    SKColor TextColor,
    SKColor TextDim,
    SKColor BadgeBg,
    SKColor BadgeText,
    SKColor MomentumUp,
    SKColor MomentumDown,
    SKColor MomentumFlat);
