using Authentication.CustomExtensionsDemo.Authentication;
using Authentication.CustomExtensionsDemo.Common;
using Authentication.CustomExtensionsDemo.Client;
using Microsoft.Kiota.Abstractions;

namespace Authentication.CustomExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="ApiView"/>: calls the demo server's protected API through the Kiota
/// client, which carries the access token because it was registered inside <c>UseHttp</c>.
/// </summary>
public sealed class ApiViewModel : ObservableObject
{
    private readonly CustomFlowService _flow;
    private readonly DemoServerClient _api;
    private readonly SecretRedactor _redactor;

    private bool _isBusy;
    private string _status = "Not called yet.";
    private string _body = "";
    private bool _hasResponse;

    public ApiViewModel(CustomFlowService flow, DemoServerClient api, SecretRedactor redactor)
    {
        _flow = flow;
        _api = api;
        _redactor = redactor;

        _flow.StateChanged += (_, _) =>
        {
            Raise(nameof(CanCall));
            Raise(nameof(SignInHint));
            Raise(nameof(NeedsSignIn));
        };

        _redactor.Changed += (_, _) =>
        {
            Raise(nameof(Status));
            Raise(nameof(Body));
        };
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            Set(ref _isBusy, value);
            Raise(nameof(CanCall));
        }
    }

    public bool CanCall => !_isBusy;

    public bool NeedsSignIn => !_flow.IsSignedIn;

    public string SignInHint => _flow.IsSignedIn
        ? ""
        : "Not signed in: the call goes out without a token and the API answers 401. Sign in on the first page, then come back.";

    public string Status
    {
        get => _redactor.Apply(_status) ?? _status;
        private set => Set(ref _status, value);
    }

    /// <summary>What the API said about the token; hidden while recording.</summary>
    public string Body
    {
        get => _redactor.Apply(_body) ?? _body;
        private set => Set(ref _body, value);
    }

    public bool HasResponse
    {
        get => _hasResponse;
        private set => Set(ref _hasResponse, value);
    }

    public async Task CallApiAsync()
    {
        IsBusy = true;
        Status = $"GET {_flow.ApiUrl}/api/profile ...";

        try
        {
            // No token handling here: the handler Uno.Extensions chained into this client adds
            // the Authorization header, and on a 401 runs the Refresh callback and retries once.
            var profile = await _api.Api.Profile.GetAsync() ?? throw new InvalidOperationException("The API returned no body.");

            _redactor.Remember(profile.DisplayName, "name");

            HasResponse = true;
            Status = "HTTP 200 - the API accepted the access token.";
            Body = $"""
                User          {profile.UserName}
                Display name  {profile.DisplayName}
                Token issued  {profile.TokenIssuedAt?.ToLocalTime():HH:mm:ss}
                Token expires {profile.TokenExpiresAt?.ToLocalTime():HH:mm:ss}

                Call again after the expiry: the API answers 401, the handler runs the Refresh
                callback (POST /auth/refresh) and retries, and the issue time moves on.
                """;
        }
        catch (ApiException ex)
        {
            HasResponse = true;
            Status = ex.ResponseStatusCode == 401
                ? "HTTP 401 - the API rejected the call. With no token stored this is expected; with one, "
                  + "the handler already tried a refresh and the tokens have been cleared (see the flow log)."
                : $"HTTP {ex.ResponseStatusCode}";
            Body = "";
        }
        catch (Exception ex)
        {
            HasResponse = true;
            Status = $"The request failed: {ex.GetType().Name}";
            Body = $"{ex.Message}{Environment.NewLine}{Environment.NewLine}Is the server project running, and reachable at {_flow.ApiUrl} from this head?";
        }
        finally
        {
            IsBusy = false;
            // A 401 the handler could not fix clears the tokens; let the Sign in page know.
            await _flow.SyncStateAsync();
        }
    }
}
