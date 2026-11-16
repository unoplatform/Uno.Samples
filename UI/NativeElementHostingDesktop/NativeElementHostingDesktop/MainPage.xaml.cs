using System.Diagnostics;
using System.Globalization;
using Uno.UI.NativeElementHosting;
using Uno.WinUI.Runtime.Skia.X11;
using Windows.Win32;
using Windows.Win32.Foundation;
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
        else if (OperatingSystem.IsWindows())
        {
            cp.Content = await OpenPowershell();
        }
        else
        {
            cp.Content = "Failed to load! This sample is only for Windows and Linux.";
        }
    }

    private Task<Win32NativeWindow> OpenPowershell()
    {
        var windowTitle = Random.Shared.NextInt64().ToString(CultureInfo.InvariantCulture);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoExit -Command \"$Host.UI.RawUI.WindowTitle = '{windowTitle}'\"",
                UseShellExecute = true
            }
        };

        process.Start();

        var tcs = new TaskCompletionSource<Win32NativeWindow>();

        _ = Task.Run(() =>
        {
            while (true)
            {
                var hwnd = PInvoke.FindWindow(null, windowTitle);
                if (hwnd != HWND.Null)
                {
                    tcs.TrySetResult(new Win32NativeWindow(hwnd));
                    return;
                }
            }
        });
        
        return tcs.Task;
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
                X11NativeWindow windowXid = FindX11WindowByPid(display, process.Id, XLib.XDefaultRootWindow(display));
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
    private unsafe X11NativeWindow FindX11WindowByPid(IntPtr display, int pid, IntPtr windowToStartSearchingFrom)
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

        if (actualFormat == 32 && nitems > 0)
        {
            Debug.Assert(nitems == 1);
            var windowPid = *(int*)prop;
            if (windowPid == pid)
            {
                _ = XLib.XFree(prop);
                return new X11NativeWindow(windowToStartSearchingFrom);
            }
        }
        else
        {
            _ = XLib.XFree(prop);
        }

        _ = XLib.XQueryTree(display, windowToStartSearchingFrom, out _, out _, out var childrenPtr, out int nchildren);

        var children = new Span<IntPtr>((void*)childrenPtr, nchildren);
        foreach (var child in children)
        {
            var childRet = FindX11WindowByPid(display, pid, child);
            if (childRet.WindowId != IntPtr.Zero)
            {
                _ = XLib.XFree(childrenPtr);
                return childRet;
            }
        }
        
        _ = XLib.XFree(childrenPtr);
        return new X11NativeWindow(IntPtr.Zero);
    }
}
