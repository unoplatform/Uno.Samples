namespace QuoteCraft.Services;

public interface ISubscriptionService
{
    SubscriptionTier CurrentTier { get; }

    /// <summary>
    /// False when no Supabase project is configured. There are no plans then: the app runs locally with every
    /// on-device feature (docs/DECISIONS-production-readiness.md, D3).
    /// </summary>
    bool IsAvailable { get; }

    Task<string?> GetCheckoutUrlAsync(string tier = "pro", string interval = "monthly");
    Task<string?> GetCustomerPortalUrlAsync();
}
