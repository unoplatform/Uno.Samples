namespace QuoteCraft.Services;

public interface IPhotoService
{
    string GetPhotoDirectory(string quoteId);
    List<string> GetPhotos(string quoteId);
    Task<string?> AddPhotoAsync(string quoteId, Stream source, string extension);
    void DeletePhoto(string photoPath);
    void DeletePhotosForQuote(string quoteId);
    int MaxPhotos => 5;
}

public class PhotoService : IPhotoService
{
    private static readonly string BaseDir = AppStorage.Combine("photos");

    public int MaxPhotos => 5;

    public string GetPhotoDirectory(string quoteId)
    {
        var dir = Path.Combine(BaseDir, quoteId);
        Directory.CreateDirectory(dir);
        return dir;
    }

    public List<string> GetPhotos(string quoteId)
    {
        var dir = Path.Combine(BaseDir, quoteId);
        if (!Directory.Exists(dir))
            return [];

        return Directory.GetFiles(dir, "*.*")
            .Where(f => IsImageFile(f))
            .OrderBy(f => File.GetCreationTimeUtc(f))
            .ToList();
    }

    private const long MaxFileSizeBytes = 1_048_576; // 1 MB

    /// <summary>
    /// Copies a picked image into the quote's photo folder. Takes a stream because a picked file's path is not a
    /// readable file system path on every head (WebAssembly, Android, iOS).
    /// </summary>
    public async Task<string?> AddPhotoAsync(string quoteId, Stream source, string extension)
    {
        var existing = GetPhotos(quoteId);
        if (existing.Count >= MaxPhotos)
            return null;

        var dir = GetPhotoDirectory(quoteId);
        var ext = extension.ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var destPath = Path.Combine(dir, fileName);

        await using (var destination = File.Create(destPath))
        {
            await source.CopyToAsync(destination);
        }

        await CompressIfNeededAsync(destPath);

        return destPath;
    }

    /// <summary>
    /// Compresses the image to max 1 MB by resizing and reducing JPEG quality.
    /// Uses SkiaSharp which is available via Uno Platform's Skia renderer.
    /// </summary>
    private static async Task CompressIfNeededAsync(string filePath)
    {
        await Task.Run(() =>
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length <= MaxFileSizeBytes)
                return;

            try
            {
                using var inputStream = File.OpenRead(filePath);
                using var original = SkiaSharp.SKBitmap.Decode(inputStream);
                if (original is null) return;

                // Calculate scale factor to bring file under 1 MB
                // Rough estimate: reduce dimensions proportionally
                var scale = Math.Sqrt((double)MaxFileSizeBytes / fileInfo.Length) * 0.9;
                var newWidth = Math.Max(1, (int)(original.Width * scale));
                var newHeight = Math.Max(1, (int)(original.Height * scale));

                using var resized = original.Resize(
                    new SkiaSharp.SKImageInfo(newWidth, newHeight),
                    new SkiaSharp.SKSamplingOptions(SkiaSharp.SKFilterMode.Linear, SkiaSharp.SKMipmapMode.Linear));
                if (resized is null) return;

                using var image = SkiaSharp.SKImage.FromBitmap(resized);
                using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80);
                using var output = File.OpenWrite(filePath);
                output.SetLength(0);
                data.SaveTo(output);
            }
            catch (Exception ex)
            {
                // The photo stays as it was added, only larger than the 1 MB aim.
                ErrorReporter.Log(ex, "Compressing a photo failed; the original is kept");
            }
        });
    }

    public void DeletePhoto(string photoPath)
    {
        if (File.Exists(photoPath))
            File.Delete(photoPath);
    }

    public void DeletePhotosForQuote(string quoteId)
    {
        var dir = Path.Combine(BaseDir, quoteId);
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
    }

    private static bool IsImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".webp";
    }
}
