namespace QuoteCraft.Helpers;

/// <summary>The app's dialogs, opened from models through navigation ("!" routes registered in App.xaml.cs).</summary>
public static class DialogNavigation
{
    /// <summary>Shows a <see cref="ConfirmRequest"/>; true when the user pressed its primary button.</summary>
    public static async Task<bool> ConfirmAsync(this INavigator navigator, object sender, ConfirmRequest request, CancellationToken ct) =>
        await navigator.GetDataAsync<ContentDialogResult>(sender, "Confirm", Qualifiers.Dialog, request, ct) == ContentDialogResult.Primary;

    /// <summary>
    /// A user action run from a model command: on failure, log it and say in a dialog what did not happen
    /// (<paramref name="failure"/>, as a sentence), with the reason when the app wrote it for people.
    /// </summary>
    public static async ValueTask RunAsync(this INavigator navigator, object sender, string failure, Func<Task> action, CancellationToken ct)
    {
        try
        {
            await action();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorReporter.Log(ex, failure);
            var reason = ex is InvalidOperationException { Message.Length: > 0 }
                ? ex.Message
                : "Try again. If it keeps happening, restart QuoteCraft.";
            try
            {
                await navigator.ConfirmAsync(sender, new ConfirmRequest("\uE783", failure, reason), ct);
            }
            catch (Exception dialogError)
            {
                ErrorReporter.Log(dialogError, "The error dialog could not open");
            }
        }
    }

    /// <summary>Shows a job photo enlarged (<see cref="PhotoViewRequest"/>) until the user closes it.</summary>
    public static Task ShowPhotoAsync(this INavigator navigator, object sender, string path, CancellationToken ct) =>
        navigator.GetDataAsync<ContentDialogResult>(sender, "PhotoViewer", Qualifiers.Dialog, new PhotoViewRequest(path), ct);

    /// <summary>Opens a dialog route and waits for the result its model sends back; null when it was cancelled.</summary>
    public static Task<TResult?> ShowDialogAsync<TResult>(this INavigator navigator, object sender, string route, object? data, CancellationToken ct) =>
        navigator.GetDataAsync<TResult>(sender, route, Qualifiers.Dialog, data, ct);
}
