namespace QuoteCraft.Services;

/// <summary>The business logo's copy in the app's data folder: Settings' Upload puts it there and Remove deletes it.</summary>
public sealed class LogoStore
{
    /// <summary>
    /// Copies an image in as the logo and returns the copy's path. It takes a stream: a picked file's path is not a
    /// readable file system path on every head (WebAssembly, Android, iOS).
    /// </summary>
    public async Task<string> SaveAsync(Stream image, string extension)
    {
        var folder = AppStorage.Combine("logo");
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, $"logo{extension}");
        await using (var copy = File.Create(path))
        {
            await image.CopyToAsync(copy);
        }
        return path;
    }

    /// <summary>Deletes the logo's copy, when there is one.</summary>
    public void Delete(string? path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
