using Microsoft.Data.Sqlite;
using QuoteCraft.Models;

namespace QuoteCraft.Data;

public interface IQuoteRepository
{
    Task<List<QuoteEntity>> GetAllAsync();
    Task<QuoteEntity?> GetByIdAsync(string id);
    Task SaveAsync(QuoteEntity quote);
    Task DeleteAsync(string id);
    Task<int> GetQuoteCountAsync();
    Task SaveLineItemAsync(LineItemEntity item);
    Task SaveLineItemsBatchAsync(params LineItemEntity[] items);
    Task DeleteLineItemAsync(string id);
    Task<List<LineItemEntity>> GetLineItemsAsync(string quoteId);
    Task<int> CountCreatedSinceAsync(DateTimeOffset since);
    Task<List<QuoteEntity>> GetExpirableAsync(DateTimeOffset now);
}

public class QuoteRepository : IQuoteRepository
{
    private readonly AppDatabase _db;
    private readonly Services.IPhotoService _photoService;

    public QuoteRepository(AppDatabase db, Services.IPhotoService photoService)
    {
        _db = db;
        _photoService = photoService;
    }

    public async Task<List<QuoteEntity>> GetAllAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM quotes WHERE is_deleted = 0 ORDER BY created_at DESC";

        var quotes = new List<QuoteEntity>();
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                quotes.Add(ReadQuote(reader));
        }

        // Load all line items in one query instead of N+1
        var lineCmd = conn.CreateCommand();
        lineCmd.CommandText = "SELECT * FROM line_items ORDER BY sort_order";
        var itemsByQuote = new Dictionary<string, List<LineItemEntity>>();
        using (var reader = await lineCmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var item = ReadLineItem(reader);
                if (!itemsByQuote.TryGetValue(item.QuoteId, out var list))
                {
                    list = new List<LineItemEntity>();
                    itemsByQuote[item.QuoteId] = list;
                }
                list.Add(item);
            }
        }

        foreach (var quote in quotes)
        {
            quote.LineItems = itemsByQuote.GetValueOrDefault(quote.Id, new List<LineItemEntity>());
        }

        return quotes;
    }

    public async Task<QuoteEntity?> GetByIdAsync(string id)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM quotes WHERE id = @id AND is_deleted = 0";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var quote = ReadQuote(reader);
            quote.LineItems = await GetLineItemsAsync(quote.Id);
            return quote;
        }
        return null;
    }

    public async Task SaveAsync(QuoteEntity quote)
    {
        var conn = await _db.GetConnectionAsync();

        quote.UpdatedAt = DateTimeOffset.UtcNow;

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO quotes
            (id, title, client_id, client_name, notes, tax_rate, status, quote_number, created_at, sent_at, valid_until, updated_at, synced_at, share_token, is_deleted)
            VALUES
            (@id, @title, @client_id, @client_name, @notes, @tax_rate, @status, @quote_number, @created_at, @sent_at, @valid_until, @updated_at, @synced_at, @share_token, @is_deleted)
            ON CONFLICT(id) DO UPDATE SET
                title=excluded.title, client_id=excluded.client_id, client_name=excluded.client_name,
                notes=excluded.notes, tax_rate=excluded.tax_rate, status=excluded.status,
                quote_number=excluded.quote_number, created_at=excluded.created_at,
                sent_at=excluded.sent_at, valid_until=excluded.valid_until,
                updated_at=excluded.updated_at, synced_at=excluded.synced_at,
                share_token=excluded.share_token, is_deleted=excluded.is_deleted
            """;
        cmd.Parameters.AddWithValue("@id", quote.Id);
        cmd.Parameters.AddWithValue("@title", quote.Title);
        cmd.Parameters.AddWithValue("@client_id", (object?)quote.ClientId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@client_name", (object?)quote.ClientName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@notes", (object?)quote.Notes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@tax_rate", (double)quote.TaxRate);
        cmd.Parameters.AddWithValue("@status", quote.Status.ToString());
        cmd.Parameters.AddWithValue("@quote_number", quote.QuoteNumber);
        cmd.Parameters.AddWithValue("@created_at", quote.CreatedAt.ToString("O"));
        cmd.Parameters.AddWithValue("@sent_at", (object?)quote.SentAt?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@valid_until", (object?)quote.ValidUntil?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@updated_at", quote.UpdatedAt.ToString("O"));
        cmd.Parameters.AddWithValue("@synced_at", (object?)quote.SyncedAt?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@share_token", (object?)quote.ShareToken ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@is_deleted", quote.IsDeleted ? 1 : 0);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var conn = await _db.GetConnectionAsync();

        // Soft-delete the quote
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE quotes SET is_deleted = 1, updated_at = @updated_at WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@updated_at", DateTimeOffset.UtcNow.ToString("O"));

        await cmd.ExecuteNonQueryAsync();

        // Hard-delete orphaned line items
        var lineCmd = conn.CreateCommand();
        lineCmd.CommandText = "DELETE FROM line_items WHERE quote_id = @id";
        lineCmd.Parameters.AddWithValue("@id", id);
        await lineCmd.ExecuteNonQueryAsync();

        // Clean up photo files from disk
        _photoService.DeletePhotosForQuote(id);
    }

    public async Task<int> GetQuoteCountAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM quotes WHERE is_deleted = 0";
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task SaveLineItemAsync(LineItemEntity item)
    {
        var conn = await _db.GetConnectionAsync();

        item.UpdatedAt = DateTimeOffset.UtcNow;

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO line_items
            (id, quote_id, description, unit_price, quantity, sort_order, updated_at)
            VALUES
            (@id, @quote_id, @description, @unit_price, @quantity, @sort_order, @updated_at)
            ON CONFLICT(id) DO UPDATE SET
                quote_id=excluded.quote_id, description=excluded.description,
                unit_price=excluded.unit_price, quantity=excluded.quantity,
                sort_order=excluded.sort_order, updated_at=excluded.updated_at
            """;
        cmd.Parameters.AddWithValue("@id", item.Id);
        cmd.Parameters.AddWithValue("@quote_id", item.QuoteId);
        cmd.Parameters.AddWithValue("@description", item.Description);
        cmd.Parameters.AddWithValue("@unit_price", (double)item.UnitPrice);
        cmd.Parameters.AddWithValue("@quantity", item.Quantity);
        cmd.Parameters.AddWithValue("@sort_order", item.SortOrder);
        cmd.Parameters.AddWithValue("@updated_at", item.UpdatedAt.ToString("O"));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SaveLineItemsBatchAsync(params LineItemEntity[] items)
    {
        var conn = await _db.GetConnectionAsync();

        using var transaction = conn.BeginTransaction();
        try
        {
            foreach (var item in items)
            {
                item.UpdatedAt = DateTimeOffset.UtcNow;
                var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = """
                    INSERT OR REPLACE INTO line_items
                    (id, quote_id, description, unit_price, quantity, sort_order, updated_at)
                    VALUES
                    (@id, @quote_id, @description, @unit_price, @quantity, @sort_order, @updated_at)
                    """;
                cmd.Parameters.AddWithValue("@id", item.Id);
                cmd.Parameters.AddWithValue("@quote_id", item.QuoteId);
                cmd.Parameters.AddWithValue("@description", item.Description);
                cmd.Parameters.AddWithValue("@unit_price", (double)item.UnitPrice);
                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@sort_order", item.SortOrder);
                cmd.Parameters.AddWithValue("@updated_at", item.UpdatedAt.ToString("O"));
                await cmd.ExecuteNonQueryAsync();
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeleteLineItemAsync(string id)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM line_items WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<LineItemEntity>> GetLineItemsAsync(string quoteId)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM line_items WHERE quote_id = @quote_id ORDER BY sort_order";
        cmd.Parameters.AddWithValue("@quote_id", quoteId);

        var items = new List<LineItemEntity>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(ReadLineItem(reader));
        }
        return items;
    }

    public async Task<int> CountCreatedSinceAsync(DateTimeOffset since)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM quotes WHERE is_deleted = 0 AND created_at >= @since";
        cmd.Parameters.AddWithValue("@since", since.ToString("O"));
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<List<QuoteEntity>> GetExpirableAsync(DateTimeOffset now)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT * FROM quotes
            WHERE is_deleted = 0
              AND status IN ('Draft', 'Sent', 'Viewed')
              AND valid_until IS NOT NULL
              AND valid_until < @now
            """;
        cmd.Parameters.AddWithValue("@now", now.ToString("O"));

        var quotes = new List<QuoteEntity>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            quotes.Add(ReadQuote(reader));
        }
        return quotes;
    }

    private static LineItemEntity ReadLineItem(SqliteDataReader reader)
    {
        return new LineItemEntity
        {
            Id = reader.GetString(reader.GetOrdinal("id")),
            QuoteId = reader.GetString(reader.GetOrdinal("quote_id")),
            Description = reader.GetString(reader.GetOrdinal("description")),
            UnitPrice = (decimal)reader.GetDouble(reader.GetOrdinal("unit_price")),
            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
            SortOrder = reader.GetInt32(reader.GetOrdinal("sort_order")),
            UpdatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("updated_at")))
        };
    }

    private static QuoteEntity ReadQuote(SqliteDataReader reader)
    {
        var syncedAtOrd = TryGetOrdinal(reader, "synced_at");
        var shareTokenOrd = TryGetOrdinal(reader, "share_token");

        return new QuoteEntity
        {
            Id = reader.GetString(reader.GetOrdinal("id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            ClientId = reader.IsDBNull(reader.GetOrdinal("client_id")) ? null : reader.GetString(reader.GetOrdinal("client_id")),
            ClientName = reader.IsDBNull(reader.GetOrdinal("client_name")) ? null : reader.GetString(reader.GetOrdinal("client_name")),
            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
            TaxRate = (decimal)reader.GetDouble(reader.GetOrdinal("tax_rate")),
            Status = Enum.Parse<QuoteStatus>(reader.GetString(reader.GetOrdinal("status"))),
            QuoteNumber = reader.GetString(reader.GetOrdinal("quote_number")),
            CreatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("created_at"))),
            SentAt = reader.IsDBNull(reader.GetOrdinal("sent_at")) ? null : DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("sent_at"))),
            ValidUntil = reader.IsDBNull(reader.GetOrdinal("valid_until")) ? null : DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("valid_until"))),
            UpdatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("updated_at"))),
            SyncedAt = syncedAtOrd >= 0 && !reader.IsDBNull(syncedAtOrd) ? DateTimeOffset.Parse(reader.GetString(syncedAtOrd)) : null,
            ShareToken = shareTokenOrd >= 0 && !reader.IsDBNull(shareTokenOrd) ? reader.GetString(shareTokenOrd) : null,
            IsDeleted = reader.GetInt32(reader.GetOrdinal("is_deleted")) == 1
        };
    }

    private static int TryGetOrdinal(SqliteDataReader reader, string name)
    {
        try { return reader.GetOrdinal(name); } catch { return -1; }
    }
}
