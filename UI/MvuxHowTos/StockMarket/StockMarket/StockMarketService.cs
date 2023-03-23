using System.Runtime.CompilerServices;

namespace StockMarket;

public partial record Stock(string Name, double Value);

public class StockMarketService
{
    public async IAsyncEnumerable<IImmutableList<Stock>> GetCurrentMarket(
        [EnumeratorCancellation] CancellationToken ct)
    {
        var rnd = new Random();

        while (!ct.IsCancellationRequested)
        {
            // return current stock-market info
            yield return _stocks.ToImmutableList();

            // this delays the next iteration by 5 seconds
            await Task.Delay(TimeSpan.FromSeconds(5), ct);

            // this updates the market prices
            // in a more realistic program
            // this would have taken place on the remote server
            for (int i = 0; i < _stocks.Count; i++)
            {
                var stock = _stocks[i];
                var increment = rnd.NextDouble();

                _stocks[i] = stock with { Value = stock.Value + increment };
            }
        }
    }

    // this list is for the purpose of this demonstration
    // we're treating this variable as our database
    // ideally a service doesn't hold the data just requests it
    private readonly List<Stock> _stocks = new List<Stock>
    {
        new Stock("MSFT", 279.35),
        new Stock("GOOG", 102.11),
    };
}
