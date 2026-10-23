using Windows.UI;

namespace SmoothScrolling;

public sealed record FeedItem(int Index, string Title, string Subtitle, string Badge, string Meta, Symbol Icon, Brush Thumb)
{
    private static readonly string[] Subjects =
    {
        "Harbor lights", "Quiet mornings", "Granite ridge", "Paper lanterns", "Night market", "Tidal pools",
        "Copper roofs", "Winter orchard", "Salt flats", "Glass atrium", "Cedar trail", "Velvet dusk",
    };

    private static readonly string[] Places =
    {
        "Lisbon", "Kyoto", "Bergen", "Valparaiso", "Tbilisi", "Hobart", "Ghent", "Oaxaca", "Tallinn", "Cusco",
    };

    private static readonly string[] Badges = { "New", "Popular", "Staff pick", "Archive", "Trending" };

    private static readonly Symbol[] Icons =
    {
        Symbol.Camera, Symbol.Pictures, Symbol.Map, Symbol.Audio, Symbol.Video, Symbol.Favorite,
    };

    public static List<FeedItem> Generate(int count)
    {
        List<FeedItem> items = new(count);
        for (var i = 0; i < count; i++)
        {
            var hue = (i * 37) % 360;
            LinearGradientBrush brush = new() { StartPoint = new(0, 0), EndPoint = new(1, 1) };
            brush.GradientStops.Add(new GradientStop { Color = FromHsl(hue, 0.62, 0.52), Offset = 0 });
            brush.GradientStops.Add(new GradientStop { Color = FromHsl((hue + 40) % 360, 0.68, 0.38), Offset = 1 });

            items.Add(new FeedItem(
                i + 1,
                $"{Subjects[i % Subjects.Length]} No. {i + 1:N0}",
                $"Collection from {Places[(i * 7) % Places.Length]} - {(i % 28) + 1} photos",
                Badges[(i * 3) % Badges.Length],
                $"{(i % 9) + 1}.{(i * 7) % 10} MB  |  {(i % 23) + 2} min ago",
                Icons[i % Icons.Length],
                brush));
        }

        return items;
    }

    private static Color FromHsl(double h, double s, double l)
    {
        var c = (1 - Math.Abs(2 * l - 1)) * s;
        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        var m = l - c / 2;
        (double r, double g, double b) = h switch
        {
            < 60 => (c, x, 0d),
            < 120 => (x, c, 0d),
            < 180 => (0d, c, x),
            < 240 => (0d, x, c),
            < 300 => (x, 0d, c),
            _ => (c, 0d, x),
        };
        return Color.FromArgb(255, (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
    }
}
