using NSubstitute;
using QuoteCraft.Data;
using QuoteCraft.Services;
using Xunit;

namespace QuoteCraft.Tests.Services;

public class FeatureGateServiceTests
{
    private readonly IQuoteRepository _quoteRepo = Substitute.For<IQuoteRepository>();
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly ISubscriptionService _subscriptionService = Substitute.For<ISubscriptionService>();

    private FeatureGateService CreateService()
    {
        return new FeatureGateService(_quoteRepo, _clientRepo, _subscriptionService);
    }

    [Fact]
    public void FreeTier_MaxQuotesPerMonth_Is5()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        var svc = CreateService();
        Assert.Equal(5, svc.MaxQuotesPerMonth);
    }

    [Fact]
    public void ProTier_MaxQuotesPerMonth_IsUnlimited()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Pro);
        var svc = CreateService();
        Assert.Equal(int.MaxValue, svc.MaxQuotesPerMonth);
    }

    [Fact]
    public void FreeTier_MaxClients_Is10()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        var svc = CreateService();
        Assert.Equal(10, svc.MaxClients);
    }

    [Fact]
    public void FreeTier_HasPdfWatermark_IsTrue()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        var svc = CreateService();
        Assert.True(svc.HasPdfWatermark);
    }

    [Fact]
    public void ProTier_HasPdfWatermark_IsFalse()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Pro);
        var svc = CreateService();
        Assert.False(svc.HasPdfWatermark);
    }

    [Fact]
    public void FreeTier_HasCloudSync_IsFalse()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        var svc = CreateService();
        Assert.False(svc.HasCloudSync);
    }

    [Fact]
    public void ProTier_HasCloudSync_IsTrue()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Pro);
        var svc = CreateService();
        Assert.True(svc.HasCloudSync);
    }

    [Fact]
    public async Task CanCreateQuote_FreeTier_UnderLimit_ReturnsTrue()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        _quoteRepo.CountCreatedSinceAsync(Arg.Any<DateTimeOffset>()).Returns(3);
        var svc = CreateService();

        var result = await svc.CanCreateQuoteAsync();
        Assert.True(result);
    }

    [Fact]
    public async Task CanCreateQuote_FreeTier_AtLimit_ReturnsFalse()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        _quoteRepo.CountCreatedSinceAsync(Arg.Any<DateTimeOffset>()).Returns(5);
        var svc = CreateService();

        var result = await svc.CanCreateQuoteAsync();
        Assert.False(result);
    }

    [Fact]
    public async Task CanCreateQuote_ProTier_Always_ReturnsTrue()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Pro);
        var svc = CreateService();

        var result = await svc.CanCreateQuoteAsync();
        Assert.True(result);
    }

    [Fact]
    public async Task CanAddClient_FreeTier_UnderLimit_ReturnsTrue()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        _clientRepo.CountActiveAsync().Returns(5);
        var svc = CreateService();

        var result = await svc.CanAddClientAsync();
        Assert.True(result);
    }

    [Fact]
    public async Task CanAddClient_FreeTier_AtLimit_ReturnsFalse()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        _clientRepo.CountActiveAsync().Returns(10);
        var svc = CreateService();

        var result = await svc.CanAddClientAsync();
        Assert.False(result);
    }

    [Fact]
    public void GetUpgradeMessage_Quotes_ContainsPrice()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        var svc = CreateService();

        var message = svc.GetUpgradeMessage("quotes");
        Assert.Contains("$15/mo", message);
        Assert.Contains("5", message);
    }

    [Fact]
    public void GetUpgradeMessage_Clients_ContainsLimit()
    {
        _subscriptionService.CurrentTier.Returns(SubscriptionTier.Free);
        var svc = CreateService();

        var message = svc.GetUpgradeMessage("clients");
        Assert.Contains("10", message);
    }
}
