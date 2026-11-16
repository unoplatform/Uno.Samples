using Windows.Storage.Pickers;

namespace Http.KiotaExtensionsDemo.Services;

/// <summary>
/// Opens the system file picker. MVUX commands run on a background thread and the picker must be
/// shown from the UI thread, so this goes through the window's dispatcher.
/// </summary>
public sealed class FilePicker(IDispatcher dispatcher)
{
    /// <returns>The picked file read into memory, or null when the user cancelled.</returns>
    public async ValueTask<PickedFile?> PickAsync(CancellationToken ct)
    {
        var file = await dispatcher.ExecuteAsync(async _ =>
        {
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.Add("*");
            return await picker.PickSingleFileAsync();
        }, ct);

        if (file is null)
        {
            return null;
        }

        using var stream = await file.OpenStreamForReadAsync();
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, ct);
        return new PickedFile(file.Name, file.ContentType, buffer.ToArray());
    }
}
