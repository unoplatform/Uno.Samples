using Authentication.MsalExtensionsDemo.Authentication;
using Authentication.MsalExtensionsDemo.Common;
using Authentication.MsalExtensionsDemo.Services;

namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="GraphView"/>: calls Microsoft Graph with the token the
/// authentication stack produced.
/// </summary>
public sealed class GraphViewModel : ObservableObject
{
    private readonly MsalFlowService _flow;
    private readonly SecretRedactor _redactor;

    private bool _isBusy;
    private string _status = "Not called yet.";
    private string _profile = "";
    private string _rawJson = "";
    private bool _hasResponse;

    public GraphViewModel(MsalFlowService flow, SecretRedactor redactor)
    {
        _flow = flow;
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
            Raise(nameof(Profile));
            Raise(nameof(RawJson));
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

    public bool CanCall => !_isBusy && _flow.IsSignedIn;

    public bool NeedsSignIn => !_flow.IsSignedIn;

    public string SignInHint => _flow.IsSignedIn
        ? ""
        : "Sign in on the first page to get an access token, then come back.";

    public string Status
    {
        get => _redactor.Apply(_status) ?? _status;
        private set => Set(ref _status, value);
    }

    public string Profile
    {
        get => _redactor.Apply(_profile) ?? _profile;
        private set => Set(ref _profile, value);
    }

    /// <summary>
    /// The response body. In recording mode every string value is replaced rather than only the
    /// fields the app parsed, so a tenant that returns more of the user's profile than this
    /// sample reads cannot leak through.
    /// </summary>
    public string RawJson
    {
        get => _redactor.ApplyToJson(_rawJson);
        private set => Set(ref _rawJson, value);
    }

    public bool HasResponse
    {
        get => _hasResponse;
        private set => Set(ref _hasResponse, value);
    }

    public async Task CallGraphAsync()
    {
        if (_flow.AccessToken is not { } token)
        {
            Status = "No access token. Sign in first.";
            return;
        }

        IsBusy = true;
        Status = "GET https://graph.microsoft.com/v1.0/me ...";

        try
        {
            var result = await GraphClient.GetMeAsync(token);

            // Before anything is shown: whatever came back identifies a real person, so register
            // it and it stays hidden everywhere it is echoed, including the flow log.
            _redactor.Remember(result.DisplayName, "name");
            _redactor.Remember(result.UserPrincipalName, "account");
            _redactor.Remember(result.Mail, "address");
            _redactor.Remember(result.JobTitle, "job title");

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
