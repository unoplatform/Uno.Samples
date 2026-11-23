namespace QuoteCraft.Services;

public class EmailLauncherService : IEmailLauncherService
{
    private readonly ILogger<EmailLauncherService> _logger;

    public EmailLauncherService(ILogger<EmailLauncherService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> ComposeEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var encodedSubject = Uri.EscapeDataString(subject);
            var encodedBody = Uri.EscapeDataString(body);
            var mailtoUri = new Uri($"mailto:{toEmail}?subject={encodedSubject}&body={encodedBody}");

            return await Windows.System.Launcher.LaunchUriAsync(mailtoUri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to launch email client");
            return false;
        }
    }
}
