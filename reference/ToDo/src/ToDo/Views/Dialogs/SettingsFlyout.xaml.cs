namespace ToDo.Views.Dialogs;

internal record ThemeChangedMessage(AppTheme Theme)
{
}

public sealed partial class SettingsFlyout : Flyout, IRecipient<ThemeChangedMessage>
{
	public SettingsFlyout()
	{
		this.InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
    }

    private void ThemeChipGroup_ItemClick(object sender, ChipItemEventArgs e)
    {
        if (FlyoutControl.DataContext is BindableSettingsViewModel viewModel)
        {
            viewModel.ChangeAppTheme.Execute(null);
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
