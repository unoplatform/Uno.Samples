using System;
using Microsoft.UI.Xaml.Automation;

namespace NavigationViewShowcase;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
	}

	private void OnShellLoaded(object sender, RoutedEventArgs e)
	{
		// The Settings entry is materialized by the control, so it gets its id here.
		if (Shell.SettingsItem is NavigationViewItem settingsItem)
		{
			AutomationProperties.SetAutomationId(settingsItem, "NavSettings");
		}

		Shell.SelectedItem = DashboardItem;

		// Checked from here rather than XAML so the handler runs against a fully built tree.
		AutoPaneMode.IsChecked = true;
	}

	private void OnShellSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		if (args.IsSettingsSelected)
		{
			ShowDestination("Settings", "Workspace preferences, theming and notification defaults.");
		}
		else if (args.SelectedItemContainer is NavigationViewItem item)
		{
			ShowDestination(item.Content as string, item.Tag as string);
		}
	}

	private void OnShellDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args) => UpdateDisplayModeText();

	private void OnPaneModeChecked(object sender, RoutedEventArgs e)
	{
		if (sender is RadioButton { Tag: string mode })
		{
			Shell.PaneDisplayMode = Enum.Parse<NavigationViewPaneDisplayMode>(mode);
			UpdateDisplayModeText();
		}
	}

	private void ShowDestination(string? title, string? summary)
	{
		Shell.Header = title;
		DestinationTitle.Text = title ?? string.Empty;
		DestinationSummary.Text = summary ?? string.Empty;
	}

	private void UpdateDisplayModeText() =>
		DisplayModeText.Text = $"PaneDisplayMode: {Shell.PaneDisplayMode} — resolved DisplayMode: {Shell.DisplayMode}";
}
