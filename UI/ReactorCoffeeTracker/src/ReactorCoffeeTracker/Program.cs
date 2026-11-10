using Microsoft.UI.Reactor;
using Microsoft.UI.Reactor.Core;
using Microsoft.UI.Xaml;
using static Microsoft.UI.Reactor.Factories;

ReactorApp.Run<CoffeeApp>("Coffee Tracker", width: 520, height: 530);

class CoffeeApp : Component
{
    static readonly string[] Days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    public override Element Render()
    {
        var (cups, setCups) = UseState(new[] { 2, 3, 1, 4, 2, 3, 1 });
        var (limit, setLimit) = UseState(3.0);
        var today = cups[^1];

        return VStack(16,
            Heading("☕ Coffee Tracker"),
            TextBlock($"{cups.Sum()} cups this week · {today} today"),

            Card(HStack(14, cups.Select((c, i) => VStack(6,
                Caption($"{c}").HAlign(HorizontalAlignment.Center),
                Border(null).Width(40).Height(c * 28).CornerRadius(8)
                    .Background(c > limit ? "#E5484D" : "#A0693A"),
                Caption(Days[i]).HAlign(HorizontalAlignment.Center)
            ).VAlign(VerticalAlignment.Bottom)).ToArray()).HAlign(HorizontalAlignment.Center)),

            TextBlock($"Daily limit: {limit:0} cups"),
            Slider(limit, 1, 8, setLimit),
            TextBlock(today > limit ? "🚨 Easy there, champ." : "✅ You're on track."),

            HStack(8,
                Button("☕ One more", () => setCups([.. cups[..^1], today + 1])),
                Button("Reset today", () => setCups([.. cups[..^1], 0]))
            )
        ).Padding(24);
    }
}
