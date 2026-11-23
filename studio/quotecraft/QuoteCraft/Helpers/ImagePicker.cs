using Windows.Storage.Pickers;

namespace QuoteCraft.Helpers;

/// <summary>Picks one image file. The picker must open on the UI thread; MVUX runs commands off it.</summary>
public static class ImagePicker
{
    public static async Task<StorageFile?> PickAsync(IDispatcher dispatcher, CancellationToken ct) =>
        await dispatcher.ExecuteAsync<StorageFile?>(async _ =>
        {
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            foreach (var type in new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" })
            {
                picker.FileTypeFilter.Add(type);
            }
#if WINDOWS
            // WinAppSDK: the picker needs its owner window.
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow));
#endif
            return await picker.PickSingleFileAsync();
        }, ct);
}
