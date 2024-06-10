namespace DevExpressApp.Models;
public class CountryGdp
{
    public string CountryName { get; }
    public IList<GdpValue> Values { get; }

    public CountryGdp(string country, params GdpValue[] values)
    {
        this.CountryName = country;
        this.Values = new List<GdpValue>(values);
    }
}
