#if __IOS__ || __ANDROID__
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Samples;

/// <summary>
/// Allows interaction with the device's media gallery.
/// </summary>
public static partial class MediaGallery
{
    /// <summary>
    /// Checks the user's permission to access the device's gallery.
    /// Will trigger the permission request if not already granted.
    /// </summary>
    /// <returns>A value indicating whether the user has access.</returns>
    public static async Task<bool> CheckAccessAsync() => await CheckAccessAsyncImpl();

    /// <summary>
    /// Saves a media file to the device's gallery.
    /// </summary>
    /// <param name="type">Media file type.</param>
    /// <param name="data">Byte array representing the file.</param>
    /// <param name="targetFileName">Target file name.</param>
    /// <returns>Task representing the progress of the operation.</returns>
    public static async Task SaveAsync(MediaFileType type, byte[] data, string targetFileName)
    {
        using var memoryStream = new MemoryStream(data);
        await SaveAsync(type, memoryStream, targetFileName);
    }

    /// <summary>
    /// Saves a media file to the device's gallery.
    /// </summary>
    /// <param name="type">Media file type.</param>
    /// <param name="stream">Stream representing the file.</param>
    /// <param name="targetFileName">Target file name.</param>
    /// <returns>Task representing the progress of the operation.</returns>
    public static async Task SaveAsync(MediaFileType type, Stream stream, string targetFileName) =>
        await SavePlatformAsyncImpl(type, stream, targetFileName);
}
#endif
