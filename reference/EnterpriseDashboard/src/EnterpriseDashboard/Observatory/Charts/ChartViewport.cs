using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using EnterpriseDashboard.Observatory.Animation;

namespace EnterpriseDashboard.Observatory.Charts;

// Shared Canvas-in-Viewbox scaffold. Brief §5.1 sizes:
//  - Single-column chart: 360 × 200
//  - Full-width chart:    740 × 220
public abstract class ChartViewport : AnimatedChartBase
{
    protected Canvas Surface { get; }
    protected double SurfaceWidth { get; }
    protected double SurfaceHeight { get; }

    protected ChartViewport(double width = 360, double height = 200)
    {
        SurfaceWidth = width;
        SurfaceHeight = height;
        Surface = new Canvas { Width = width, Height = height, Opacity = 0 };
        var viewbox = new Viewbox { Stretch = Stretch.Uniform, Child = Surface };
        Content = viewbox;
    }

    // Default loop: fade entire surface in → hold → out → repeat. Specific charts override for richer motion.
    protected override Storyboard? CreateLoopStoryboard()
    {
        return LoopAnimations.ScalarLoop(Surface, "Opacity", 0, 1, cycleSec: 5.5, inSec: 1.2);
    }
}
