#if __ANDROID__
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Extensions;
using Path = System.IO.Path;
using Stream = System.IO.Stream;
using Environment = Android.OS.Environment;
using NativeFile = Java.IO.File;
using NativeUri = Android.Net.Uri;
using static Android.Provider.MediaStore;

namespace Uno.Samples;

partial class MediaGallery
{
    private static async Task<bool> CheckAccessAsyncImpl()
    {
        if ((int)Build.VERSION.SdkInt < 29)
        {
            return await PermissionsHelper.CheckWriteExternalStoragePermission(default);
        }
        else
        {
            return true;
        }
    }

    private static async Task SavePlatformAsyncImpl(MediaFileType type, Stream sourceStream, string targetFileName)
    {
        var context = Android.App.Application.Context;
        var contentResolver = context.ContentResolver ?? throw new InvalidOperationException("ContentResolver is not set.");

        var appFolderName = Package.Current.DisplayName;
        // Ensure folder name is file system safe
        appFolderName = string.Join("_", appFolderName.Split(Path.GetInvalidFileNameChars()));

        var dateTimeNow = DateTime.Now;

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetFileName);
        var extension = Path.GetExtension(targetFileName).ToLower();

        using var values = new ContentValues();

        values.Put(IMediaColumns.DateAdded, GetUnixTimestampInSeconds(dateTimeNow));
        values.Put(IMediaColumns.Title, fileNameWithoutExtension);
        values.Put(IMediaColumns.DisplayName, targetFileName);

        var mimeTypeMap = MimeTypeMap.Singleton ?? throw new InvalidOperationException("MimeTypeMap is not set.");

        var mimeType = mimeTypeMap.GetMimeTypeFromExtension(extension.Replace(".", string.Empty));
        if (!string.IsNullOrWhiteSpace(mimeType))
            values.Put(IMediaColumns.MimeType, mimeType);

        using var externalContentUri = type == MediaFileType.Image
            ? Images.Media.ExternalContentUri
            : Video.Media.ExternalContentUri;

        if (externalContentUri is null)
        {
            throw new InvalidOperationException($"External Content URI for {type} is not available.");
        }

        var relativePath = type == MediaFileType.Image
            ? Environment.DirectoryPictures
            : Environment.DirectoryMovies;

        if (relativePath is null)
        {
            throw new InvalidOperationException($"Relative path for {type} is not available.");
        }

        if ((int)Build.VERSION.SdkInt >= 29)
        {
            values.Put(IMediaColumns.RelativePath, Path.Combine(relativePath, appFolderName));
            values.Put(IMediaColumns.IsPending, true);

            using var uri = contentResolver.Insert(externalContentUri, values) ??
                throw new InvalidOperationException("Could not generate new content URI");

            using var stream = contentResolver.OpenOutputStream(uri) ??
                throw new InvalidOperationException("Could not open output stream");

            await sourceStream.CopyToAsync(stream);
            stream.Close();

            values.Put(IMediaColumns.IsPending, false);
            context.ContentResolver.Update(uri, values, null, null);
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            using var directory = new NativeFile(Environment.GetExternalStoragePublicDirectory(relativePath), appFolderName);
            directory.Mkdirs();

            using var file = new NativeFile(directory, targetFileName);

            using var fileOutputStream = System.IO.File.Create(file.AbsolutePath);
            await sourceStream.CopyToAsync(fileOutputStream);
            fileOutputStream.Close();

            values.Put(IMediaColumns.Data, file.AbsolutePath);
            contentResolver.Insert(externalContentUri, values);

#pragma warning disable CA1422 // Validate platform compatibility
            using var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
#pragma warning restore CA1422 // Validate platform compatibility
            mediaScanIntent.SetData(NativeUri.FromFile(file));
            context.SendBroadcast(mediaScanIntent);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    private static long GetUnixTimestampInSeconds(DateTime current) => (long)GetTimeDifference(current).TotalSeconds;

    private static TimeSpan GetTimeDifference(DateTime current) => current.ToUniversalTime() - DateTime.UnixEpoch;
}
#endif
