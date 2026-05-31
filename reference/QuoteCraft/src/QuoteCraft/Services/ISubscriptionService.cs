namespace QuoteCraft.Services;

public interface ISubscriptionService
{
    SubscriptionTier CurrentTier { get; }
    event Action<SubscriptionTier>? TierChanged;

    Task RefreshTierAsync();
    Task<string?> GetCheckoutUrlAsync(string tier = "pro", string interval = "monthly");
    Task<string?> GetCustomerPortalUrlAsync();
}
