using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace EnterpriseDashboard.Observatory.Helpers;

public static class SplineBuilder
{
    // Catmull-Rom -> cubic Bezier path. Tension 0.5 = standard Catmull-Rom.
    public static PathGeometry Build(IReadOnlyList<Point> points, double tension = 0.5)
    {
        var geo = new PathGeometry();
        if (points.Count < 2) return geo;

        var fig = new PathFigure { StartPoint = points[0], IsClosed = false, IsFilled = false };

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p0 = i == 0 ? points[0] : points[i - 1];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Count ? points[i + 2] : p2;

            var c1 = new Point(
                p1.X + (p2.X - p0.X) * tension / 3.0,
                p1.Y + (p2.Y - p0.Y) * tension / 3.0);
            var c2 = new Point(
                p2.X - (p3.X - p1.X) * tension / 3.0,
                p2.Y - (p3.Y - p1.Y) * tension / 3.0);

            fig.Segments.Add(new BezierSegment { Point1 = c1, Point2 = c2, Point3 = p2 });
        }

        geo.Figures.Add(fig);
        return geo;
    }
}
