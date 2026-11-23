namespace QuoteCraft.Helpers;

/// <summary>
/// A dialog takes the window's theme. The theme service sets Light or Dark on the window's content, and a dialog opens
/// in a popup outside it, so without this a dialog kept the system's theme while the app showed the other. The dialog
/// takes the content's requested theme as it is built, before its template resolves its brushes: Light or Dark when
/// the user chose one, Default when the app follows the system, so an open dialog still follows a system change. Its
/// surface follows through AppDialogStyle (App.xaml).
/// </summary>
public static class DialogTheme
{
    /// <summary>Call from a dialog's constructor (a dialog route builds a new dialog each time it opens).</summary>
    public static void FollowWindow(ContentDialog dialog)
    {
        if (App.MainAppWindow.Content is FrameworkElement root)
        {
            dialog.RequestedTheme = root.RequestedTheme;
        }
    }
}
