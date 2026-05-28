using Pens.Services;
using Uno.Extensions.Navigation;

namespace Pens.Presentation;

public partial class PlayerPickerViewModel : ObservableObject
{
    private readonly ISupabaseService _supabase;
    private readonly IPlayerIdentityService _identity;
    private readonly INavigator _navigator;

    public PlayerPickerViewModel(ISupabaseService supabase, IPlayerIdentityService identity, INavigator navigator)
    {
        _supabase = supabase;
        _identity = identity;
        _navigator = navigator;
        _ = LoadPlayersAsync();
    }

    public ObservableCollection<PlayerPickerItem> Players { get; } = [];

    [ObservableProperty]
    private bool _isLoading = true;

    private async Task LoadPlayersAsync()
    {
        try
        {
            IsLoading = true;
            var dbPlayers = await _supabase.GetPlayersAsync();

            foreach (var player in dbPlayers)
            {
                Players.Add(new PlayerPickerItem(
                    player.Id,
                    player.Name,
                    player.Number,
                    player.IsInjured ? "IR" : player.Position,
                    () => SelectPlayer(player.Id, player.Name)
                ));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading players: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SelectPlayer(int playerId, string playerName)
    {
        _identity.SetCurrentPlayer(playerId, playerName);
        // Enter the tab shell and clear the login page from the back stack.
        _ = _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
    }
}

public partial record PlayerPickerItem(int Id, string Name, int Number, string Position, Action SelectAction)
{
    public ICommand SelectCommand => new RelayCommand(SelectAction);
}
