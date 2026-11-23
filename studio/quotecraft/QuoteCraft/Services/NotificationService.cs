using Microsoft.Data.Sqlite;

namespace QuoteCraft.Services;

public class NotificationService : INotificationService
{
    private readonly AppDatabase _db;
    private int _unreadCount;

    public int UnreadCount => _unreadCount;
    public event Action<int>? UnreadCountChanged;

    public NotificationService(AppDatabase db)
    {
        _db = db;
    }

    public async Task<List<NotificationEntity>> GetAllAsync()
    {
        await using var conn = await _db.OpenConnectionAsync();

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
        await using var conn = await _db.OpenConnectionAsync();

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
        await using var conn = await _db.OpenConnectionAsync();

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

    public async Task<bool> ExistsAsync(NotificationType type, string quoteId, DateTimeOffset since)
    {
        await using var conn = await _db.OpenConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT EXISTS (SELECT 1 FROM notifications WHERE type = @type AND quote_id = @quote_id AND created_at >= @since)
            """;
        cmd.Parameters.AddWithValue("@type", type.ToString());
        cmd.Parameters.AddWithValue("@quote_id", quoteId);
        cmd.Parameters.AddWithValue("@since", since.ToUniversalTime().ToString("O"));
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) == 1;
    }

    public async Task MarkAllAsReadAsync()
    {
        await using var conn = await _db.OpenConnectionAsync();

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
