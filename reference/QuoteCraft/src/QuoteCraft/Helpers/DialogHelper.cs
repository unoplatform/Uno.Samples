namespace QuoteCraft.Helpers;

/// <summary>
/// Builds styled ContentDialogs with gradient banner headers
/// matching the "Unlock QuoteCraft Pro" banner style.
/// </summary>
public static class DialogHelper
{
    /// <summary>
    /// Builds a gradient banner Border (slate blue gradient, amber icon, white text).
    /// </summary>
    public static Border BuildGradientBanner(string iconGlyph, string title, string subtitle)
    {
        var headerTitle = new TextBlock
        {
            Text = title,
            Style = (Style)Application.Current.Resources["TitleMedium"],
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AppBarForegroundBrush"],
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
        };
        var headerSubtitle = new TextBlock
        {
            Text = subtitle,
            Style = (Style)Application.Current.Resources["BodySmall"],
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["UpgradeBannerSubtitleBrush"],
            TextWrapping = TextWrapping.Wrap,
        };
        var textStack = new StackPanel { Spacing = 2 };
        textStack.Children.Add(headerTitle);
        textStack.Children.Add(headerSubtitle);

        var icon = new FontIcon
        {
            Glyph = iconGlyph,
            FontSize = 20,
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AppBarAccentBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0),
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetColumn(icon, 0);
        Grid.SetColumn(textStack, 1);
        grid.Children.Add(icon);
        grid.Children.Add(textStack);

        return new Border
        {
            Background = BuildGradientBrush(),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(20, 16, 20, 16),
            Child = grid,
        };
    }

    /// <summary>
    /// Builds a simple ContentDialog with only a gradient banner (no form fields).
    /// </summary>
    public static ContentDialog BuildBannerDialog(
        XamlRoot xamlRoot,
        string iconGlyph,
        string title,
        string subtitle,
        string? closeButtonText = null,
        string? primaryButtonText = null)
    {
        var banner = BuildGradientBanner(iconGlyph, title, subtitle);
        var panel = new StackPanel { Spacing = 0, MinWidth = 380 };
        panel.Children.Add(banner);

        return new ContentDialog
        {
            Content = panel,
            CloseButtonText = closeButtonText ?? "OK",
            PrimaryButtonText = primaryButtonText,
            XamlRoot = xamlRoot,
        };
    }

    /// <summary>
    /// Builds a ContentDialog with a gradient banner header and custom body content.
    /// </summary>
    public static ContentDialog BuildBannerDialogWithContent(
        XamlRoot xamlRoot,
        string iconGlyph,
        string title,
        string subtitle,
        UIElement bodyContent,
        string? primaryButtonText = null,
        string? closeButtonText = null)
    {
        var banner = BuildGradientBanner(iconGlyph, title, subtitle);
        banner.Margin = new Thickness(0, 0, 0, 4);

        var panel = new StackPanel { Spacing = 16, MinWidth = 380 };
        panel.Children.Add(banner);
        panel.Children.Add(bodyContent);

        return new ContentDialog
        {
            Content = panel,
            PrimaryButtonText = primaryButtonText ?? "Save",
            CloseButtonText = closeButtonText ?? "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = xamlRoot,
        };
    }

    /// <summary>
    /// Builds the gradient brush matching the Pro banner.
    /// </summary>
    public static Microsoft.UI.Xaml.Media.LinearGradientBrush BuildGradientBrush()
    {
        var brush = new Microsoft.UI.Xaml.Media.LinearGradientBrush
        {
            StartPoint = new Windows.Foundation.Point(0, 0),
            EndPoint = new Windows.Foundation.Point(1, 0.5),
        };
        brush.GradientStops.Add(new Microsoft.UI.Xaml.Media.GradientStop
        {
            Color = (Windows.UI.Color)Application.Current.Resources["UpgradeBannerStartColor"],
            Offset = 0,
        });
        brush.GradientStops.Add(new Microsoft.UI.Xaml.Media.GradientStop
        {
            Color = (Windows.UI.Color)Application.Current.Resources["UpgradeBannerEndColor"],
            Offset = 1,
        });
        return brush;
    }
}
