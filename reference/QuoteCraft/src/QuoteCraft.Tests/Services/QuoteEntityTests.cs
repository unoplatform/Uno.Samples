using QuoteCraft.Models;
using Xunit;

namespace QuoteCraft.Tests.Services;

public class QuoteEntityTests
{
    [Fact]
    public void Subtotal_SumsLineItems()
    {
        var quote = new QuoteEntity
        {
            LineItems =
            [
                new LineItemEntity { UnitPrice = 100m, Quantity = 2 },
                new LineItemEntity { UnitPrice = 50m, Quantity = 3 },
            ]
        };

        Assert.Equal(350m, quote.Subtotal);
    }

    [Fact]
    public void TaxAmount_CalculatesCorrectly()
    {
        var quote = new QuoteEntity
        {
            TaxRate = 10m,
            LineItems =
            [
                new LineItemEntity { UnitPrice = 100m, Quantity = 1 },
            ]
        };

        Assert.Equal(10m, quote.TaxAmount);
    }

    [Fact]
    public void Total_IsSubtotalPlusTax()
    {
        var quote = new QuoteEntity
        {
            TaxRate = 13m,
            LineItems =
            [
                new LineItemEntity { UnitPrice = 200m, Quantity = 1 },
            ]
        };

        Assert.Equal(226m, quote.Total);
    }

    [Fact]
    public void Total_NoLineItems_IsZero()
    {
        var quote = new QuoteEntity { TaxRate = 13m };
        Assert.Equal(0m, quote.Total);
    }

    [Fact]
    public void LineItemTotal_IsUnitPriceTimesQuantity()
    {
        var item = new LineItemEntity { UnitPrice = 12.50m, Quantity = 10 };
        Assert.Equal(125m, item.LineTotal);
    }

    [Fact]
    public void NewQuote_HasDefaultValues()
    {
        var quote = new QuoteEntity();

        Assert.NotNull(quote.Id);
        Assert.Equal(QuoteStatus.Draft, quote.Status);
        Assert.False(quote.IsDeleted);
        Assert.Null(quote.SyncedAt);
        Assert.Null(quote.ShareToken);
    }

    [Fact]
    public void NewClient_HasDefaultValues()
    {
        var client = new ClientEntity();

        Assert.NotNull(client.Id);
        Assert.False(client.IsDeleted);
        Assert.Null(client.SyncedAt);
    }
}
