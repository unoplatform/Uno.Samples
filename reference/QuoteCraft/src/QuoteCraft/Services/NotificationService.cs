using Microsoft.Data.Sqlite;

namespace QuoteCraft.Services;

public class NotificationService : INotificationService
{
    private readonly AppDatabase _db;
    private readonly ILogger<NotificationService> _logger;
    private int _unreadCount;

    public int UnreadCount => _unreadCount;
    public event Action<int>? UnreadCountChanged;

    public NotificationService(AppDatabase db, ILogger<NotificationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<NotificationEntity>> GetAllAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM notifications ORDER BY created_at DESC LIMIT 50";

        var notifications = new List<NotificationEntity>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notifications.Add(ReadNotification(reader));
        }
        return notifications;
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM notifications WHERE is_read = 0";
        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        if (_unreadCount != count)
        {
            _unreadCount = count;
            UnreadCountChanged?.Invoke(count);
        }

        return count;
    }

    public async Task CreateAsync(NotificationType type, string title, string message, string? quoteId = null)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO notifications (id, type, title, message, quote_id, is_read, created_at)
            VALUES (@id, @type, @title, @message, @quote_id, 0, @created_at)
            """;
        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@type", type.ToString());
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@message", message);
        cmd.Parameters.AddWithValue("@quote_id", (object?)quoteId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@created_at", DateTimeOffset.UtcNow.ToString("O"));
        await cmd.ExecuteNonQueryAsync();

        _unreadCount++;
        UnreadCountChanged?.Invoke(_unreadCount);
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE notifications SET is_read = 1 WHERE id = @id AND is_read = 0";
        cmd.Parameters.AddWithValue("@id", notificationId);
        var affected = await cmd.ExecuteNonQueryAsync();

        if (affected > 0)
        {
            _unreadCount = Math.Max(0, _unreadCount - 1);
            UnreadCountChanged?.Invoke(_unreadCount);
        }
    }

    public async Task MarkAllAsReadAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE notifications SET is_read = 1 WHERE is_read = 0";
        await cmd.ExecuteNonQueryAsync();

        _unreadCount = 0;
        UnreadCountChanged?.Invoke(0);
    }

    private static NotificationEntity ReadNotification(SqliteDataReader reader)
    {
        return new NotificationEntity
        {
            Id = reader.GetString(reader.GetOrdinal("id")),
            Type = reader.GetString(reader.GetOrdinal("type")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            Message = reader.GetString(reader.GetOrdinal("message")),
            QuoteId = reader.IsDBNull(reader.GetOrdinal("quote_id")) ? null : reader.GetString(reader.GetOrdinal("quote_id")),
            IsRead = reader.GetInt32(reader.GetOrdinal("is_read")) == 1,
            CreatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("created_at"))),
        };
    }
}
