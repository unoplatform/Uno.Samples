namespace QuoteCraft.Presentation;

/// <summary>
/// Binds the Settings page's parts to their own models.
/// </summary>
/// <remarks>
/// MVUX generates one view model per model, and a model's property reaches the view as the plain record, whose states
/// a binding cannot read. The page composes two models of its own (<see cref="AccountSettingsModel"/> and
/// <see cref="ThemeSettingsModel"/>, from the container); these additions give each its generated view model over the
/// very instance the page holds, as the DataContext of its sections, and dispose of them with the page. Each addition
/// opts out of bindable generation: its name ends in "Model", so MVUX would otherwise take it for a model.
/// </remarks>
[ReactiveBindable(false)]
public partial class SettingsViewModel
{
    private AccountSettingsViewModel? _account;
    private ThemeSettingsViewModel? _appearance;

    /// <summary>The DataContext of the upgrade banner and the account and billing sections.</summary>
    public AccountSettingsViewModel Account => _account ??= Owned(AccountSettingsViewModel.Over(Model.AccountSettings));

    /// <summary>The DataContext of the Appearance row.</summary>
    public ThemeSettingsViewModel Appearance => _appearance ??= Owned(ThemeSettingsViewModel.Over(Model.ThemeSettings));

    private T Owned<T>(T part)
        where T : IAsyncDisposable
    {
        RegisterDisposable(part);
        return part;
    }
}

[ReactiveBindable(false)]
public partial class AccountSettingsViewModel
{
    /// <summary>A view model over the account part the page already holds.</summary>
    internal static AccountSettingsViewModel Over(AccountSettingsModel model) => new(model);
}

[ReactiveBindable(false)]
public partial class ThemeSettingsViewModel
{
    /// <summary>A view model over the theme part the page already holds.</summary>
    internal static ThemeSettingsViewModel Over(ThemeSettingsModel model) => new(model);
}
