using System;
using System.IO;

namespace FitBeginnerApp;

public partial class App
{
    // Desktop-only implementation of the App partial declared in App.xaml.cs. macOS shows a generic
    // "exec" Dock tile for an unbundled `dotnet run` (the real icon only appears for a packaged .app),
    // so we set NSApplication.applicationIconImage at startup to brand the Dock during development.
    // macOS does NOT mask applicationIconImage (unlike iOS), so DockIcon.png is pre-baked as the
    // rounded "squircle" macOS-grid icon. Windows/Linux get the full-bleed square icon via
    // MainWindow.SetWindowIcon(); this path is a runtime no-op off macOS.
    partial void TrySetDesktopDockIcon()
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }

        var iconPath = Path.Combine(AppContext.BaseDirectory, "DockIcon.png");
        MacOSDockIcon.TrySet(iconPath);
    }
}
