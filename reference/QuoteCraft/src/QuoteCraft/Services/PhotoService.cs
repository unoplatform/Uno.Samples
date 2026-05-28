namespace QuoteCraft.Services;

public interface IPhotoService
{
    string GetPhotoDirectory(string quoteId);
    List<string> GetPhotos(string quoteId);
    Task<string?> AddPhotoAsync(string quoteId, string sourceFilePath);
    void DeletePhoto(string photoPath);
    void DeletePhotosForQuote(string quoteId);
    int MaxPhotos => 5;
}

public class PhotoService : IPhotoService
{
    private static readonly string BaseDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "QuoteCraft", "photos");

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

    public async Task<string?> AddPhotoAsync(string quoteId, string sourceFilePath)
    {
        var existing = GetPhotos(quoteId);
        if (existing.Count >= MaxPhotos)
            return null;

        var dir = GetPhotoDirectory(quoteId);
        var ext = Path.GetExtension(sourceFilePath).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var destPath = Path.Combine(dir, fileName);

        await Task.Run(() => File.Copy(sourceFilePath, destPath, overwrite: true));

        // Compress if file exceeds 1 MB
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

                using var resized = original.Resize(new SkiaSharp.SKImageInfo(newWidth, newHeight), SkiaSharp.SKFilterQuality.Medium);
                if (resized is null) return;

                using var image = SkiaSharp.SKImage.FromBitmap(resized);
                using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80);
                using var output = File.OpenWrite(filePath);
                output.SetLength(0);
                data.SaveTo(output);
            }
            catch
            {
                // If compression fails, keep the original file
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
