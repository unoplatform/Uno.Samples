using DestinationRolodex.Models;
using DestinationRolodex.Rendering;

namespace DestinationRolodex.Tests;

public class GlobeTests
{
    private static double Deg(double rad) => rad * 180 / Math.PI;

    [Test]
    public void Unit_vector_round_trips_through_lat_lon()
    {
        foreach (var (lat, lon) in new[] { (11.98, 120.20), (35.36, 138.73), (-44.67, 167.93), (63.53, -19.51), (0, 0), (89.9, 10) })
        {
            var v = GlobeRenderer.ToUnitVector(new GeoPoint(lat, lon));
            var (la, lo) = GlobeRenderer.ToLatLon(v);
            Deg(la).Should().BeApproximately(lat, 1e-6);
            Deg(lo).Should().BeApproximately(lon, 1e-6);
        }
    }

    [Test]
    public void Slerp_crosses_the_date_line_the_short_way()
    {
        var a = GlobeRenderer.ToUnitVector(new GeoPoint(0, 170));
        var b = GlobeRenderer.ToUnitVector(new GeoPoint(0, -170));
        var mid = GlobeRenderer.ToLatLon(GlobeRenderer.Slerp(a, b, .5));
        Math.Abs(Deg(mid.Lon)).Should().BeApproximately(180, 1e-6);
        Deg(mid.Lat).Should().BeApproximately(0, 1e-6);
    }

    [Test]
    public void Slerp_endpoints_and_identical_inputs_are_exact()
    {
        var a = GlobeRenderer.ToUnitVector(new GeoPoint(46.69, 12.09));
        var b = GlobeRenderer.ToUnitVector(new GeoPoint(68.21, 13.61));
        GlobeRenderer.Slerp(a, b, 0).Should().Be(a);
        var end = GlobeRenderer.Slerp(a, b, 1);
        end.X.Should().BeApproximately(b.X, 1e-9);
        end.Z.Should().BeApproximately(b.Z, 1e-9);
        GlobeRenderer.Slerp(a, a, .37).Should().Be(a);
    }

    [Test]
    public void Antipodal_pairs_travel_deterministically_without_nan()
    {
        var a = GlobeRenderer.ToUnitVector(new GeoPoint(10, 20));
        var b = GlobeRenderer.ToUnitVector(new GeoPoint(-10, -160));
        foreach (var t in new[] { 0, .25, .5, .75, 1 })
        {
            var v = GlobeRenderer.Slerp(a, b, t);
            double.IsFinite(v.X).Should().BeTrue();
            (v.X * v.X + v.Y * v.Y + v.Z * v.Z).Should().BeApproximately(1, 1e-9);
        }

        var first = GlobeRenderer.Slerp(a, b, .5);
        var second = GlobeRenderer.Slerp(a, b, .5);
        first.Should().Be(second);
    }

    [Test]
    public void Poles_project_and_interpolate()
    {
        var north = GlobeRenderer.ToUnitVector(new GeoPoint(90, 0));
        var equator = GlobeRenderer.ToUnitVector(new GeoPoint(0, 0));
        var mid = GlobeRenderer.ToLatLon(GlobeRenderer.Slerp(north, equator, .5));
        Deg(mid.Lat).Should().BeApproximately(45, 1e-6);
    }

    [Test]
    public void Invalid_coordinates_are_rejected_by_the_model()
    {
        new GeoPoint(91, 0).IsValid.Should().BeFalse();
        new GeoPoint(0, 181).IsValid.Should().BeFalse();
        new GeoPoint(double.NaN, 0).IsValid.Should().BeFalse();
        new GeoPoint(-44.67, 167.93).IsValid.Should().BeTrue();
    }

    [Test]
    public void Renderer_draws_a_neutral_globe_without_a_mask_or_orientation()
    {
        using var renderer = new GlobeRenderer(null);
        using var surface = SkiaSharp.SKSurface.Create(new SkiaSharp.SKImageInfo(40, 40));
        renderer.Render(surface.Canvas, new SkiaSharp.SKRect(0, 0, 40, 40), null);
        renderer.Render(surface.Canvas, new SkiaSharp.SKRect(0, 0, 40, 40), GlobeRenderer.ToUnitVector(new GeoPoint(35.36, 138.73)));
        using var snap = surface.Snapshot();
        snap.Width.Should().Be(40);
    }
}
