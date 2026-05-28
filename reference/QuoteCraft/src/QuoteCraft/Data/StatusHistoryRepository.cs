using Microsoft.Data.Sqlite;
using QuoteCraft.Models;

namespace QuoteCraft.Data;

public interface IStatusHistoryRepository
{
    Task RecordAsync(string quoteId, string status, string? changedBy = null);
    Task<List<StatusHistoryEntry>> GetByQuoteIdAsync(string quoteId);
}

public class StatusHistoryRepository : IStatusHistoryRepository
{
    private readonly AppDatabase _db;

    public StatusHistoryRepository(AppDatabase db)
    {
        _db = db;
    }

    public async Task RecordAsync(string quoteId, string status, string? changedBy = null)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO status_history (id, quote_id, status, changed_at, changed_by)
            VALUES (@id, @quote_id, @status, @changed_at, @changed_by)
            """;
        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@quote_id", quoteId);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@changed_at", DateTimeOffset.UtcNow.ToString("O"));
        cmd.Parameters.AddWithValue("@changed_by", (object?)changedBy ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<StatusHistoryEntry>> GetByQuoteIdAsync(string quoteId)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM status_history WHERE quote_id = @quote_id ORDER BY changed_at ASC";
        cmd.Parameters.AddWithValue("@quote_id", quoteId);

        var entries = new List<StatusHistoryEntry>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            entries.Add(new StatusHistoryEntry
            {
                Id = reader.GetString(reader.GetOrdinal("id")),
                QuoteId = reader.GetString(reader.GetOrdinal("quote_id")),
                Status = reader.GetString(reader.GetOrdinal("status")),
                ChangedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("changed_at"))),
                ChangedBy = reader.IsDBNull(reader.GetOrdinal("changed_by")) ? null : reader.GetString(reader.GetOrdinal("changed_by")),
            });
        }
        return entries;
    }
}
