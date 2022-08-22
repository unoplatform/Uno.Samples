using System.Windows.Input;
using Uno.Extensions.Authentication;

namespace Commerce.ViewModels;

public partial record LoginViewModel
{
    private readonly INavigator _navigator;
    private readonly IAuthenticationService _authenticationService;


    private LoginViewModel(
        INavigator navigator,
        IOptions<AppInfo> appInfo,
        IAuthenticationService authenticationService)
    {
        Title = appInfo.Value.Title;
        _navigator = navigator;
        _authenticationService = authenticationService;

    }

    public string? Title { get; }

    public IState<Credentials> Credentials => State<Credentials>.Value(this,()=> new Credentials { UserName=DummyJsonEndpointConstants.ValidUserName, Password=DummyJsonEndpointConstants.ValidPassword});

    public ICommand Login => Command.Create(b => b.Given(Credentials).When(CanLogin).Then(DoLogin));

    private bool CanLogin(Credentials credentials)
        => credentials is { UserName.Length: > 0 } and { Password.Length: > 0 };

    private async ValueTask DoLogin(Credentials credentials, CancellationToken ct)
    {
        var loggedIn = await _authenticationService.LoginAsync(new Dictionary<string, string>
                                                    {
                                                        { nameof(Commerce.ViewModels.Credentials.UserName), credentials.UserName! },
                                                        { nameof(Commerce.ViewModels.Credentials.Password),credentials.Password! }
                                                    }, cancellationToken: ct);
        if (loggedIn)
        {
            await _navigator.NavigateBackWithResultAsync(this, data: Option.Some(credentials), cancellation: ct);
        }
    }

}
