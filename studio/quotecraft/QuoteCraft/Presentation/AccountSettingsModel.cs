namespace QuoteCraft.Presentation;

/// <summary>
/// Settings' account and billing (part of <see cref="SettingsModel"/>): with a Supabase project configured, the plan,
/// the quotes used, Stripe checkout and the billing portal, and Sign In or Sign Out (D3, D17); without one, the note
/// that the data stays on this device.
/// </summary>
public partial record AccountSettingsModel
{
    private readonly INavigator _navigator;
    private readonly IFeatureGateService _featureGate;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IAuthService _authService;
    private readonly Data.Remote.SupabaseConfig _supabase;

    public AccountSettingsModel(
        INavigator navigator,
        IFeatureGateService featureGate,
        ISubscriptionService subscriptionService,
        IAuthService authService,
        Data.Remote.SupabaseConfig supabase)
    {
        _navigator = navigator;
        _featureGate = featureGate;
        _subscriptionService = subscriptionService;
        _authService = authService;
        _supabase = supabase;
    }

    /// <summary>
    /// Empty when a Supabase project is configured (the plans, subscription and account sections show); otherwise the
    /// local-mode note that takes their place.
    /// </summary>
    public IFeed<string> LocalModeNote => Feed.Async(ct => ValueTask.FromResult(
        _supabase.IsConfigured ? string.Empty : Data.Remote.SupabaseConfig.NotConfiguredMessage));

    public IFeed<string> CurrentPlanName => Feed.Async(ct =>
    {
        var maxQuotes = _featureGate.MaxQuotesPerMonth;
        return ValueTask.FromResult(_featureGate.CurrentTier switch
        {
            SubscriptionTier.Free => $"Free ({maxQuotes} quotes/mo)",
            SubscriptionTier.Pro => "Pro (Unlimited)",
            SubscriptionTier.Business => "Business (Unlimited)",
            _ => "Free",
        });
    });

    public IFeed<string> QuotaUsage => Feed.Async(async ct =>
    {
        var used = await _featureGate.GetQuotesUsedThisMonthAsync();
        var max = _featureGate.MaxQuotesPerMonth;
        return max == int.MaxValue ? $"{used} this month" : $"{used} of {max}";
    });

    /// <summary>Pro, billed monthly: Stripe Checkout in the browser.</summary>
    public ValueTask UpgradeToPro(CancellationToken ct) => OpenCheckoutAsync("monthly", ct);

    /// <summary>Pro, billed yearly at the annual discount (PRD F13.6): Stripe Checkout in the browser.</summary>
    public ValueTask UpgradeToProAnnual(CancellationToken ct) => OpenCheckoutAsync("annual", ct);

    private ValueTask OpenCheckoutAsync(string interval, CancellationToken ct) => _navigator.RunAsync(this, "Checkout could not open.", async () =>
    {
        var url = await _subscriptionService.GetCheckoutUrlAsync("pro", interval);
        await LaunchAsync(url, "Checkout is not available right now. Try again later.");
    }, ct);

    public ValueTask ManageSubscription(CancellationToken ct) => _navigator.RunAsync(this, "The billing portal could not open.", async () =>
    {
        var url = await _subscriptionService.GetCustomerPortalUrlAsync();
        await LaunchAsync(url, "The billing portal is not available right now. Try again later.");
    }, ct);

    /// <summary>Opens a billing page in the browser; says so when there is none or nothing opened it.</summary>
    private static async Task LaunchAsync(string? url, string unavailable)
    {
        if (string.IsNullOrEmpty(url) || !await Windows.System.Launcher.LaunchUriAsync(new Uri(url)))
        {
            throw new InvalidOperationException(unavailable);
        }
    }

    /// <summary>Signed in to the configured Supabase project (only then is Sign Out offered; Sign In otherwise).</summary>
    public IFeed<bool> IsSignedIn => Feed.AsyncEnumerable(WatchSignedInAsync);

    private async IAsyncEnumerable<bool> WatchSignedInAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var changed = new ChangeSignal();
        Action<AuthState> handler = _ => changed.Raise();
        _authService.AuthStateChanged += handler;
        try
        {
            while (true)
            {
                yield return _authService.CurrentState.IsAuthenticated;
                await changed.WaitAsync(ct);
            }
        }
        finally
        {
            _authService.AuthStateChanged -= handler;
        }
    }

    /// <summary>Sign In (a Supabase project is configured and nobody is signed in): the sign-in page.</summary>
    public ValueTask SignIn(CancellationToken ct) => _navigator.RunAsync(this, "The sign-in page could not open.",
        async () => await _navigator.NavigateRouteAsync(this, "Auth", cancellation: ct), ct);

    public ValueTask SignOut(CancellationToken ct) => _navigator.RunAsync(this, "You could not be signed out.", async () =>
    {
        await _authService.SignOutAsync();
        await _navigator.NavigateRouteAsync(this, "Auth", qualifier: Qualifiers.ClearBackStack, cancellation: ct);
    }, ct);
}
