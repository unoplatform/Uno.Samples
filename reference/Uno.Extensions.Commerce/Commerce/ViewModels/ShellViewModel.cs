
using Uno.Extensions.Authentication;

namespace Commerce.ViewModels;

public class ShellViewModel
{
    private readonly INavigator _navigator;
    private readonly ITokenCache _tokenCache;


    public ShellViewModel(
        ILogger<ShellViewModel> logger,
        INavigator navigator,
        IOptions<HostConfiguration> configuration,
        ITokenCache tokenCache)
    {
        _navigator = navigator;
        _tokenCache = tokenCache;

        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation($"Launch url '{configuration.Value?.LaunchUrl}'");
        var initialRoute = configuration.Value?.LaunchUrl;

        // Go to the login page on app startup
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Start(initialRoute);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        tokenCache.Cleared += TokenCacheCleared;
    }

    private async void TokenCacheCleared(object? sender, EventArgs e)
    {
        await Start();
    }

    public async Task Start(string? initialRoute = default)
    {

        if (await _tokenCache.HasTokenAsync(CancellationToken.None))
        {
            if (initialRoute is not null && 
                !string.IsNullOrWhiteSpace(initialRoute))
            {
                var initialResponse = await _navigator.NavigateRouteForResultAsync<object>(this, initialRoute);
                if (initialResponse is not null)
                {
                    _ = await initialResponse.Result;
                }
            }
            else
            {
                var homeResponse = await _navigator.NavigateViewModelAsync<HomeViewModel>(this, Qualifiers.ClearBackStack);
            }
        }
        else
        {
            // Navigate to Login page, requesting Credentials
            var response = await _navigator.GetDataAsync<Credentials>(this, qualifier: Qualifiers.ClearBackStack);

            if (response is not null)
            {
                _ = Start();
                return;
            }
        }
    }
}

public class HomeViewModel { }
