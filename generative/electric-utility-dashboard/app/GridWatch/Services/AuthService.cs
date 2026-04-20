namespace GridWatch.Services;

public class AuthService
{
    private static AuthService? _instance;
    public static AuthService Instance => _instance ??= new AuthService();

    public bool IsAuthenticated { get; private set; }
    public string? CurrentUser { get; private set; }

    private const string ValidUsername = "admin";
    private const string ValidPassword = "grid2024";

    public AuthService()
    {
        _instance = this;
    }

    public bool Login(string username, string password)
    {
        if (username == ValidUsername && password == ValidPassword)
        {
            IsAuthenticated = true;
            CurrentUser = username;
            return true;
        }

        IsAuthenticated = false;
        CurrentUser = null;
        return false;
    }

    public void Logout()
    {
        IsAuthenticated = false;
        CurrentUser = null;
    }
}
