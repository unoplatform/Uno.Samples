using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI.ViewManagement;

namespace EnterpriseDashboard.Observatory.Animation;

// Common scaffolding for chart UserControls.
//
// Lifecycle contract — reveal once, then HOLD visible:
//   - BuildScene renders the chart's shapes in their pre-reveal (collapsed) state.
//   - CreateLoopStoryboard returns a 0 -> peak -> hold (-> 0) timeline. The base
//     converts it into a ONE-SHOT reveal that ends holding the peak, so the chart
//     animates in and then stays fully rendered.
//   - Visibility is never coupled to a running animation. Once a chart has revealed,
//     any later rebuild (theme toggle / late Loaded) snaps straight back to the final
//     visible state via ShowFinal — it can never blank out.
//
// This fixes the prior race where Loaded/SizeChanged rebuilt shapes and stopped or
// orphaned the storyboard the cascade had begun, stranding charts at Opacity = 0.
// (Resize no longer rebuilds at all — the Viewbox scales the fixed-size Surface.)
public abstract class AnimatedChartBase : UserControl, IAnimatableChart
{
    private bool _built;
    private bool _revealed;   // has the entrance played at least once?
    private bool _isPlaying;
    private Storyboard? _reveal;

    // System reduced-motion flag, read live each Play(); the instance is COM-backed so
    // it's allocated once and reused.
    private static readonly UISettings _uiSettings = new();

    protected AnimatedChartBase()
    {
        // The scene's geometry comes from the fixed Surface size inside a Viewbox, so it
        // never depends on the control's layout size — no rebuild on SizeChanged (the
        // Viewbox scales the held result). Rebuild only on data change, theme, or load.
        Loaded += (_, _) => OnTreeChanged();
        ActualThemeChanged += (_, _) => OnTreeChanged();
    }

    public bool IsPlaying => _isPlaying;

    // Called by data-bound property changes and tree lifecycle events. Rebuilds the
    // scene for the current size/theme/data; if the entrance already played, snaps
    // back to the held final state so the chart stays visible.
    protected void EnsureBuilt() => OnTreeChanged();

    private void OnTreeChanged()
    {
        _built = true;
        BuildScene();
        if (_revealed) ShowFinal();
    }

    // Cascade entry point — play the one-shot entrance reveal.
    public void Play()
    {
        _built = true;
        BuildScene();
        _revealed = true;

        if (!_uiSettings.AnimationsEnabled) { ShowFinal(); return; } // reduced motion: show, don't animate

        StopReveal();
        _reveal = BuildRevealStoryboard();
        if (_reveal == null) { _isPlaying = false; return; }
        _reveal.Begin();
        _isPlaying = true;
    }

    // Pre-reveal state — collapsed and waiting for this chart's cascade slot.
    public void Reset()
    {
        _isPlaying = false;
        _revealed = false;
        StopReveal();
        if (_built) BuildScene();
    }

    // Snap to the held final state with no visible animation.
    private void ShowFinal()
    {
        StopReveal();
        _reveal = BuildRevealStoryboard();
        if (_reveal == null) return;
        _reveal.Begin();
        _reveal.SkipToFill();   // jump to the held peak (final, visible)
        _isPlaying = false;
    }

    // Convert the chart's perpetual loop into a one-shot reveal that holds at its
    // peak: run once, and drop the trailing "return to start" keyframe each
    // KeyFramesLoop appends for cycling.
    private Storyboard? BuildRevealStoryboard()
    {
        var sb = CreateLoopStoryboard();
        if (sb == null) return null;
        sb.RepeatBehavior = new RepeatBehavior(1);
        foreach (var child in sb.Children)
        {
            if (child is DoubleAnimationUsingKeyFrames kf && kf.KeyFrames.Count >= 3)
                kf.KeyFrames.RemoveAt(kf.KeyFrames.Count - 1);
        }
        return sb;
    }

    private void StopReveal()
    {
        if (_reveal != null) { _reveal.Stop(); _reveal = null; }
    }

    protected abstract void BuildScene();

    // Override to provide the chart's 0 -> peak entrance timeline, expressed as a
    // perpetual loop. The base converts it to a one-shot reveal-and-hold.
    protected virtual Storyboard? CreateLoopStoryboard() => null;
}
