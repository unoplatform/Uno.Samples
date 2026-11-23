using Microsoft.UI.Xaml.Media;

namespace DestinationRolodex.Controls;

// xaml-lint: allow codebehind - custom control implementation: marker metrics and the focus ring follow the card's frame loop.
/// <summary>
/// Focus and semantics host for the infinite country list plus the active-selection marks. The rows are rendered by
/// the card's Skia scene so photo-in-type and the solid base share one glyph layout.
/// </summary>
public sealed partial class CountryPicker : UserControl
{
    public const int RowPool = 9;

    private readonly SolidColorBrush _accent = new(Windows.UI.Color.FromArgb(255, 0xE5, 0xF5, 0xB6));

    public CountryPicker()
    {
        InitializeComponent();
        MarkerDot.Fill = _accent;
        GotFocus += (_, _) => UpdateFocusRing();
        LostFocus += (_, _) => UpdateFocusRing();
    }

    /// <summary>True in Look around: removed from focus and hit testing.</summary>
    public bool IsInert
    {
        get => !IsTabStop;
        set
        {
            IsTabStop = !value;
            IsHitTestVisible = !value;
        }
    }

    public void Configure(double windowHeight, bool compact)
    {
        Height = windowHeight;
        FocusRing.Margin = new Thickness(13, Math.Max(8, windowHeight / 2 - 44), 13, Math.Max(8, windowHeight / 2 - 44));
        MarkerLeft.Margin = new Thickness(compact ? 10 : 15, 0, 0, 0);
        MarkerDot.Width = MarkerDot.Height = compact ? 6 : 7;
        MarkerRight.Width = compact ? 9 : 12;
        MarkerRight.Margin = new Thickness(0, 0, compact ? 14 : 19, 0);
    }

    /// <summary>Material accent for the selection dot (authored color mix, updated per frame while in flight).</summary>
    public void SetAccent(Windows.UI.Color color)
    {
        if (_accent.Color != color)
        {
            _accent.Color = color;
        }
    }

    private void UpdateFocusRing() => FocusRing.Visibility = FocusState == FocusState.Keyboard ? Visibility.Visible : Visibility.Collapsed;
}
