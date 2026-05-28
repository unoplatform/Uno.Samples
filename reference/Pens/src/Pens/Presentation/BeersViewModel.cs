using Microsoft.Extensions.Logging;
using Pens.Services;

namespace Pens.Presentation;

public partial class BeersViewModel : ObservableObject
{
    private readonly ISupabaseService _supabase;
    private readonly ILogger<BeersViewModel> _logger;
    private const int TotalCases = 52;
    private const int BeersPerCase = 30;
    private ImmutableList<CaseBlock>? _cachedCaseBlocks;
    private int _cachedConsumedCases = -1;

    public BeersViewModel(ISupabaseService supabase, ILogger<BeersViewModel> logger)
    {
        _supabase = supabase;
        _logger = logger;
        _ = LoadBeerCountAsync();
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalBeers))]
    [NotifyPropertyChangedFor(nameof(CaseBlocks))]
    [NotifyPropertyChangedFor(nameof(CasesRemaining))]
    [NotifyPropertyChangedFor(nameof(AvgPerGame))]
    [NotifyPropertyChangedFor(nameof(BeersPerPlayer))]
    private int _consumedCases = 0;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    // H3: real, derived stats (replaces the previously hardcoded grid values).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvgPerGame))]
    private int _gamesPlayed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BeersPerPlayer))]
    private int _playerCount;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public int TotalBeers => ConsumedCases * BeersPerCase;
    public int TotalCasesCount => TotalCases;
    public int CasesRemaining => TotalCases - ConsumedCases;
    public int AvgPerGame => GamesPlayed > 0 ? TotalBeers / GamesPlayed : 0;
    public int BeersPerPlayer => PlayerCount > 0 ? TotalBeers / PlayerCount : 0;

    public ImmutableList<CaseBlock> CaseBlocks
    {
        get
        {
            if (_cachedCaseBlocks == null || _cachedConsumedCases != ConsumedCases)
            {
                _cachedConsumedCases = ConsumedCases;
                _cachedCaseBlocks = Enumerable.Range(0, TotalCases)
                    .Select(i => new CaseBlock(i, i < ConsumedCases))
                    .ToImmutableList();
            }
            return _cachedCaseBlocks;
        }
    }

    private async Task LoadBeerCountAsync()
    {
        try
        {
            var trackerTask = _supabase.GetBeerTrackerAsync();
            var pastGamesTask = _supabase.GetPastGamesAsync();
            var playersTask = _supabase.GetPlayersAsync();
            await Task.WhenAll(trackerTask, pastGamesTask, playersTask);

            var consumed = trackerTask.Result?.ConsumedCases ?? 0;
            var games = pastGamesTask.Result.Count;
            var players = playersTask.Result.Count;

            UiThread.Run(() =>
            {
                ConsumedCases = consumed;
                GamesPlayed = games;
                PlayerCount = players;
                IsLoading = false;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading beer count");
            UiThread.Run(() =>
            {
                ErrorMessage = "Failed to load beer data";
                IsLoading = false;
            });
        }
    }

    [RelayCommand]
    private async Task ToggleCaseAsync(CaseBlock caseBlock)
    {
        var newCount = caseBlock.Index < ConsumedCases
            ? caseBlock.Index
            : caseBlock.Index + 1;

        var previousCount = ConsumedCases;
        ConsumedCases = newCount;
        ErrorMessage = null;

        try
        {
            await _supabase.UpdateBeerCountAsync(newCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating beer count");
            UiThread.Run(() =>
            {
                ConsumedCases = previousCount; // Rollback on error
                ErrorMessage = "Failed to save";
            });
        }
    }
}
