namespace QuoteCraft.Services;

public interface IOnboardingService
{
    Task<bool> IsOnboardingCompleteAsync();
    Task MarkCompleteAsync();
}

public class OnboardingService : IOnboardingService
{
    private readonly string _flagFilePath;
    private bool? _cachedResult;

    public OnboardingService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuoteCraft");
        Directory.CreateDirectory(dir);
        _flagFilePath = Path.Combine(dir, ".onboarding_complete");
    }

    public Task<bool> IsOnboardingCompleteAsync()
    {
        _cachedResult ??= File.Exists(_flagFilePath);
        return Task.FromResult(_cachedResult.Value);
    }

    public async Task MarkCompleteAsync()
    {
        await File.WriteAllTextAsync(_flagFilePath, DateTimeOffset.UtcNow.ToString("O"));
        _cachedResult = true;
    }
}
