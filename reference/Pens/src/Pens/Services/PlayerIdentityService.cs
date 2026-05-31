using Windows.Storage;

namespace Pens.Services;

public interface IPlayerIdentityService
{
    int? CurrentPlayerId { get; }
    string? CurrentPlayerName { get; }
    bool IsLoggedIn { get; }
    void SetCurrentPlayer(int playerId, string playerName);
    void ClearCurrentPlayer();
}

public class PlayerIdentityService : IPlayerIdentityService
{
    private const string PlayerIdKey = "CurrentPlayerId";
    private const string PlayerNameKey = "CurrentPlayerName";

    private int? _currentPlayerId;
    private string? _currentPlayerName;

    public PlayerIdentityService()
    {
        LoadFromStorage();
    }

    public int? CurrentPlayerId => _currentPlayerId;
    public string? CurrentPlayerName => _currentPlayerName;
    public bool IsLoggedIn => _currentPlayerId.HasValue;

    public void SetCurrentPlayer(int playerId, string playerName)
    {
        _currentPlayerId = playerId;
        _currentPlayerName = playerName;
        SaveToStorage();
    }

    public void ClearCurrentPlayer()
    {
        _currentPlayerId = null;
        _currentPlayerName = null;
        ClearStorage();
    }

    private void LoadFromStorage()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue(PlayerIdKey, out var idValue) && idValue is int id)
        {
            _currentPlayerId = id;
        }
        if (localSettings.Values.TryGetValue(PlayerNameKey, out var nameValue) && nameValue is string name)
        {
            _currentPlayerName = name;
        }
    }

    private void SaveToStorage()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (_currentPlayerId.HasValue)
        {
            localSettings.Values[PlayerIdKey] = _currentPlayerId.Value;
        }
        if (_currentPlayerName != null)
        {
            localSettings.Values[PlayerNameKey] = _currentPlayerName;
        }
    }

    private void ClearStorage()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values.Remove(PlayerIdKey);
        localSettings.Values.Remove(PlayerNameKey);
    }
}
