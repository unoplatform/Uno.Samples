using Uno.Extensions.Reactive;

namespace StockMarket;

public partial record StockMarketModel(IStockMarketService StockMarketService)
{
    public IListFeed<Stock> Stocks => ListFeed.AsyncEnumerable(StockMarketService.GetCurrentMarket);
}