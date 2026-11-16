using Microsoft.UI.Xaml.Controls.Primitives;

namespace LottieBuiltIn;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        foreach (var card in new[] { LoaderCard, CheckCard, HeartCard })
        {
            card.SetAutomationIds();
        }
    }

    public static string FormatPercent(double value) => $"Determinate: {value:0}%";

    private void OnSpeedChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (LoaderCard is null || HeartCard is null)
        {
            return;
        }

        LoaderCard.PlaybackRate = e.NewValue;
        CheckCard.PlaybackRate = e.NewValue;
        HeartCard.PlaybackRate = e.NewValue;
    }
}
