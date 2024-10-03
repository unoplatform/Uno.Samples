using SQLite;

namespace ScottPlotDataPersistedSample;

public class DataService
{
    private SQLiteConnection _db;
    private static StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

    public async Task InitializeAsync()
    {
        // Ensure the filesystem is initialized properly including WebAssembly
        StorageFolder folder = await _localFolder.CreateFolderAsync("ScottPlotDatabase", CreationCollisionOption.OpenIfExists);

        string dbPath = Path.Combine(folder.Path, "seriesData.db");
        _db = new SQLiteConnection(dbPath);
        // Create table if it doesn't exist
        _db.CreateTable<Series>();
        // Create table for plot settings if it doesn't exist
        _db.CreateTable<PlotSettings>();
    }

    // Batch insert series asynchronously
    public async Task AddSeriesBatchAsync(List<Series> seriesList)
    {
        await Task.Run(() =>
        {
            _db.RunInTransaction(() =>
            {
                foreach (var series in seriesList)
                {
                    _db.Insert(series);
                }
            });
        });
    }

    // Get a batch of series asynchronously to avoid blocking the UI thread (lazy loading)
    public async Task<List<Series>> GetSeriesBatchAsync(int startIndex, int batchSize)
    {
        return await Task.Run(() =>
        {
            return _db.Table<Series>()
                .Skip(startIndex)
                .Take(batchSize)
                .ToList();
        });
    }

    // Clear all data asynchronously
    public async Task ClearAllDataAsync()
    {
        await Task.Run(() => _db.DeleteAll<Series>());
    }

    public void SaveLastUsedPlotType(string plotType)
    {
        var setting = _db.Table<PlotSettings>().FirstOrDefault();
        if (setting == null)
        {
            _db.Insert(new PlotSettings { LastUsedPlotType = plotType });
        }
        else
        {
            setting.LastUsedPlotType = plotType;
            _db.Update(setting);
        }
    }

    public string? GetLastUsedPlotType()
    {
        return _db.Table<PlotSettings>().FirstOrDefault()?.LastUsedPlotType;
    }
}
