namespace QuoteCraft.Services;

public interface IEmailLauncherService
{
    Task<bool> ComposeEmailAsync(string toEmail, string subject, string body);
}
