#if !__WASM__ && !ANDROID && !IOS
using System.Diagnostics;
#endif

namespace QuoteCraft.Services;

public interface IShareService
{
    /// <summary>
    /// Draws the quote's PDF and hands it to the user: desktop and Windows write it to the exports folder and open it,
    /// Android asks where to save it, iOS offers the system's export sheet (Save to Files), and the browser downloads it.
    /// It changes nothing on the quote: sending is a separate step.
    /// </summary>
    /// <returns>What the user needs to know when the hand-off fell short (desktop: the file is saved but no app
    /// opened it, and where it is); null otherwise.</returns>
    Task<string?> GenerateAndDownloadPdfAsync(QuoteEntity quote);
}

public class ShareService : IShareService
{
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly ILogger<ShareService> _logger;

    public ShareService(
        IPdfGenerator pdfGenerator,
        IQuoteRepository quoteRepo,
        ILogger<ShareService> logger)
    {
        _pdfGenerator = pdfGenerator;
        _quoteRepo = quoteRepo;
        _logger = logger;
    }

    public async Task<string?> GenerateAndDownloadPdfAsync(QuoteEntity quote)
    {
        if (quote.LineItems.Count == 0)
            quote.LineItems = await _quoteRepo.GetLineItemsAsync(quote.Id);

        var pdf = await _pdfGenerator.GenerateQuotePdfAsync(quote);
        var fileName = $"{quote.QuoteNumber}.pdf";

#if __WASM__ || ANDROID
        await OnUiThreadAsync(() => SaveWithPickerAsync(pdf, fileName));
#elif IOS
        var path = WriteExport(pdf, fileName);
        await OnUiThreadAsync(() =>
        {
            PresentExportPicker(path);
            return Task.CompletedTask;
        });
#else
        // Desktop and Windows: the exports folder, then the system's PDF viewer; if none opens it, say where it is.
        var path = WriteExport(pdf, fileName);
        if (!OpenFileWithDefaultApp(path))
        {
            return $"{fileName} is saved in {Path.GetDirectoryName(path)}, but no app on this computer opened it.";
        }
#endif
        _logger.LogInformation("PDF for {QuoteNumber} handed over ({Bytes} bytes)", quote.QuoteNumber, pdf.Length);
        return null;
    }

#if __WASM__ || ANDROID
    /// <summary>
    /// Android: the system's Save dialog (ACTION_CREATE_DOCUMENT). WebAssembly: the browser's download, in every browser;
    /// the File System Access save dialog would need a fresh user gesture, and the PDF is drawn before this runs.
    /// </summary>
    private static async Task SaveWithPickerAsync(byte[] pdf, string fileName)
    {
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = Path.GetFileNameWithoutExtension(fileName),
        };
        picker.FileTypeChoices.Add("PDF document", new List<string> { ".pdf" });

        Windows.Storage.StorageFile? file;
#if __WASM__
        var pickers = Uno.WinRTFeatureConfiguration.Storage.Pickers.WasmConfiguration;
        Uno.WinRTFeatureConfiguration.Storage.Pickers.WasmConfiguration = Uno.WasmPickerConfiguration.DownloadUpload;
        try
        {
            file = await picker.PickSaveFileAsync();
        }
        finally
        {
            Uno.WinRTFeatureConfiguration.Storage.Pickers.WasmConfiguration = pickers;
        }
#else
        file = await picker.PickSaveFileAsync();
#endif
        if (file is null)
        {
            return; // The user cancelled.
        }

        Windows.Storage.CachedFileManager.DeferUpdates(file);
        await Windows.Storage.FileIO.WriteBytesAsync(file, pdf);
        // WebAssembly: the download starts here.
        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
    }
#endif

#if IOS
    /// <summary>
    /// iOS: Uno's FileSavePicker needs an app-provided extension there ("FileSavePicker extension is not registered"), so
    /// the PDF goes to the system's export picker, which copies it where the user chooses (Files, iCloud Drive).
    /// </summary>
    private static void PresentExportPicker(string path)
    {
        var picker = new UIKit.UIDocumentPickerViewController(new[] { Foundation.NSUrl.FromFilename(path) }, true);
        var top = UIKit.UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIKit.UIWindowScene>()
            .SelectMany(scene => scene.Windows)
            .FirstOrDefault(window => window.IsKeyWindow)?
            .RootViewController;
        while (top?.PresentedViewController is { } presented)
        {
            top = presented;
        }

        if (top is null)
        {
            throw new InvalidOperationException("The PDF was created, but there is no window to show the save dialog in.");
        }
        top.PresentViewController(picker, true, null);
    }
#endif

#if __WASM__ || ANDROID || IOS
    /// <summary>Pickers open on the UI thread; MVUX runs commands on a background thread.</summary>
    private static Task OnUiThreadAsync(Func<Task> action)
    {
        var queue = App.MainAppWindow.DispatcherQueue;
        if (queue.HasThreadAccess)
        {
            return action();
        }

        var done = new TaskCompletionSource();
        if (!queue.TryEnqueue(async () =>
            {
                try
                {
                    await action();
                    done.SetResult();
                }
                catch (Exception ex)
                {
                    done.SetException(ex);
                }
            }))
        {
            done.SetException(new InvalidOperationException("The save dialog could not be opened."));
        }
        return done.Task;
    }
#endif

#if !__WASM__ && !ANDROID
    private static string WriteExport(byte[] pdf, string fileName)
    {
        try
        {
            var dir = AppStorage.Combine("exports");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, fileName);
            File.WriteAllBytes(path, pdf);
            return path;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Most often the previous export of this quote is still open in a PDF viewer that locks it (Windows).
            throw new InvalidOperationException(
                "The PDF file could not be written. If it is open in another app, close it and try again.", ex);
        }
    }
#endif

#if !__WASM__ && !ANDROID && !IOS
    /// <returns>False when nothing could open the file.</returns>
    private bool OpenFileWithDefaultApp(string filePath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Shell execute failed for {FilePath}, trying platform fallback", filePath);
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    Process.Start("explorer.exe", $"\"{filePath}\"");
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", filePath);
                }
                else if (OperatingSystem.IsLinux())
                {
                    Process.Start("xdg-open", filePath);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "All file open methods failed for {FilePath}", filePath);
                return false;
            }
        }
    }
#endif
}
