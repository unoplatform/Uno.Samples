using System.IO;
using System.Runtime.InteropServices;

namespace ClaudeCodeTracker;

public partial class App
{
    /// <summary>
    /// Sets the macOS Dock icon at runtime. macOS only shows an app's real icon in the Dock for a
    /// packaged <c>.app</c> bundle; when the Skia desktop head is launched unbundled (e.g.
    /// <c>dotnet run</c>) the Dock shows a generic placeholder. There is no managed Uno API for
    /// this, so we set <c>NSApplication.applicationIconImage</c> via the Objective-C runtime from
    /// the generated app icon at <c>images/icon.png</c>. Compiled for the desktop head only and a
    /// no-op on any OS other than macOS.
    /// </summary>
    partial void SetMacOSDockIcon()
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }

        // Prefer the macOS-styled icon (rounded "squircle" body with the standard margin) since
        // macOS shows applicationIconImage as-is without masking; fall back to the square icon.
        var iconPath = Path.Combine(AppContext.BaseDirectory, "images", "icon-macos.png");
        if (!File.Exists(iconPath))
        {
            iconPath = Path.Combine(AppContext.BaseDirectory, "images", "icon.png");
        }
        if (!File.Exists(iconPath))
        {
            return;
        }

        var utf8Path = Marshal.StringToCoTaskMemUTF8(iconPath);
        try
        {
            // NSApplication.sharedApplication
            var app = objc_msgSend(objc_getClass("NSApplication"), sel_registerName("sharedApplication"));
            if (app == IntPtr.Zero)
            {
                return;
            }

            // [[NSImage alloc] initWithContentsOfFile:[NSString stringWithUTF8String:path]]
            var nsPath = objc_msgSend(objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), utf8Path);
            var image = objc_msgSend(objc_getClass("NSImage"), sel_registerName("alloc"));
            image = objc_msgSend(image, sel_registerName("initWithContentsOfFile:"), nsPath);

            // [NSApp setApplicationIconImage:image]
            if (image != IntPtr.Zero)
            {
                objc_msgSend(app, sel_registerName("setApplicationIconImage:"), image);
            }
        }
        catch
        {
            // Best-effort dev convenience; ignore if AppKit interop is unavailable.
        }
        finally
        {
            Marshal.FreeCoTaskMem(utf8Path);
        }
    }

    private const string ObjCRuntime = "/usr/lib/libobjc.A.dylib";

    [DllImport(ObjCRuntime, CharSet = CharSet.Ansi)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(ObjCRuntime, CharSet = CharSet.Ansi)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(ObjCRuntime)]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport(ObjCRuntime)]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);
}
