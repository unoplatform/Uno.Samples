using Authentication.OidcExtensionsDemo.Authentication;
using Authentication.OidcExtensionsDemo.Common;
using Authentication.OidcExtensionsDemo.Services.Endpoints;
using Refit;

namespace Authentication.OidcExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="ApiView"/>: calls the demo server's protected API through the Refit
/// client, which carries the access token because it was registered inside <c>UseHttp</c>.
/// </summary>
public sealed class ApiViewModel : ObservableObject
{
    private readonly OidcFlowService _flow;
    private readonly IDemoApi _api;
    private readonly SecretRedactor _redactor;

    private bool _isBusy;
    private string _status = "Not called yet.";
    private string _claims = "";
    private bool _hasResponse;

    public ApiViewModel(OidcFlowService flow, IDemoApi api, SecretRedactor redactor)
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
            Raise(nameof(Claims));
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

    /// <summary>The claims the API read from the token, one per line; hidden while recording.</summary>
    public string Claims
    {
        get => _redactor.Apply(_claims) ?? _claims;
        private set => Set(ref _claims, value);
    }

    public bool HasResponse
    {
        get => _hasResponse;
        private set => Set(ref _hasResponse, value);
    }

    public async Task CallApiAsync()
    {
        IsBusy = true;
        Status = "GET https://demo.duendesoftware.com/api/test ...";

        try
        {
            // No token handling here: the handler Uno.Extensions chained into this client adds
            // the Authorization header, and on a 401 refreshes the token and retries once.
            var claims = await _api.GetTestAsync(CancellationToken.None);

            foreach (var claim in claims)
            {
                if (claim.Type is "sub" or "name" or "email" or "preferred_username")
                {
                    _redactor.Remember(claim.Value, claim.Type);
                }
            }

            HasResponse = true;
            Status = $"HTTP 200 - the API accepted the access token and returned {claims.Length} claims.";
            Claims = string.Join(
                Environment.NewLine,
                claims.Select(claim => $"{claim.Type,-22} {claim.Value}"));
        }
        catch (ApiException ex)
        {
            HasResponse = true;
            Status = ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? "HTTP 401 - the API rejected the call. With no token stored this is expected; with one, "
                  + "the handler already tried a refresh and the tokens have been cleared (see the flow log)."
                : $"HTTP {(int)ex.StatusCode} - {ex.ReasonPhrase}";
            Claims = ex.Content ?? "";
        }
        catch (Exception ex)
        {
            HasResponse = true;
            Status = $"The request failed: {ex.GetType().Name}";
            Claims = ex.Message;
        }
        finally
        {
            IsBusy = false;
            // A 401 the handler could not fix clears the tokens; let the Sign in page know.
            await _flow.SyncStateAsync();
        }
    }
}
