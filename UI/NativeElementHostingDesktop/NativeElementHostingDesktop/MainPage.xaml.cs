using System.Diagnostics;
using Uno.Disposables;
using Uno.UI.NativeElementHosting;
using Uno.WinUI.Runtime.Skia.X11;
namespace NativeElementHostingDesktop;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsLinux())
        {
            cp.Content = await OpenXterm();
        }
        else
        {
            cp.Content = "Failed to load! This sample is only for Windows and Linux.";
        }
    }

    private Task<X11NativeWindow> OpenXterm()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "xterm",
                UseShellExecute = false
            }
        };


        // start minimized
        process.StartInfo.ArgumentList.Add("-iconic");
        // run top
        process.StartInfo.ArgumentList.Add("-e");
        process.StartInfo.ArgumentList.Add("top");

        process.Start();

        var tcs = new TaskCompletionSource<X11NativeWindow>();

        _ = Task.Run(() =>
        {
            while (true)
            {
                var display = XLib.XOpenDisplay(IntPtr.Zero);
                // This will 
                X11NativeWindow windowXid = FindWindowByPid(display, process.Id, XLib.XDefaultRootWindow(display));
                _ = XLib.XCloseDisplay(display);
                if (windowXid.WindowId != IntPtr.Zero)
                {
                    tcs.TrySetResult(windowXid);
                    return;
                }
            }
        });
        
        return tcs.Task;
    }

    // Only supported on ICCCM-conformant X11 window managers
    private unsafe X11NativeWindow FindWindowByPid(IntPtr display, int pid, IntPtr windowToStartSearchingFrom)
    {
        _ = XLib.XGetWindowProperty(
            display,
            windowToStartSearchingFrom,
            XLib.XInternAtom(display, "_NET_WM_PID", false),
            IntPtr.Zero,
            0x7FFFFFFF,
            false,
            IntPtr.Zero, // AnyPropertyType
            out _,
            out var actualFormat,
            out var nitems,
            out var _,
            out var prop);
        using var atomsDisposable = Disposable.Create(() => XLib.XFree(prop));

        if (actualFormat == 32 && nitems > 0)
        {
            Debug.Assert(nitems == 1);
            var windowPid = *(int*)prop;
            if (windowPid == pid)
            {
                return new X11NativeWindow(windowToStartSearchingFrom);
            }
        }

        _ = XLib.XQueryTree(display, windowToStartSearchingFrom, out _, out _, out var childrenPtr, out int nchildren);
        using var childrenDisposable = Disposable.Create(() => XLib.XFree(childrenPtr));

        var children = new Span<IntPtr>((void*)childrenPtr, nchildren);
        foreach (var child in children)
        {
            var childRet = FindWindowByPid(display, pid, child);
            if (childRet.WindowId != IntPtr.Zero)
            {
                return childRet;
            }
        }

        return new X11NativeWindow(IntPtr.Zero);
    }
}
