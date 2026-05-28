using Pens.Models;

namespace Pens.Presentation;

public partial class PlayerViewModel : ObservableObject
{
    private readonly RosterViewModel _parent;

    public PlayerViewModel(int id, string name, int number, string position, PlayerStatus status, RosterViewModel parent, bool isInjured = false, bool isCurrentPlayer = false)
    {
        Id = id;
        Name = name;
        Number = number;
        Position = isInjured ? "IR" : position;
        _status = status;
        _parent = parent;
        IsInjured = isInjured;
        IsCurrentPlayer = isCurrentPlayer;
    }

    public int Id { get; }
    public string Name { get; }
    public int Number { get; }
    public string Position { get; }
    public bool IsInjured { get; }
    public bool IsCurrentPlayer { get; }

    [ObservableProperty]
    private PlayerStatus _status;

    [RelayCommand]
    private async Task ToggleStatusAsync()
    {
        // Only allow the current player to toggle their own status
        if (!IsCurrentPlayer) return;

        Status = Status switch
        {
            PlayerStatus.In => PlayerStatus.Out,
            PlayerStatus.Out => PlayerStatus.Pending,
            PlayerStatus.Pending => PlayerStatus.In,
            _ => PlayerStatus.In
        };
        _parent.UpdateCounts();
        await _parent.SaveAttendanceAsync(Id, Status);
    }
}
