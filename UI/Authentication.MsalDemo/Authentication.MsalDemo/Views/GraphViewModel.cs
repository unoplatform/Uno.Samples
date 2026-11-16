using Authentication.MsalDemo.Authentication;
using Authentication.MsalDemo.Common;
using Authentication.MsalDemo.Services;

namespace Authentication.MsalDemo.Views;

/// <summary>
/// View state for <see cref="GraphView"/>: calls Microsoft Graph with the token MSAL produced.
/// </summary>
internal sealed class GraphViewModel : ObservableObject
{
    private readonly AuthenticationService _auth = AuthenticationService.Instance;

    private bool _isBusy;
    private string _status = "Not called yet.";
    private string _profile = "";
    private string _rawJson = "";
    private bool _hasResponse;

    public GraphViewModel()
    {
        _auth.StateChanged += (_, _) =>
        {
            Raise(nameof(CanCall));
            Raise(nameof(SignInHint));
            Raise(nameof(NeedsSignIn));
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

    public bool CanCall => !_isBusy && _auth.IsSignedIn;

    public bool NeedsSignIn => !_auth.IsSignedIn;

    public string SignInHint => _auth.IsSignedIn
        ? ""
        : "Sign in on the first page to get an access token, then come back.";

    public string Status
    {
        get => _status;
        private set => Set(ref _status, value);
    }

    public string Profile
    {
        get => _profile;
        private set => Set(ref _profile, value);
    }

    public string RawJson
    {
        get => _rawJson;
        private set => Set(ref _rawJson, value);
    }

    public bool HasResponse
    {
        get => _hasResponse;
        private set => Set(ref _hasResponse, value);
    }

    public async Task CallGraphAsync()
    {
        if (_auth.LastResult is not { } token)
        {
            Status = "No access token. Sign in first.";
            return;
        }

        IsBusy = true;
        Status = "GET https://graph.microsoft.com/v1.0/me ...";

        try
        {
            var result = await GraphClient.GetMeAsync(token.AccessToken);

            HasResponse = true;
            RawJson = result.Body;

            if (result.IsSuccess)
            {
                Status = $"HTTP {result.StatusCode} - the access token was accepted by Microsoft Graph.";
                Profile = $"""
                    Display name  {result.DisplayName ?? "(not set)"}
                    UPN           {result.UserPrincipalName ?? "(not set)"}
                    Mail          {result.Mail ?? "(not set)"}
                    Job title     {result.JobTitle ?? "(not set)"}
                    """;
            }
            else
            {
                Status = $"HTTP {result.StatusCode} - Graph rejected the call. "
                    + "401 means the token was not accepted; 403 usually means the User.Read scope "
                    + "was not granted.";
                Profile = "";
            }
        }
        catch (Exception ex)
        {
            HasResponse = true;
            Status = $"The request failed: {ex.GetType().Name}";
            Profile = "";
            RawJson = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
