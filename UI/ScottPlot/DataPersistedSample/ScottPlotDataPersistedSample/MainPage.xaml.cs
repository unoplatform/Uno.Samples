// This sample application uses SQLite to persist large datasets and ScottPlot for data visualization.
// The following plot types were selected based on their performance characteristics with large datasets:
//
// - SignalPlot and SignalConst: These plot types are the most efficient for large datasets, especially when X-values are evenly spaced, due to their memory optimization in ScottPlot.
// - ScatterPlot (with downsampling): This plot type is ideal for non-uniform X-values, as downsampling helps to optimize performance by reducing the number of points plotted.
// - Heatmap: Suitable for visualizing data in grid or image form, typically used for 2D data where performance is acceptable.
//
// Other plot types were excluded in this sample because they may struggle with performance when handling large datasets, as noted in ScottPlot's documentation.
// The chosen types are intended to balance performance and visualization quality, especially in platforms like WebAssembly where resource limitations may apply.

using Microsoft.UI.Dispatching;

namespace ScottPlotDataPersistedSample;

public sealed partial class MainPage : Page
{

    // Data-related fields
    private DataService _dataService;
    private readonly Random _random = new();
    private readonly List<Series> _cachedSeries = new(); // Caches the series data in memory
    private readonly object _seriesLock = new(); // Object for locking the _cachedSeries list for thread safety

    // Plot-related fields
    private string currentPlotType = "SignalPlot"; // Default chart type if none found
    private readonly string[] _plotTypes = { "SignalPlot", "SignalConst", "Heatmap", "ScatterDownsample" };

    // UI-related fields
    private readonly DispatcherQueue _dispatcherQueue; // Dispatcher for UI updates

    // State-tracking fields
    private bool _isInitialized = false; // Track if the filesystem and database have been initialized
    private const int _batchSize = 5; // Lazy loading: number of series to load at once
    private int _currentBatchIndex = 0; // Track the current loading index for lazy loading
    private int _currentChartTypeIndex = 0; // Track the current index for chart cycling

    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
        // Get the UI dispatcher
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    // Immediate feedback during initialization
    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Provide feedback while loading inital data
        await UpdateUIStatusAsync("Loading initial data, please wait...");
        try
        {
            // Initialize the database and wait for the filesystem to be ready
            await InitializeDatabaseAsync();

            // Load last used plot type
            currentPlotType = _dataService.GetLastUsedPlotType() ?? currentPlotType;

            // Start lazy loading of data
            await LoadNextDataBatchAsync();  // Load the first batch of data
            _isInitialized = true;  // Mark initialization as complete

            // Initialize plot and update the status text with correct information
            await InitializePlotAsync();

            // Update the status text after data is loaded
            UpdateStatusText(_cachedSeries.Sum(s => s.DataPoints.Length));
        }
        catch (Exception ex)
        {
            await UpdateUIStatusAsync($"Initialization failed: {ex.Message}");
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        _dataService = new DataService();
        // Ensure async initialization for WebAssembly compatibility as well
        await _dataService.InitializeAsync();
    }

    // Lazy load the next batch of data
    private async Task LoadNextDataBatchAsync()
    {
        List<Series> nextBatch;
        do
        {
            // Retrieve the next batch of data
            nextBatch = await _dataService.GetSeriesBatchAsync(_currentBatchIndex, _batchSize);

            // Lock the cached series during updates to prevent concurrency issues
            lock (_seriesLock)
            {
                _cachedSeries.AddRange(nextBatch);
            }

            _currentBatchIndex += nextBatch.Count;

            // Provide feedback after loading each batch
            await UpdateUIStatusAsync(nextBatch.Count > 0
                ? $"Loaded {_cachedSeries.Count} series so far..."
                : $"All series loaded. Total: {_cachedSeries.Count} series.");

        } while (nextBatch.Count == _batchSize); // Continue loading if the current batch size matches the threshold
    }

    // Asynchronous plotting to avoid blocking the UI thread
    private async Task InitializePlotAsync()
    {
        if (!_isInitialized)
        {
            await UpdateUIStatusAsync("Initialization in progress. Please wait.");
            return;
        }

        WinUIPlot1.Plot.Title("Multi-Series Random Walk");
        WinUIPlot1.Plot.XLabel("Array Index");
        WinUIPlot1.Plot.YLabel("Random Value");

        // Plot data asynchronously to avoid UI freezing
        await PlotDataAsync();
    }

    // Asynchronous method to plot data
    private async Task PlotDataAsync()
    {
        await Task.Run(() =>
        {
            // Lock the collection during the plotting process
            lock (_seriesLock)
            {
                // Create a local copy to avoid concurrency issues
                var localSeriesList = _cachedSeries.ToList();

                // Marshal plot updates to the main thread
                _dispatcherQueue.TryEnqueue(() =>
                {

                    // Only update the plot once after adding all the series
                    WinUIPlot1.Plot.Clear();
                    var palette = new ScottPlot.Palettes.Category10();

                    foreach (var series in localSeriesList)
                    {
                        switch (currentPlotType)
                        {
                            case "SignalPlot":
                                var signalPlot = WinUIPlot1.Plot.Add.Signal(series.DataPoints);
                                signalPlot.Color = palette.GetColor(localSeriesList.IndexOf(series));
                                break;

                            case "SignalConst":
                                var signalConstPlot = WinUIPlot1.Plot.Add.SignalConst(series.DataPoints);
                                signalConstPlot.LineWidth = 2;
                                signalConstPlot.Color = palette.GetColor(localSeriesList.IndexOf(series));
                                break;

                            case "Heatmap":
                                if (localSeriesList.Count > 0)
                                {
                                    double[,] heatmapData = GenerateHeatmapData(series.DataPoints);
                                    WinUIPlot1.Plot.Add.Heatmap(heatmapData);
                                }
                                break;

                            case "ScatterDownsample":
                                var xs = Enumerable.Range(0, series.DataPoints.Length).Select(x => (double)x).ToArray();
                                var scatterPlot = WinUIPlot1.Plot.Add.Scatter(xs, series.DataPoints);
                                scatterPlot.Color = palette.GetColor(localSeriesList.IndexOf(series));
                                break;
                        }
                    }

                    // Auto scale the axes after all series are added
                    WinUIPlot1.Plot.Axes.AutoScale();

                    // Force the plot to refresh visually after plotting
                    WinUIPlot1.Refresh();
                });
            }
        });
    }

    // Add new random data and refresh the plot asynchronously
    private async void AddRandomDataButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
        {
            await UpdateUIStatusAsync("Initialization in progress. Please wait.");
            return;
        }

        // Generate a new random walk starting at the current total data points
        var newSeries = GenerateRandomWalk(100000, _cachedSeries.Sum(s => s.DataPoints.Length));
        lock (_seriesLock)
        {
            // Add to cached series
            _cachedSeries.Add(newSeries);
        }

        // Batch insert to database for efficiency
        await _dataService.AddSeriesBatchAsync(new List<Series> { newSeries });

        // Plot data asynchronously and update status text with the new total points
        await PlotDataAsync();

        // Update status after adding new data
        UpdateStatusText(_cachedSeries.Sum(s => s.DataPoints.Length));

        // Force garbage collection to ensure unused data from previous plots or operations is cleaned up.
        // This is useful when working with large datasets here to avoid memory bloat.
        GC.Collect();
    }

    // Clear the plot and data asynchronously, followed by a forced GC collection
    private async void ClearPlotButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
        {
            await UpdateUIStatusAsync("Initialization in progress. Please wait.");
            return;
        }

        lock (_seriesLock)
        {
            // Clear cache
            _cachedSeries.Clear();
        }

        // Clear database
        await _dataService.ClearAllDataAsync();

        // Clear the plot asynchronously
        await PlotDataAsync();
        // Update status after clearing data
        UpdateStatusText(0);

        // Force garbage collection to ensure unused data from previous plots or operations is cleaned up.
        // This is useful when clearing large datasets here to free up memory.
        GC.Collect();
    }

    // Change chart type and refresh plot asynchronously
    private async void ChangeChartTypeButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
        {
            await UpdateUIStatusAsync("Initialization in progress. Please wait.");
            return;
        }

        // Cycle through plot types
        _currentChartTypeIndex = (_currentChartTypeIndex + 1) % _plotTypes.Length;
        currentPlotType = _plotTypes[_currentChartTypeIndex];

        // Save the selected plot type in the database
        _dataService.SaveLastUsedPlotType(currentPlotType);

        // Plot data asynchronously and update status text with the new chart type
        await PlotDataAsync();
        // Update status after changing chart type
        UpdateStatusText(_cachedSeries.Sum(s => s.DataPoints.Length));
    }

    // Update the status text with total points and the current chart type
    private void UpdateStatusText(int totalPoints)
    {
        // Marshaling UI updates back to the main thread
        _dispatcherQueue.TryEnqueue(() =>
        {
            StatusTextBlock.Text = $"Total data points: {totalPoints:N0} | Current Chart Type: {currentPlotType}";
        });
    }

    // Helper method to update UI status with proper thread dispatching
    private async Task UpdateUIStatusAsync(string message)
    {
        await Task.Run(() =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusTextBlock.Text = message;
            });
        });
    }

    // Generates a random walk series of 'length' data points starting from 'origin'
    private Series GenerateRandomWalk(int length, double origin)
    {
        double[] data = new double[length];
        double value = 0;

        for (int i = 0; i < length; i++)
        {
            data[i] = value += _random.NextDouble() * 2 - 1;
        }

        return new Series
        {
            DataPoints = data,
            Origin = origin
        };
    }

    // Generate heatmap data from a series' data points
    private double[,] GenerateHeatmapData(double[] dataPoints)
    {
        int size = (int)Math.Sqrt(dataPoints.Length);
        double[,] heatmap = new double[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                heatmap[i, j] = dataPoints[i * size + j];
            }
        }

        return heatmap;
    }
}
