using System.Text.Json;
using DestinationRolodex.Models;

namespace DestinationRolodex.Tests;

public class CircularMathTests
{
    [TestCase(-1.5, -1)]
    [TestCase(-0.5, 0)]
    [TestCase(0.5, 1)]
    [TestCase(1.5, 2)]
    [TestCase(-0.51, -1)]
    [TestCase(2.49, 2)]
    public void RoundLikeJavaScript_matches_math_round(double input, long expected)
        => CircularMath.RoundLikeJavaScript(input).Should().Be(expected);

    [TestCase(-1, 6, 5)]
    [TestCase(6, 6, 0)]
    [TestCase(-7, 6, 5)]
    [TestCase(2.5, 6, 3)]
    [TestCase(-0.5, 6, 0)]
    public void SelectedIndex_uses_positive_modulo(double position, int count, int expected)
        => CircularMath.SelectedIndex(position, count).Should().Be(expected);

    [TestCase(0, 5, 6, -1)]
    [TestCase(5, 0, 6, 1)]
    [TestCase(0, 3, 6, 3)]
    [TestCase(3, 0, 6, 3)]
    [TestCase(1, 1, 6, 0)]
    public void ShortestDelta_ties_resolve_forward(int from, int to, int count, int expected)
        => CircularMath.ShortestDelta(from, to, count).Should().Be(expected);

    [Test]
    public void Pose_matches_prototype_formulas()
    {
        var p = CircularMath.Pose(1, 80);
        p.Y.Should().BeApproximately(80 * (1 - .035), 1e-9);
        p.Z.Should().Be(-13);
        p.RotationX.Should().Be(-12);
        p.Scale.Should().BeApproximately(1 - .044, 1e-9);
        p.Opacity.Should().BeApproximately(.5, 1e-9);
        p.Visible.Should().BeTrue();

        var far = CircularMath.Pose(-3, 80);
        far.Visible.Should().BeFalse();
        far.Opacity.Should().Be(0);
        CircularMath.Pose(2, 80).Opacity.Should().BeApproximately(.16, 1e-9);
        CircularMath.Pose(-1.5, 80).Opacity.Should().BeApproximately(.5 - .34 * .5, 1e-9);
        CircularMath.Pose(10, 80).Scale.Should().Be(.65);
    }

    [Test]
    public void Handoff_test_vectors_pass()
    {
        var path = FindKitFile(Path.Combine("qa", "math-test-vectors.json"));
        if (path is null)
        {
            Assert.Inconclusive("Handoff kit qa/math-test-vectors.json not found next to the solution.");
            return;
        }

        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        foreach (var v in doc.RootElement.GetProperty("rounding").EnumerateArray())
        {
            CircularMath.RoundLikeJavaScript(v.GetProperty("input").GetDouble()).Should().Be(v.GetProperty("expected").GetInt64());
        }

        foreach (var v in doc.RootElement.GetProperty("selectedIndex").EnumerateArray())
        {
            CircularMath.SelectedIndex(v.GetProperty("position").GetDouble(), v.GetProperty("count").GetInt32()).Should().Be(v.GetProperty("expected").GetInt32());
        }

        foreach (var v in doc.RootElement.GetProperty("shortestDelta").EnumerateArray())
        {
            CircularMath.ShortestDelta(v.GetProperty("from").GetInt32(), v.GetProperty("to").GetInt32(), v.GetProperty("count").GetInt32()).Should().Be(v.GetProperty("expected").GetInt32());
        }
    }

    internal static string? FindKitFile(string relative)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, relative);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        return null;
    }
}
