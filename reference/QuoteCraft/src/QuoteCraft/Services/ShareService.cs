using System.Diagnostics;

namespace QuoteCraft.Services;

public interface IShareService
{
    Task ShareQuotePdfAsync(QuoteEntity quote);
    Task GenerateAndDownloadPdfAsync(QuoteEntity quote);
    Task MarkAsSentAsync(QuoteEntity quote);
    Task<string> GenerateShareLinkAsync(QuoteEntity quote);
    Task CopyShareLinkAsync(QuoteEntity quote);
    Task ShareViaSmsAsync(QuoteEntity quote, string phoneNumber);
}

public class ShareService : IShareService
{
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly ILogger<ShareService> _logger;

    public ShareService(IPdfGenerator pdfGenerator, IQuoteRepository quoteRepo, ILogger<ShareService> logger)
    {
        _pdfGenerator = pdfGenerator;
        _quoteRepo = quoteRepo;
        _logger = logger;
    }

    public async Task ShareQuotePdfAsync(QuoteEntity quote)
    {
        await GenerateAndDownloadPdfAsync(quote);
        await MarkAsSentAsync(quote);
    }

    public async Task GenerateAndDownloadPdfAsync(QuoteEntity quote)
    {
        // Ensure line items are loaded
        if (quote.LineItems.Count == 0)
            quote.LineItems = await _quoteRepo.GetLineItemsAsync(quote.Id);

        var filePath = await _pdfGenerator.GenerateQuotePdfAsync(quote);

#if __BROWSERWASM__
        // WASM: trigger a browser download
        await DownloadFileInBrowserAsync(filePath, $"{quote.QuoteNumber}.pdf");
#else
        // Desktop/Mobile: open the PDF with the system's default application
        OpenFileWithDefaultApp(filePath);
#endif
    }

    public async Task MarkAsSentAsync(QuoteEntity quote)
    {
        if (quote.Status == QuoteStatus.Draft)
        {
            var fresh = await _quoteRepo.GetByIdAsync(quote.Id);
            if (fresh != null)
            {
                fresh.Status = QuoteStatus.Sent;
                fresh.SentAt = DateTimeOffset.UtcNow;
                await _quoteRepo.SaveAsync(fresh);
            }
        }
    }

    public async Task<string> GenerateShareLinkAsync(QuoteEntity quote)
    {
        var fresh = await _quoteRepo.GetByIdAsync(quote.Id) ?? quote;

        // Generate share token if not already present
        if (string.IsNullOrEmpty(fresh.ShareToken))
        {
            fresh.ShareToken = GenerateShareToken();
            fresh.UpdatedAt = DateTimeOffset.UtcNow;
            await _quoteRepo.SaveAsync(fresh);
        }

        // Construct the client-facing URL
        return $"{Data.Remote.SupabaseConfig.Url}/functions/v1/quote-view?token={fresh.ShareToken}";
    }

    public async Task CopyShareLinkAsync(QuoteEntity quote)
    {
        var link = await GenerateShareLinkAsync(quote);

        // Copy to clipboard using platform-specific approach
        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dataPackage.SetText(link);
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
    }

    public async Task ShareViaSmsAsync(QuoteEntity quote, string phoneNumber)
    {
        var link = await GenerateShareLinkAsync(quote);
        var message = $"Here's your quote #{quote.QuoteNumber}: {link}";

        // Use the system SMS launcher
        var uri = new Uri($"sms:{phoneNumber}?body={Uri.EscapeDataString(message)}");
        await Windows.System.Launcher.LaunchUriAsync(uri);

        await MarkAsSentAsync(quote);
    }

    private static string GenerateShareToken()
    {
        var bytes = new byte[16];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

#if !__BROWSERWASM__
    private void OpenFileWithDefaultApp(string filePath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
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
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "All file open methods failed for {FilePath}", filePath);
            }
        }
    }
#endif

#if __BROWSERWASM__
    private static async Task DownloadFileInBrowserAsync(string filePath, string fileName)
    {
        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var base64 = Convert.ToBase64String(bytes);

        // Use JS interop to trigger browser download
        var js = $@"
            (function() {{
                var byteChars = atob('{base64}');
                var byteArray = new Uint8Array(byteChars.length);
                for (var i = 0; i < byteChars.length; i++) {{
                    byteArray[i] = byteChars.charCodeAt(i);
                }}
                var blob = new Blob([byteArray], {{ type: 'application/pdf' }});
                var url = URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = '{fileName}';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }})();
        ";

        await Uno.Foundation.WebAssemblyRuntime.InvokeAsync(js);
    }
#endif
}
