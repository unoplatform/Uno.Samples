using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TelerikApp.Presentation;

internal partial class FinancialChartSampleViewModel : ObservableObject
{
    public FinancialChartSampleViewModel()
    {
        IndicatorsList = Enum.GetValues(typeof(Indicators)).Cast<Indicators>().ToList();
        TrendlinesList = Enum.GetValues(typeof(Trendlines)).Cast<Trendlines>().ToList();
        SeriesData = LoadDataFromJsonFile();
    }

    [ObservableProperty]
    private List<Indicators> indicatorsList;

    [ObservableProperty]
    private List<Trendlines> trendlinesList;

    [ObservableProperty]
    private FinancialDataItem[] seriesData;

    private FinancialDataItem[] LoadDataFromJsonFile()
    {
        var assembly = GetType().Assembly;
        var resourceName = assembly.GetManifestResourceNames().First(x => x.EndsWith("AppleStockPrices.json"));
        var stream = assembly.GetManifestResourceStream(resourceName);

        using var reader = new StreamReader(stream!);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize(json, FinancialDataJsonContext.Default.FinancialDataItemArray) ?? Array.Empty<FinancialDataItem>();
    }
}

// Source-generated metadata: reflection-based System.Text.Json serialization is disabled in trimmed builds (WebAssembly).
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(FinancialDataItem[]))]
internal partial class FinancialDataJsonContext : JsonSerializerContext
{
}
