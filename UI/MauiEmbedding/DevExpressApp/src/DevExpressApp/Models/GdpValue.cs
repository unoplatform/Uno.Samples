namespace DevExpressApp.Models;

public class GdpValue
{
    public DateTime Year { get; }
    public double Value { get; }

    public GdpValue(DateTime year, double value)
    {
        this.Year = year;
        this.Value = value;
    }
}
