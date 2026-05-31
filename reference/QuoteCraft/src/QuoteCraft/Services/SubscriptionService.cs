using System.Text.Json;
using System.Text.Json.Serialization;
using QuoteCraft.Data.Remote;

namespace QuoteCraft.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IAuthService _authService;
    private readonly SupabaseClient _supabase;
    private readonly ILogger<SubscriptionService> _logger;
    private SubscriptionTier _currentTier = SubscriptionTier.Free;

    public SubscriptionTier CurrentTier => _currentTier;
    public event Action<SubscriptionTier>? TierChanged;

    public SubscriptionService(
        IAuthService authService,
        SupabaseClient supabase,
        ILogger<SubscriptionService> logger)
    {
        _authService = authService;
        _supabase = supabase;
        _logger = logger;

        _authService.AuthStateChanged += async state =>
        {
            if (state.IsAuthenticated)
                await RefreshTierAsync();
            else
                UpdateTier(SubscriptionTier.Free);
        };
    }

    public async Task RefreshTierAsync()
    {
        if (!_authService.CurrentState.IsAuthenticated)
        {
            UpdateTier(SubscriptionTier.Free);
            return;
        }

        try
        {
            var results = await _supabase.GetAsync<SubscriptionRecord>(
                "subscriptions",
                $"select=tier,status&user_id=eq.{_authService.CurrentState.UserId}&limit=1");

            if (results.Count > 0 && results[0].Status == "active")
            {
                var tier = results[0].Tier switch
                {
                    "pro" => SubscriptionTier.Pro,
                    "business" => SubscriptionTier.Business,
                    _ => SubscriptionTier.Free,
                };
                UpdateTier(tier);
            }
            else
            {
                UpdateTier(SubscriptionTier.Free);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to refresh subscription tier");
        }
    }

    public async Task<string?> GetCheckoutUrlAsync(string tier = "pro", string interval = "monthly")
    {
        try
        {
            var result = await _supabase.InvokeFunctionAsync<CheckoutResponse>(
                "create-checkout-session",
                new { tier, interval });

            return result?.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create checkout session");
            return null;
        }
    }

    public Task<string?> GetCustomerPortalUrlAsync()
    {
        // Stripe Customer Portal URL — configured in Stripe Dashboard
        // This is a static URL that redirects authenticated customers
        return Task.FromResult<string?>("https://billing.stripe.com/p/login/YOUR_PORTAL_LINK");
    }

    private void UpdateTier(SubscriptionTier tier)
    {
        if (_currentTier != tier)
        {
            _currentTier = tier;
            TierChanged?.Invoke(tier);
        }
    }

    private class SubscriptionRecord
    {
        [JsonPropertyName("tier")]
        public string Tier { get; set; } = "free";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "active";
    }

    private class CheckoutResponse
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
