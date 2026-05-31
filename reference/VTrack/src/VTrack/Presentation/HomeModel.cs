namespace VTrack.Presentation;

public partial record HomeModel
{
    private readonly INavigator _navigator;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(INavigator navigator, ILogger<HomeModel> logger)
    {
        _navigator = navigator;
        _logger = logger;
    }

    public IState<bool> IsUploading => State<bool>.Value(this, () => false);
    public IState<string> UploadFileName => State<string>.Value(this, () => string.Empty);
    public IState<double> UploadProgress => State<double>.Value(this, () => 0);

    public async ValueTask BrowseFiles(CancellationToken ct)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");

#if VTRACK_DESKTOP
            // The Skia desktop head must associate the picker with the window handle on Windows.
            // Other platforms (and non-Windows desktop) let Uno resolve the picker target itself.
            if (OperatingSystem.IsWindows() && App.MainWindow is { } window)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            }
#endif

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await ProcessVideoFile(file, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error picking file");
        }
    }

    public async Task HandleDroppedFile(StorageFile file)
    {
        await ProcessVideoFile(file, CancellationToken.None);
    }

    private async Task ProcessVideoFile(StorageFile file, CancellationToken ct)
    {
        await IsUploading.SetAsync(true, ct);
        await UploadFileName.SetAsync(file.Name, ct);
        await UploadProgress.SetAsync(0, ct);

        try
        {
            for (int i = 0; i <= 100; i += 10)
            {
                await UploadProgress.SetAsync(i, ct);
                await Task.Delay(100, ct);
            }

            var videoFile = new VideoFile(
                Id: Guid.NewGuid().ToString(),
                Name: file.Name,
                Duration: 0,
                ThumbnailUrl: null,
                VideoUrl: file.Path,
                UploadedAt: DateTime.UtcNow);

            await _navigator.NavigateViewModelAsync<VideoAnalysisModel>(this, data: videoFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video file");
        }
        finally
        {
            await IsUploading.SetAsync(false, ct);
        }
    }
}
