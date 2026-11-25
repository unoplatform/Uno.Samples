namespace QuoteCraft.Presentation;

/// <summary>
/// Sign in, create an account or get a sign-in link from the configured Supabase project (route <c>Auth</c>, from
/// Settings' Sign In and Sign Out; D3, D17), or go on without an account.
/// </summary>
public partial record AuthModel
{
    private readonly INavigator _navigator;
    private readonly IAuthService _authService;

    public AuthModel(INavigator navigator, IAuthService authService)
    {
        _navigator = navigator;
        _authService = authService;
    }

    public IState<string> Email => State<string>.Value(this, () => string.Empty);
    public IState<string> Password => State<string>.Value(this, () => string.Empty);
    public IState<string> ErrorMessage => State<string>.Value(this, () => string.Empty);
    public IState<bool> IsSignUp => State<bool>.Value(this, () => false);
    /// <summary>True while a request to the Supabase project runs (the page shows a progress bar).</summary>
    public IState<bool> IsBusy => State<bool>.Value(this, () => false);
    public IState<bool> MagicLinkSent => State<bool>.Value(this, () => false);

    /// <summary>The mode switch under the actions names the other mode.</summary>
    public IFeed<string> ModeToggleText => IsSignUp.Select(signUp => signUp ? "Have an account? Sign in" : "Don't have an account? Sign up");

    public async ValueTask SignIn(CancellationToken ct)
    {
        await ErrorMessage.UpdateAsync(_ => string.Empty, ct);
        var email = (await Email)?.Trim();
        var password = await Password;

        if (string.IsNullOrWhiteSpace(email))
        {
            await ErrorMessage.UpdateAsync(_ => "Email is required.", ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            await ErrorMessage.UpdateAsync(_ => "Password is required.", ct);
            return;
        }

        await RunBusyAsync("Sign in failed.", async () =>
        {
            var result = await _authService.SignInAsync(email, password);
            if (result.IsAuthenticated)
            {
                await _navigator.NavigateRouteAsync(this, "Main", qualifier: Qualifiers.ClearBackStack);
            }
            else
            {
                await ErrorMessage.UpdateAsync(_ => "Invalid email or password.", ct);
            }
        }, ct);
    }

    public async ValueTask SignUp(CancellationToken ct)
    {
        await ErrorMessage.UpdateAsync(_ => string.Empty, ct);
        var email = (await Email)?.Trim();
        var password = await Password;

        if (string.IsNullOrWhiteSpace(email))
        {
            await ErrorMessage.UpdateAsync(_ => "Email is required.", ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            await ErrorMessage.UpdateAsync(_ => "Password must be at least 6 characters.", ct);
            return;
        }

        await RunBusyAsync("Sign up failed.", async () =>
        {
            var result = await _authService.SignUpAsync(email, password);
            if (result.IsAuthenticated)
            {
                await _navigator.NavigateRouteAsync(this, "Main", qualifier: Qualifiers.ClearBackStack);
            }
            else
            {
                await ErrorMessage.UpdateAsync(_ => "Check your email to confirm your account.", ct);
            }
        }, ct);
    }

    public async ValueTask SendMagicLink(CancellationToken ct)
    {
        await ErrorMessage.UpdateAsync(_ => string.Empty, ct);
        var email = (await Email)?.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            await ErrorMessage.UpdateAsync(_ => "Email is required.", ct);
            return;
        }

        await RunBusyAsync("The sign-in link could not be sent.", async () =>
        {
            await _authService.SignInWithMagicLinkAsync(email);
            await MagicLinkSent.UpdateAsync(_ => true, ct);
        }, ct);
    }

    /// <summary>
    /// A request to the Supabase project, with the progress bar showing; a failure of any kind says what did not
    /// happen in the error line (<see cref="ErrorReporter.ReportFailureAsync"/>).
    /// </summary>
    private async ValueTask RunBusyAsync(string failure, Func<Task> request, CancellationToken ct)
    {
        await IsBusy.UpdateAsync(_ => true, ct);
        try
        {
            await ErrorMessage.ReportFailureAsync(failure, request, ct);
        }
        finally
        {
            await IsBusy.UpdateAsync(_ => false, ct);
        }
    }

    public async ValueTask ToggleMode(CancellationToken ct)
    {
        var current = await IsSignUp;
        await IsSignUp.UpdateAsync(_ => !current, ct);
        await ErrorMessage.UpdateAsync(_ => string.Empty, ct);
        await MagicLinkSent.UpdateAsync(_ => false, ct);
    }

    public async ValueTask SkipAuth()
    {
        // Allow Free tier users to use the app without authentication
        await _navigator.NavigateRouteAsync(this, "Main", qualifier: Qualifiers.ClearBackStack);
    }
}
