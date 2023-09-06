using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DevExpressApp.MauiControls;

public partial class ChartControl : ScrollView
{
    public ChartControl()
    {
        InitializeComponent();
    }
}

public class ViewModel
{
    public CountryGdp GdpValueForUSA { get; }
    public CountryGdp GdpValueForChina { get; }
    public CountryGdp GdpValueForJapan { get; }

    public ViewModel()
    {
        GdpValueForUSA = new CountryGdp(
            "USA",
            new GdpValue(new DateTime(2020, 1, 1), 20.93),
            new GdpValue(new DateTime(2019, 1, 1), 21.43),
            new GdpValue(new DateTime(2018, 1, 1), 20.58),
            new GdpValue(new DateTime(2017, 1, 1), 19.391),
            new GdpValue(new DateTime(2016, 1, 1), 18.624),
            new GdpValue(new DateTime(2015, 1, 1), 18.121),
            new GdpValue(new DateTime(2014, 1, 1), 17.428),
            new GdpValue(new DateTime(2013, 1, 1), 16.692),
            new GdpValue(new DateTime(2012, 1, 1), 16.155),
            new GdpValue(new DateTime(2011, 1, 1), 15.518),
            new GdpValue(new DateTime(2010, 1, 1), 14.964)
        );
        GdpValueForChina = new CountryGdp(
            "China",
            new GdpValue(new DateTime(2020, 1, 1), 14.72),
            new GdpValue(new DateTime(2019, 1, 1), 14.34),
            new GdpValue(new DateTime(2018, 1, 1), 13.89),
            new GdpValue(new DateTime(2017, 1, 1), 12.238),
            new GdpValue(new DateTime(2016, 1, 1), 11.191),
            new GdpValue(new DateTime(2015, 1, 1), 11.065),
            new GdpValue(new DateTime(2014, 1, 1), 10.482),
            new GdpValue(new DateTime(2013, 1, 1), 9.607),
            new GdpValue(new DateTime(2012, 1, 1), 8.561),
            new GdpValue(new DateTime(2011, 1, 1), 7.573),
            new GdpValue(new DateTime(2010, 1, 1), 6.101)
        );
        GdpValueForJapan = new CountryGdp(
            "Japan",
            new GdpValue(new DateTime(2020, 1, 1), 4.888),
            new GdpValue(new DateTime(2019, 1, 1), 5.082),
            new GdpValue(new DateTime(2018, 1, 1), 4.955),
            new GdpValue(new DateTime(2017, 1, 1), 4.872),
            new GdpValue(new DateTime(2016, 1, 1), 4.949),
            new GdpValue(new DateTime(2015, 1, 1), 4.395),
            new GdpValue(new DateTime(2014, 1, 1), 4.850),
            new GdpValue(new DateTime(2013, 1, 1), 5.156),
            new GdpValue(new DateTime(2012, 1, 1), 6.203),
            new GdpValue(new DateTime(2011, 1, 1), 6.156),
            new GdpValue(new DateTime(2010, 1, 1), 5.700)
        );
    }
}

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
