namespace ToDo.Presentation;

public partial record LoginModel(IDispatcher Dispatcher, INavigator Navigator, IAuthenticationService Authentication)
{
    public string Title { get; } = "Login";

    public IState<string> Username => State<string>.Value(this, () => string.Empty);

    public IState<string> Password => State<string>.Value(this, () => string.Empty);

    public async ValueTask Login(CancellationToken token)
    {
        var username = await Username ?? string.Empty;
        var password = await Password ?? string.Empty;

        var success = await Authentication.LoginAsync(Dispatcher, new Dictionary<string, string> { { nameof(Username), username }, { nameof(Password), password } });
        if (success)
        {
            await Navigator.NavigateViewModelAsync<MainModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }

}
