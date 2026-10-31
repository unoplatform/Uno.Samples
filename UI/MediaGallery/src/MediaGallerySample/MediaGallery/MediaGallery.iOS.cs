#if __IOS__
using Foundation;
using Photos;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Uno.Samples;

public partial class MediaGallery
{
    private static readonly string _requiredInfoPlistKey = CheckSystemVersion(14) ? "NSPhotoLibraryAddUsageDescription" : "NSPhotoLibraryUsageDescription";

    private static async Task<bool> CheckAccessAsyncImpl()
    {
        if (!NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(_requiredInfoPlistKey)))
        {
            throw new InvalidOperationException($"The Info.plist file is missing the required key '{_requiredInfoPlistKey}'.");
        }

        var authorizationStatus = CheckSystemVersion(14) ?
            PHPhotoLibrary.GetAuthorizationStatus(PHAccessLevel.AddOnly) :
#pragma warning disable CA1422 // Deprecated API
            PHPhotoLibrary.AuthorizationStatus;
#pragma warning restore CA1422 // Deprecated API

        if (!IsAuthorized(authorizationStatus))
        {
            authorizationStatus = CheckSystemVersion(14) ?
                await PHPhotoLibrary.RequestAuthorizationAsync(PHAccessLevel.AddOnly) :
#pragma warning disable CA1422 // Deprecated API
                await PHPhotoLibrary.RequestAuthorizationAsync();
#pragma warning restore CA1422 // Deprecated API
        }

        return IsAuthorized(authorizationStatus);
    }

    private static async Task SavePlatformAsyncImpl(MediaFileType type, Stream sourceStream, string targetFileName)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), targetFileName);
        try
        {
            // Write stream copy to temp
            using var fileStream = File.Create(tempFile);
            await sourceStream.CopyToAsync(fileStream);

            // get the file uri
            var fileUri = new NSUrl(tempFile);

            await PhotoLibraryPerformChanges(() =>
            {
                using var request = type == MediaFileType.Image ?
                    PHAssetChangeRequest.FromImage(fileUri) :
                    PHAssetChangeRequest.FromVideo(fileUri);
            }).ConfigureAwait(false);
        }
        finally
        {
            // Attempt to delete the temp file
            File.Delete(tempFile);
        }
    }

    static async Task PhotoLibraryPerformChanges(Action action)
    {
        var tcs = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);

        PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
            () =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(ex);
                }
            },
            (success, error) =>
                tcs.TrySetResult(error is not null ? new NSErrorException(error) : null));

        var exception = await tcs.Task;
        if (exception is not null)
        {
            throw exception;
        }
    }

    private static bool CheckSystemVersion(int major) => UIDevice.CurrentDevice.CheckSystemVersion(major, 0);

    private static bool IsAuthorized(PHAuthorizationStatus status) => status is PHAuthorizationStatus.Authorized or PHAuthorizationStatus.Limited;
}
#endif
