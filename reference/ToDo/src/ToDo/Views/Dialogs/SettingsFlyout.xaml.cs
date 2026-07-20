namespace ToDo.Views.Dialogs;

internal record ThemeChangedMessage(AppTheme Theme)
{
}

public sealed partial class SettingsFlyout : Flyout, IRecipient<ThemeChangedMessage>
{
    private bool _isThemeInitialized = false;

    public SettingsFlyout()
    {
        this.InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
    }

    private void ThemeChipGroup_ItemChecked(object sender, ChipItemEventArgs e)
    {
        if (FlyoutRoot.DataContext is BindableSettingsViewModel viewModel)
        {
            if (_isThemeInitialized)
            {
                viewModel.ChangeAppTheme.Execute(e.Item);
            }
            _isThemeInitialized = true;
#if WINDOWS
            viewModel.ChangeAppTheme.Execute(e.Item);
#endif
        }
    }

    void IRecipient<ThemeChangedMessage>.Receive(ThemeChangedMessage message)
    {
        // Workaround for https://github.com/unoplatform/uno/issues/15287
#if WINDOWS
        _ = DispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                FlyoutRoot.RequestedTheme = message.Theme == AppTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
            }
            catch (Exception)
            {
            }
        });
#endif
    }
}
