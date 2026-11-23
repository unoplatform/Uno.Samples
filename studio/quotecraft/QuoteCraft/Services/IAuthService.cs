namespace QuoteCraft.Services;

public record AuthState(bool IsAuthenticated, string? UserId, string? Email, string? AccessToken, string? RefreshToken);

public interface IAuthService
{
    /// <summary>False when no Supabase project is configured: the app runs without accounts.</summary>
    bool IsAvailable { get; }

    AuthState CurrentState { get; }
    event Action<AuthState>? AuthStateChanged;

    Task<AuthState> SignUpAsync(string email, string password);
    Task<AuthState> SignInAsync(string email, string password);
    Task<AuthState> SignInWithMagicLinkAsync(string email);
    Task SignOutAsync();
    Task<AuthState> RefreshSessionAsync();
}
