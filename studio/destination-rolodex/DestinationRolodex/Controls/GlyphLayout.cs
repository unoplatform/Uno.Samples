using Microsoft.UI.Xaml.Media;

namespace DestinationRolodex.Controls;

/// <summary>
/// Letter-spaced text on the Uno Skia text stack. TextBlock.CharacterSpacing is a silent no-op there (runtime gotcha),
/// so tracked labels are laid out one glyph per TextBlock with a negative or positive margin. A whitespace-only
/// TextBlock measures zero, so spaces become an explicit-width spacer. Pair kerning is lost; recorded in SPEC.md.
/// </summary>
internal static class GlyphLayout
{
    /// <summary>Approximate Inter space advance in em (584/2048).</summary>
    private const double SpaceAdvanceEm = .285;

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<StackPanel, string> Keys = new();

    public static void Apply(StackPanel panel, string text, FontFamily font, double fontSize, double trackingPx, Brush foreground, double lineHeight = 0)
    {
        var key = $"{text}\u0001{fontSize:0.##}\u0001{trackingPx:0.##}";
        if (Keys.TryGetValue(panel, out var existing) && existing == key)
        {
            return;
        }

        Keys.AddOrUpdate(panel, key);
        panel.Orientation = Orientation.Horizontal;
        panel.Children.Clear();
        var first = true;
        foreach (var ch in text)
        {
            if (ch == ' ')
            {
                panel.Children.Add(new Border { Width = Math.Max(1, SpaceAdvanceEm * fontSize + trackingPx), Height = 1 });
                first = false;
                continue;
            }

            var tb = new TextBlock
            {
                Text = ch.ToString(),
                FontFamily = font,
                FontSize = fontSize,
                Foreground = foreground,
                Margin = first ? default : new Thickness(trackingPx, 0, 0, 0),
                IsTextSelectionEnabled = false,
                VerticalAlignment = VerticalAlignment.Center,
            };
            if (lineHeight > 0)
            {
                tb.LineHeight = lineHeight;
            }

            panel.Children.Add(tb);
            first = false;
        }
    }
}
