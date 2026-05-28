using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace EnterpriseDashboard.Observatory.Views;

public sealed partial class CohortsPage : Page, IObservatorySection
{
    // Retention triangle: % of each signup-month cohort still active N months later.
    private static readonly (string Cohort, double[] Retention)[] Triangle =
    {
        ("JUL", new[] { 100.0, 82, 71, 64, 58, 54, 51 }),
        ("AUG", new[] { 100.0, 80, 69, 62, 56, 52 }),
        ("SEP", new[] { 100.0, 83, 73, 66, 60 }),
        ("OCT", new[] { 100.0, 79, 68, 61 }),
        ("NOV", new[] { 100.0, 81, 70 }),
        ("DEC", new[] { 100.0, 84 }),
    };

    public CohortsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => BuildTriangle();
        ActualThemeChanged += (_, _) => BuildTriangle();
    }

    // Navigation system resolves CohortsViewModel from DI and assigns DataContext.
    public void Refresh()
    {
        if (DataContext is ObservatoryViewModel vm) vm.ResetKey++;
    }

    // Brightness-encoded cohort grid (same language as the usage heatmap): higher
    // retention reads brighter in dark theme, darker ink in light theme.
    private void BuildTriangle()
    {
        TriangleHost.Children.Clear();
        TriangleHost.ColumnDefinitions.Clear();
        TriangleHost.RowDefinitions.Clear();

        bool isLight = ActualTheme == ElementTheme.Light;
        var labelStyle = (Style)Application.Current.Resources["ObsListMeta"];
        var mono = (FontFamily)Application.Current.Resources["ObsMonoFamily"];

        TriangleHost.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(44) });
        for (int c = 0; c < 7; c++)
            TriangleHost.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        TriangleHost.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int r = 0; r < Triangle.Length; r++)
            TriangleHost.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        for (int m = 0; m < 7; m++)
        {
            var h = new TextBlock { Text = "M" + m, Style = labelStyle, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 6) };
            Grid.SetRow(h, 0);
            Grid.SetColumn(h, m + 1);
            TriangleHost.Children.Add(h);
        }

        for (int r = 0; r < Triangle.Length; r++)
        {
            var (cohort, vals) = Triangle[r];
            var lbl = new TextBlock { Text = cohort, Style = labelStyle, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(lbl, r + 1);
            Grid.SetColumn(lbl, 0);
            TriangleHost.Children.Add(lbl);

            for (int m = 0; m < vals.Length; m++)
            {
                double v = vals[m];
                byte lum = isLight
                    ? (byte)Math.Clamp(228 - v * 1.7, 28, 228)
                    : (byte)Math.Clamp(26 + v * 1.7, 26, 210);
                byte txt = lum > 128 ? (byte)0x16 : (byte)0xC8;
                var cell = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, lum, lum, lum)),
                    Margin = new Thickness(1.5),
                    CornerRadius = new CornerRadius(2),
                    Child = new TextBlock
                    {
                        Text = ((int)Math.Round(v)).ToString(),
                        FontFamily = mono,
                        FontSize = 9,
                        Foreground = new SolidColorBrush(Color.FromArgb(255, txt, txt, txt)),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                Grid.SetRow(cell, r + 1);
                Grid.SetColumn(cell, m + 1);
                TriangleHost.Children.Add(cell);
            }
        }
    }
}
