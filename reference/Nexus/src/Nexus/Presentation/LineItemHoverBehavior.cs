using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace Nexus.Presentation;

/// <summary>
/// Attaches subtle lift hover effects to Border line items within a page.
/// Recursively finds row-style borders (top border only) inside StackPanels.
/// </summary>
public static class LineItemHoverBehavior
{
    private static readonly TimeSpan AnimDuration = TimeSpan.FromMilliseconds(150);

    /// <summary>
    /// Walks the visual tree and attaches hover to all line-item borders.
    /// Call this on Page.Loaded.
    /// </summary>
    public static void AttachToTree(DependencyObject root)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);

            if (child is StackPanel panel && panel.Spacing == 0)
            {
                foreach (var item in panel.Children)
                {
                    if (item is Border border &&
                        border.BorderThickness.Top > 0 &&
                        border.BorderThickness.Left == 0 &&
                        border.BorderThickness.Right == 0 &&
                        border.BorderThickness.Bottom == 0)
                    {
                        AttachToBorder(border);
                    }
                }
            }

            AttachToTree(child);
        }
    }

    private static void AttachToBorder(Border border)
    {
        var transform = new TranslateTransform();
        border.RenderTransform = transform;

        border.PointerEntered += (s, e) =>
        {
            AnimateY(transform, -2);
            border.Background = new SolidColorBrush(
                Application.Current.RequestedTheme == ApplicationTheme.Dark
                    ? Microsoft.UI.ColorHelper.FromArgb(20, 255, 255, 255)
                    : Microsoft.UI.ColorHelper.FromArgb(15, 0, 0, 0));
        };

        border.PointerExited += (s, e) =>
        {
            AnimateY(transform, 0);
            border.Background = null;
        };
    }

    private static void AnimateY(TranslateTransform transform, double to)
    {
        var storyboard = new Storyboard();
        var animation = new DoubleAnimation
        {
            To = to,
            Duration = new Duration(AnimDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(animation, transform);
        Storyboard.SetTargetProperty(animation, "Y");
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }
}
