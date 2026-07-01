using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace MenuFlyoutShowcase;

public sealed partial class MainPage : Page
{
    private int _eventCounter;

    public MainPage()
    {
        this.InitializeComponent();
    }

    private void LogEvent(string message)
    {
        _eventCounter++;
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        EventLogText.Text = $"[{_eventCounter}] {timestamp}: {message}\n" + EventLogText.Text;

        if (EventLogText.Text.Length > 4000)
        {
            EventLogText.Text = EventLogText.Text.Substring(0, 3500);
        }
    }

    private void OnMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item)
        {
            MenuStatus.Text = $"Invoked: {item.Text}";
            LogEvent($"MenuFlyoutItem clicked: '{item.Text}'");
        }
    }

    private void OnToggleClick(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleMenuFlyoutItem toggle)
        {
            MenuStatus.Text = $"{toggle.Text}: {(toggle.IsChecked ? "on" : "off")}";
            LogEvent($"ToggleMenuFlyoutItem '{toggle.Text}' -> {(toggle.IsChecked ? "checked" : "unchecked")}");
        }
    }

    private void OnSplitPrimaryClick(object sender, RoutedEventArgs e)
    {
        if (sender is SplitMenuFlyoutItem split)
        {
            MenuStatus.Text = $"Primary action: {split.Text}";
            LogEvent($"SplitMenuFlyoutItem primary action: '{split.Text}'");
        }
    }

    private void OnContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (args.TryGetPosition(sender, out var position))
        {
            PositionText.Text = $"Position: ({position.X:F0}, {position.Y:F0})";
            LogEvent($"ContextRequested (pointer) at ({position.X:F0}, {position.Y:F0})");
        }
        else
        {
            PositionText.Text = "Position: keyboard (Shift+F10)";
            LogEvent("ContextRequested (keyboard) - TryGetPosition returned false");
        }
    }

    private void OnContextCanceled(UIElement sender, RoutedEventArgs args)
    {
        PositionText.Text = "Context canceled";
        LogEvent("ContextCanceled - long-press dragged away before the menu opened");
    }

    private void OnClearLogClick(object sender, RoutedEventArgs e)
    {
        _eventCounter = 0;
        EventLogText.Text = "Events will be logged here...";
        PositionText.Text = "Position: (waiting...)";
    }
}
