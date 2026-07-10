using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BrewHouse;

/// <summary>
/// Sets the macOS Dock icon at runtime from a PNG on disk, using raw Objective-C runtime P/Invoke
/// (libobjc + AppKit) only — no Xamarin/.NET-for-iOS bindings (the Skia desktop head has none). This
/// file lives under Platforms/Desktop, so it is compiled into the desktop head ONLY and never reaches
/// the iOS/Android/WebAssembly heads. It still no-ops at runtime on non-macOS desktops (Windows/Linux).
/// </summary>
internal static class MacOSDockIcon
{
    private const string LibObjC = "/usr/lib/libobjc.dylib";
    private const string AppKit = "/System/Library/Frameworks/AppKit.framework/AppKit";

    [DllImport(LibObjC, EntryPoint = "objc_getClass", CharSet = CharSet.Ansi)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(LibObjC, EntryPoint = "sel_registerName", CharSet = CharSet.Ansi)]
    private static extern IntPtr sel_registerName(string name);

    // objc_msgSend has no usable varargs form from C#: you MUST declare one [DllImport] per distinct
    // call-site shape (return type + every argument type), each pointing at EntryPoint "objc_msgSend".
    // A single object[]/params overload mismarshals and corrupts registers, notably on arm64.

    // (id, SEL) -> id   e.g. [NSApplication sharedApplication], [NSImage alloc]
    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr IntPtr_msgSend(IntPtr receiver, IntPtr selector);

    // (id, SEL, char*) -> id   e.g. [NSString stringWithUTF8String:]
    [DllImport(LibObjC, EntryPoint = "objc_msgSend", CharSet = CharSet.Ansi)]
    private static extern IntPtr IntPtr_msgSend_str(IntPtr receiver, IntPtr selector, string arg1);

    // (id, SEL, id) -> id   e.g. [[NSImage alloc] initWithContentsOfFile:]
    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr IntPtr_msgSend_ptr(IntPtr receiver, IntPtr selector, IntPtr arg1);

    // (id, SEL, id) -> void   e.g. [NSApp setApplicationIconImage:]
    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern void void_msgSend_ptr(IntPtr receiver, IntPtr selector, IntPtr arg1);

    // (id, SEL) -> void   e.g. [image release]
    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern void void_msgSend(IntPtr receiver, IntPtr selector);

    /// <summary>Sets the Dock icon to the PNG at <paramref name="pngPath"/>. No-ops on any failure.</summary>
    public static void TrySet(string pngPath)
    {
        try
        {
            if (!OperatingSystem.IsMacOS() || string.IsNullOrEmpty(pngPath) || !File.Exists(pngPath))
            {
                return;
            }

            // Ensure AppKit's classes are registered. In a Skia desktop head the windowing stack has
            // already pulled AppKit in, but loading it explicitly is cheap and safe.
            NativeLibrary.TryLoad(AppKit, out _);

            var nsApplication = objc_getClass("NSApplication");
            var nsImage = objc_getClass("NSImage");
            var nsString = objc_getClass("NSString");
            if (nsApplication == IntPtr.Zero || nsImage == IntPtr.Zero || nsString == IntPtr.Zero)
            {
                return;
            }

            // app = [NSApplication sharedApplication];
            var app = IntPtr_msgSend(nsApplication, sel_registerName("sharedApplication"));
            if (app == IntPtr.Zero)
            {
                return;
            }

            // path = [NSString stringWithUTF8String:pngPath];  (autoreleased — do not release)
            var path = IntPtr_msgSend_str(nsString, sel_registerName("stringWithUTF8String:"), pngPath);

            // image = [[NSImage alloc] initWithContentsOfFile:path];  (owned — release after use)
            var alloc = IntPtr_msgSend(nsImage, sel_registerName("alloc"));
            var image = IntPtr_msgSend_ptr(alloc, sel_registerName("initWithContentsOfFile:"), path);
            if (image == IntPtr.Zero)
            {
                return;
            }

            // [app setApplicationIconImage:image];
            void_msgSend_ptr(app, sel_registerName("setApplicationIconImage:"), image);

            // [image release];  (setApplicationIconImage: retained it)
            void_msgSend(image, sel_registerName("release"));
        }
        catch
        {
            // Best-effort: a cosmetic Dock-icon failure must never crash the app.
        }
    }
}
