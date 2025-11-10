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
        ExtendsContentIntoTitleBar = true;

        // Set the custom title bar element for drag region
        SetTitleBar(CustomTitleBar);
    }

    public void CloseClick(object sender, RoutedEventArgs args) => Close();
}
