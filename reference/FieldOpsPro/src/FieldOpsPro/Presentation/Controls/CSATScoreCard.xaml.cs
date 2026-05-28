using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class CSATScoreCard : UserControl
{
    public CSATScoreCard()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ScoreProperty =
        DependencyProperty.Register(nameof(Score), typeof(double), typeof(CSATScoreCard),
            new PropertyMetadata(4.8));

    public double Score
    {
        get => (double)GetValue(ScoreProperty);
        set => SetValue(ScoreProperty, value);
    }

    /// <summary>Formats Score as one decimal place for the headline.</summary>
    public string FormatScore(double score) => score.ToString("F1", CultureInfo.CurrentCulture);

    /// <summary>Picks the glyph for the star at <paramref name="position"/> (1..5) given the score.</summary>
    public string StarGlyph(double score, int position)
    {
        var full = (int)System.Math.Floor(score);
        var hasHalf = score - full >= 0.5;

        if (position <= full)
            return ((char)0xE735).ToString();   // Full star
        if (position == full + 1 && hasHalf)
            return ((char)0xE7C6).ToString();   // Half star
        return ((char)0xE734).ToString();       // Empty star
    }

    /// <summary>Picks the foreground brush for the star at <paramref name="position"/> given the score.</summary>
    public Brush StarForeground(double score, int position)
    {
        var full = (int)System.Math.Floor(score);
        var hasHalf = score - full >= 0.5;

        if (position <= full)
            return Utils.ColorUtils.GetBrush("AccentPrimaryBrush");
        if (position == full + 1 && hasHalf)
            return Utils.ColorUtils.GetBrush("AccentSecondaryBrush");
        return Utils.ColorUtils.GetBrush("TextMutedBrush");
    }
}
