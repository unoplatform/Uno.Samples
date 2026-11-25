namespace QuoteCraft.Services;

public enum SubscriptionTier
{
    Free,
    Pro,
    Business
}

public interface IFeatureGateService
{
    SubscriptionTier CurrentTier { get; }

    /// <summary>
    /// True when plans apply: a Supabase project with billing is configured. Without one QuoteCraft runs locally,
    /// with no plan limits and no upgrade prompts (docs/DECISIONS-production-readiness.md, D3).
    /// </summary>
    bool HasPlans { get; }
    Task<bool> CanCreateQuoteAsync();
    Task<bool> CanAddClientAsync();
    Task<int> GetQuotesUsedThisMonthAsync();
    Task<int> GetClientCountAsync();
    int MaxQuotesPerMonth { get; }
    int MaxClients { get; }
    bool HasPdfWatermark { get; }
    bool HasCloudSync { get; }
    string GetUpgradeMessage(string feature);
}

public class FeatureGateService : IFeatureGateService
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly ISubscriptionService _subscriptionService;

    public FeatureGateService(
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        ISubscriptionService subscriptionService)
    {
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _subscriptionService = subscriptionService;
    }

    public SubscriptionTier CurrentTier => _subscriptionService.CurrentTier;

    public bool HasPlans => _subscriptionService.IsAvailable;

    // Free-plan limits apply only where a paid plan can be bought; local-only builds have no plans.
    private bool IsFreePlan => HasPlans && CurrentTier == SubscriptionTier.Free;

    public int MaxQuotesPerMonth => IsFreePlan ? 5 : int.MaxValue;

    public int MaxClients => IsFreePlan ? 10 : int.MaxValue;

    public bool HasPdfWatermark => IsFreePlan;

    /// <summary>
    /// Off for every plan in this version: sync would send quotes without their line items, and the catalog and
    /// business profile not at all (docs/DECISIONS-production-readiness.md, D8). SyncService stays unstarted.
    /// </summary>
    public bool HasCloudSync => false;

    public async Task<bool> CanCreateQuoteAsync()
    {
        if (!IsFreePlan)
            return true;

        var usedThisMonth = await GetQuotesUsedThisMonthAsync();
        return usedThisMonth < MaxQuotesPerMonth;
    }

    public async Task<bool> CanAddClientAsync()
    {
        if (!IsFreePlan)
            return true;

        var count = await GetClientCountAsync();
        return count < MaxClients;
    }

    public async Task<int> GetQuotesUsedThisMonthAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        return await _quoteRepo.CountCreatedSinceAsync(startOfMonth);
    }

    public async Task<int> GetClientCountAsync()
    {
        return await _clientRepo.CountActiveAsync();
    }

    public string GetUpgradeMessage(string feature) => feature switch
    {
        "quotes" => $"You've used all {MaxQuotesPerMonth} free quotes this month. Upgrade to Pro for unlimited quotes at $15/mo.",
        "clients" => $"You've reached the {MaxClients} client limit. Upgrade to Pro for unlimited clients at $15/mo.",
        "branding" => "Remove the QuoteCraft watermark and add your logo. Upgrade to Pro at $15/mo.",
        "catalogs" => "Unlock all trade catalogs (Plumbing, Electrical, HVAC, Painting). Upgrade to Pro at $15/mo.",
        _ => "Upgrade to QuoteCraft Pro for the full experience at $15/mo."
    };
}
