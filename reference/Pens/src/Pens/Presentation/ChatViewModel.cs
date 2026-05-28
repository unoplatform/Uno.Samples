using Microsoft.Extensions.Logging;
using Pens.Models;
using Pens.Services;

namespace Pens.Presentation;

public partial class ChatViewModel : ObservableObject, IDisposable
{
    private readonly ISupabaseService _supabase;
    private readonly IPlayerIdentityService _identity;
    private readonly ILogger<ChatViewModel> _logger;
    private readonly CancellationTokenSource _cts = new();

    public ChatViewModel(ISupabaseService supabase, IPlayerIdentityService identity, ILogger<ChatViewModel> logger)
    {
        _supabase = supabase;
        _identity = identity;
        _logger = logger;
        _ = LoadMessagesAsync();
    }

    [ObservableProperty]
    private string _messageText = string.Empty;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isEmpty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ObservableCollection<ChatMessage> Messages { get; } = [];

    private async Task LoadMessagesAsync()
    {
        try
        {
            var dbMessages = await _supabase.GetChatMessagesAsync(50);
            var loaded = dbMessages.Select(ToViewModel).ToList();

            UiThread.Run(() =>
            {
                foreach (var msg in loaded)
                {
                    Messages.Add(msg);
                }
                IsEmpty = Messages.Count == 0;
                IsLoading = false;
            });

            // Subscribe to new messages using polling with cancellation support
            await _supabase.SubscribeToChatMessagesAsync(OnNewMessage, _cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading messages");
            UiThread.Run(() =>
            {
                ErrorMessage = "Failed to load messages";
                IsLoading = false;
            });
        }
    }

    private void OnNewMessage(DbChatMessage msg)
    {
        UiThread.Run(() =>
        {
            // Avoid duplicates
            if (!Messages.Any(m => m.Message == msg.Message && m.Sender == msg.PlayerName))
            {
                Messages.Add(ToViewModel(msg));
                IsEmpty = false;
            }
        });
    }

    private static ChatMessage ToViewModel(DbChatMessage msg)
    {
        var initials = GetInitials(msg.PlayerName);
        var time = msg.CreatedAt.ToLocalTime().ToString("h:mm tt");
        return new ChatMessage(initials, msg.PlayerName, time, msg.Message);
    }

    private static string GetInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        if (parts.Length == 1 && parts[0].Length >= 2)
            return parts[0][..2].ToUpper();
        return name.Length >= 2 ? name[..2].ToUpper() : name.ToUpper();
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(MessageText))
            return;

        var messageToSend = MessageText.Trim();
        if (messageToSend.Length > 500)
        {
            ErrorMessage = "Message too long (max 500 characters)";
            return;
        }

        MessageText = string.Empty;
        ErrorMessage = null;

        try
        {
            var playerName = _identity.CurrentPlayerName ?? "Unknown";
            await _supabase.SendChatMessageAsync(_identity.CurrentPlayerId, playerName, messageToSend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            UiThread.Run(() =>
            {
                MessageText = messageToSend;
                ErrorMessage = "Failed to send message";
            });
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _supabase.UnsubscribeFromChat();
    }
}
