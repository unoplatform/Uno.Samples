namespace QuoteCraft.Helpers;

/// <summary>
/// Where QuoteCraft keeps its local data: the SQLite database, photos, the logo, PDF exports and small flag files.
/// </summary>
public static class AppStorage
{
    /// <summary>
    /// The app's data folder. On WebAssembly only the ApplicationData folders are kept across page loads (IndexedDB),
    /// so the data lives under LocalFolder there; every other head keeps the folder it has always used.
    /// </summary>
    public static string Root { get; } =
#if DEBUG
        // Evidence runs (--qc-data=<folder>) keep their sample data away from the developer's own.
        VerificationArgs.Get("data") ??
#endif
#if __WASM__
        Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "QuoteCraft");
#else
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuoteCraft");
#endif

    /// <summary>
    /// Makes sure <see cref="Root"/> exists and, on WebAssembly, that the browser file system has been restored from
    /// IndexedDB. Await this before the first System.IO access.
    /// </summary>
    public static async Task EnsureReadyAsync()
    {
#if __WASM__
        // Any StorageFolder call waits for the IndexedDB-backed file system to load; plain System.IO does not.
        await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(
            "QuoteCraft", Windows.Storage.CreationCollisionOption.OpenIfExists);
#else
        await Task.CompletedTask;
#endif
        Directory.CreateDirectory(Root);
    }

    public static string Combine(params string[] parts) => Path.Combine([Root, .. parts]);
}
