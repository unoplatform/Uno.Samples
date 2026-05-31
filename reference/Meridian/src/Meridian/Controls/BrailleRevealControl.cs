using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace Meridian.Controls;

/// <summary>
/// Animates text from scrambled braille → decoded real characters → subtle ambient flicker.
/// 3 phases: Idle (all braille), Decode (per-char resolve L→R), Resolved (ambient flicker).
/// </summary>
public sealed class BrailleRevealControl : Microsoft.UI.Xaml.Controls.Control
{
    private static readonly char[] BraillePool = ['⠀', '⣀', '⣤', '⣴', '⣶', '⣷', '⣿', '⣶', '⣴', '⣤', '⣀', '⠋', '⠙', '⠹'];
    private static readonly int[] Seeds = [7, 13, 23, 37, 41, 53, 61, 71, 83, 97, 3, 11, 19, 29, 43, 59];

    private TextBlock? _textBlock;
    private DispatcherTimer? _timer;
    private string _targetText = "";
    private int _frame;
    private int _scrambleCycles = 5; // Ticks per character to resolve
    private Phase _phase = Phase.Idle;
    private int _delayTicksRemaining;

    // Theme brushes (lazy from resources)
    private static SolidColorBrush? _accentBrush;
    private static SolidColorBrush? _subtleBrush;
    private static SolidColorBrush AccentBrush => _accentBrush ??= (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
    private static SolidColorBrush SubtleBrush => _subtleBrush ??= (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"];

    private enum Phase { Idle, Decode, Resolved }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string),
            typeof(BrailleRevealControl), new PropertyMetadata("", OnTextChanged));

    public static readonly DependencyProperty DelayMsProperty =
        DependencyProperty.Register(nameof(DelayMs), typeof(int),
            typeof(BrailleRevealControl), new PropertyMetadata(800));

    public static readonly DependencyProperty StaggerMsProperty =
        DependencyProperty.Register(nameof(StaggerMs), typeof(int),
            typeof(BrailleRevealControl), new PropertyMetadata(50));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public int DelayMs { get => (int)GetValue(DelayMsProperty); set => SetValue(DelayMsProperty, value); }
    public int StaggerMs { get => (int)GetValue(StaggerMsProperty); set => SetValue(StaggerMsProperty, value); }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BrailleRevealControl c) c.StartAnimation();
    }

    public BrailleRevealControl() => DefaultStyleKey = typeof(BrailleRevealControl);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _textBlock = GetTemplateChild("RevealText") as TextBlock;
        if (!string.IsNullOrEmpty(Text)) StartAnimation();
    }

    private void StartAnimation()
    {
        _targetText = Text ?? "";
        if (_textBlock == null || _targetText.Length == 0) return;

        _timer?.Stop();
        _frame = 0;
        _phase = Phase.Idle;
        _delayTicksRemaining = Math.Max(1, DelayMs / Math.Max(1, StaggerMs));

        // Show initial scrambled state
        RenderFrame();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(StaggerMs) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnTick(object? sender, object e)
    {
        _frame++;

        switch (_phase)
        {
            case Phase.Idle:
                _delayTicksRemaining--;
                if (_delayTicksRemaining <= 0)
                    _phase = Phase.Decode;
                RenderFrame();
                break;

            case Phase.Decode:
                RenderFrame();
                // Check if all characters resolved
                var totalFrames = _targetText.Length * _scrambleCycles + _scrambleCycles;
                if (_frame >= totalFrames + _delayTicksRemaining)
                {
                    _phase = Phase.Resolved;
                    _frame = 0; // Reset for flicker phase
                }
                break;

            case Phase.Resolved:
                RenderFlicker();
                // Loop: restart full cycle after ~30s of ambient flicker
                // At 50ms/tick, 30s = 600 ticks
                if (_frame >= 600)
                    StartAnimation();
                break;
        }
    }

    private void RenderFrame()
    {
        if (_textBlock == null) return;

        _textBlock.Inlines.Clear();

        for (int i = 0; i < _targetText.Length; i++)
        {
            var ch = _targetText[i];

            if (ch == ' ')
            {
                _textBlock.Inlines.Add(new Run { Text = "\u2003" }); // em-space
                continue;
            }

            var resolveFrame = i * _scrambleCycles + _scrambleCycles;
            // In idle phase, nothing is resolved
            var isResolved = _phase == Phase.Decode && (_frame - (DelayMs / Math.Max(1, StaggerMs))) >= resolveFrame;

            if (_phase == Phase.Idle || !isResolved)
            {
                // Scrambled: deterministic pseudo-random braille
                var seed = i < Seeds.Length ? Seeds[i] : i * 7;
                var idx = (seed + _frame * 3 + i * 7) % BraillePool.Length;
                var braille = BraillePool[idx];

                // Opacity ramps from 0.3 → 0.8 as we approach resolve
                double opacity = 0.3;
                if (_phase == Phase.Decode)
                {
                    var progress = Math.Min(1.0, (double)(_frame - (DelayMs / Math.Max(1, StaggerMs))) / resolveFrame);
                    opacity = 0.3 + 0.5 * Math.Max(0, progress);
                }

                _textBlock.Inlines.Add(new Run
                {
                    Text = braille.ToString(),
                    Foreground = AccentBrush,
                    FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
                    CharacterSpacing = CharacterSpacing,
                });
                // Set opacity via the run's foreground opacity
                if (_textBlock.Inlines[^1] is Run r)
                {
                    var brush = AccentBrush.Color;
                    r.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(
                        (byte)(opacity * 255), brush.R, brush.G, brush.B));
                }
            }
            else
            {
                // Resolved: real character, subtle color, sans font
                _textBlock.Inlines.Add(new Run
                {
                    Text = ch.ToString(),
                    Foreground = SubtleBrush,
                    FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
                    CharacterSpacing = CharacterSpacing,
                });
            }
        }
    }

    private void RenderFlicker()
    {
        if (_textBlock == null) return;

        _textBlock.Inlines.Clear();

        for (int i = 0; i < _targetText.Length; i++)
        {
            var ch = _targetText[i];
            if (ch == ' ')
            {
                _textBlock.Inlines.Add(new Run { Text = "\u2003" });
                continue;
            }

            // Staggered flicker: each char offset by i * 3 frames
            var flickerPhase = (_frame + i * 3) % 40; // 40-frame cycle (~2s at 50ms)
            var isFlickering = flickerPhase >= 13 && flickerPhase <= 16; // ~35% of cycle

            var brush = isFlickering ? AccentBrush : SubtleBrush;
            var opacity = isFlickering ? 0.5 : (flickerPhase < 5 ? 0.55 : 0.85); // Subtle breathing

            var color = brush.Color;
            _textBlock.Inlines.Add(new Run
            {
                Text = ch.ToString(),
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(
                    (byte)(opacity * 255), color.R, color.G, color.B)),
                FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
                CharacterSpacing = CharacterSpacing,
            });
        }
    }
}
