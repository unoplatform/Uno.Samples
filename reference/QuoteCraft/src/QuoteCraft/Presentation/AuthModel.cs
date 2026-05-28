using QuoteCraft.Services;

namespace QuoteCraft.Presentation;

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
    public IState<bool> IsBusy => State<bool>.Value(this, () => false);
    public IState<bool> MagicLinkSent => State<bool>.Value(this, () => false);

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

        await IsBusy.UpdateAsync(_ => true, ct);
        try
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
        }
        catch (HttpRequestException ex)
        {
            await ErrorMessage.UpdateAsync(_ => $"Sign in failed: {ex.Message}", ct);
        }
        finally
        {
            await IsBusy.UpdateAsync(_ => false, ct);
        }
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

        await IsBusy.UpdateAsync(_ => true, ct);
        try
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
        }
        catch (HttpRequestException ex)
        {
            await ErrorMessage.UpdateAsync(_ => $"Sign up failed: {ex.Message}", ct);
        }
        finally
        {
            await IsBusy.UpdateAsync(_ => false, ct);
        }
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

        await IsBusy.UpdateAsync(_ => true, ct);
        try
        {
            await _authService.SignInWithMagicLinkAsync(email);
            await MagicLinkSent.UpdateAsync(_ => true, ct);
        }
        catch (HttpRequestException ex)
        {
            await ErrorMessage.UpdateAsync(_ => $"Failed to send magic link: {ex.Message}", ct);
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

    public async ValueTask SkipAuth(CancellationToken ct)
    {
        // Allow Free tier users to use the app without authentication
        await _navigator.NavigateRouteAsync(this, "Main", qualifier: Qualifiers.ClearBackStack);
    }
}
