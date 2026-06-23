using Microsoft.UI.Windowing;

namespace WindowingSamples;

/// <summary>
/// Window demonstrating ExtendsContentIntoTitleBar API.
/// This API allows extending XAML UI into the title bar area while preserving caption buttons.
/// </summary>
public sealed partial class ExtendContentIntoTitleBarWindow : Window
{
    public ExtendContentIntoTitleBarWindow()
    {
        InitializeComponent();

        Title = "Extend Content Into Title Bar";

        // Enable extending content into title bar
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
    }

    public void CloseClick(object sender, RoutedEventArgs args) => Close();
}
