using Microsoft.Extensions.Logging;
using Pens.Models;
using Pens.Services;

namespace Pens.Presentation;

public partial class ScheduleViewModel : ObservableObject
{
    private readonly ISupabaseService _supabase;
    private readonly ILogger<ScheduleViewModel> _logger;

    public ScheduleViewModel(ISupabaseService supabase, ILogger<ScheduleViewModel> logger)
    {
        _supabase = supabase;
        _logger = logger;
        _ = LoadGamesAsync();
    }

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isEmpty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [ObservableProperty]
    private Game? _nextGame;

    [ObservableProperty]
    private ImmutableList<Game> _upcomingGames = [];

    [ObservableProperty]
    private ImmutableList<GameResult> _lastResults = [];

    [ObservableProperty]
    private string _lastGameDate = "";

    [ObservableProperty]
    private string _lastGameArena = "Dorval Arena";

    [ObservableProperty]
    private bool _isGameToday;

    [ObservableProperty]
    private string _nextGameBadgeText = "NEXT GAME";

    private async Task LoadGamesAsync()
    {
        try
        {
            var dbGames = await _supabase.GetUpcomingGamesAsync();

            var games = dbGames.Select(g => new Game(
                Opponent: g.Opponent,
                Date: g.GameDate.ToString("ddd, MMM d"),
                Time: FormatTime(g.GameTime),
                Rink: g.Rink,
                IsHome: g.IsHome
            )).ToList();

            Game? nextGame = null;
            var upcoming = ImmutableList<Game>.Empty;
            var gameToday = false;
            var badge = "NEXT GAME";
            if (games.Count > 0)
            {
                nextGame = games[0] with { IsNext = true };
                upcoming = games.Skip(1).ToImmutableList();
                gameToday = dbGames[0].GameDate.Date == DateTime.Today;
                badge = gameToday ? "GAME NIGHT" : "NEXT GAME";
            }

            var (lastDate, results) = await LoadPastResultsAsync();

            UiThread.Run(() =>
            {
                NextGame = nextGame;
                UpcomingGames = upcoming;
                IsGameToday = gameToday;
                NextGameBadgeText = badge;
                LastGameDate = lastDate;
                LastResults = results;
                IsLoading = false;
                IsEmpty = !HasError && nextGame is null && upcoming.Count == 0 && results.Count == 0;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading games");
            UiThread.Run(() =>
            {
                ErrorMessage = "Failed to load schedule";
                IsLoading = false;
            });
        }
    }

    private async Task<(string lastDate, ImmutableList<GameResult> results)> LoadPastResultsAsync()
    {
        try
        {
            var pastGames = await _supabase.GetPastGamesAsync();
            if (pastGames.Count == 0)
                return ("", ImmutableList<GameResult>.Empty);

            var lastDate = pastGames[0].GameDate.ToString("MMM d");
            var results = pastGames.Select(g =>
            {
                var homeTeam = g.IsHome ? "Penguins" : g.Opponent;
                var awayTeam = g.IsHome ? g.Opponent : "Penguins";
                var homeScore = g.IsHome ? (g.HomeScore ?? 0) : (g.AwayScore ?? 0);
                var awayScore = g.IsHome ? (g.AwayScore ?? 0) : (g.HomeScore ?? 0);
                return new GameResult(homeTeam, homeScore, awayTeam, awayScore, IsPenguinsGame: true);
            }).ToImmutableList();

            return (lastDate, results);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error loading past games");
            return ("", ImmutableList<GameResult>.Empty);
        }
    }

    private static string FormatTime(string time)
    {
        if (TimeOnly.TryParse(time, out var t))
        {
            return t.ToString("h:mm tt");
        }
        return time;
    }
}
