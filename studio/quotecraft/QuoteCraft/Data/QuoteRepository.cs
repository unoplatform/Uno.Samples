using Microsoft.Data.Sqlite;

namespace QuoteCraft.Data;

public interface IQuoteRepository
{
    Task<List<QuoteEntity>> GetAllAsync();
    Task<QuoteEntity?> GetByIdAsync(string id);
    Task SaveAsync(QuoteEntity quote);
    Task DeleteAsync(string id);
    Task<string> NextQuoteNumberAsync(string prefix, int year);
    Task SaveLineItemAsync(LineItemEntity item);
    Task ReorderLineItemsAsync(string quoteId, IReadOnlyList<string> orderedIds);
    Task DeleteLineItemAsync(string id);
    Task<List<LineItemEntity>> GetLineItemsAsync(string quoteId);
    Task<int> CountCreatedSinceAsync(DateTimeOffset since);
    Task<List<QuoteEntity>> GetExpirableAsync(DateTimeOffset now);

    /// <summary>Marks a Sent or Viewed quote Expired; false when it is no longer Sent or Viewed (or is gone).</summary>
    Task<bool> MarkExpiredAsync(string id);
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
        await using var conn = await _db.OpenConnectionAsync();

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
        lineCmd.CommandText = "SELECT * FROM line_items ORDER BY sort_order, rowid";
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
        await using var conn = await _db.OpenConnectionAsync();

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
        await using var conn = await _db.OpenConnectionAsync();

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
        await using var conn = await _db.OpenConnectionAsync();
        using var tx = conn.BeginTransaction();

        // Soft-delete the quote (it keeps its number: numbers are never reused)
        var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "UPDATE quotes SET is_deleted = 1, updated_at = @updated_at WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@updated_at", DateTimeOffset.UtcNow.ToString("O"));

        await cmd.ExecuteNonQueryAsync();

        // Hard-delete orphaned line items
        var lineCmd = conn.CreateCommand();
        lineCmd.Transaction = tx;
        lineCmd.CommandText = "DELETE FROM line_items WHERE quote_id = @id";
        lineCmd.Parameters.AddWithValue("@id", id);
        await lineCmd.ExecuteNonQueryAsync();
        tx.Commit();

        _photoService.DeletePhotosForQuote(id);
    }

    /// <summary>
    /// The next number of <paramref name="year"/>'s counter (it only goes up), skipping any number a quote already has
    /// (sample data, or a prefix changed and changed back). The quotes table also refuses a duplicate.
    /// </summary>
    public async Task<string> NextQuoteNumberAsync(string prefix, int year)
    {
        await using var conn = await _db.OpenConnectionAsync();

        using var tx = conn.BeginTransaction();
        while (true)
        {
            var next = conn.CreateCommand();
            next.Transaction = tx;
            next.CommandText = """
                INSERT INTO quote_counters (year, last_number) VALUES (@year, 1)
                ON CONFLICT(year) DO UPDATE SET last_number = last_number + 1
                RETURNING last_number
                """;
            next.Parameters.AddWithValue("@year", year);
            var number = QuoteNumberGenerator.Format(prefix, year, Convert.ToInt32(await next.ExecuteScalarAsync()));

            var taken = conn.CreateCommand();
            taken.Transaction = tx;
            taken.CommandText = "SELECT EXISTS (SELECT 1 FROM quotes WHERE quote_number = @number)";
            taken.Parameters.AddWithValue("@number", number);
            if (Convert.ToInt32(await taken.ExecuteScalarAsync()) == 0)
            {
                tx.Commit();
                return number;
            }
        }
    }

    /// <summary>
    /// A new item goes after the quote's last item; an edited item keeps its place (its description, price and
    /// quantity change). <see cref="LineItemEntity.SortOrder"/> is not read: the order changes only through
    /// <see cref="ReorderLineItemsAsync"/>.
    /// </summary>
    public async Task SaveLineItemAsync(LineItemEntity item)
    {
        await using var conn = await _db.OpenConnectionAsync();

        item.UpdatedAt = DateTimeOffset.UtcNow;

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO line_items
            (id, quote_id, description, unit_price, quantity, sort_order, updated_at)
            VALUES
            (@id, @quote_id, @description, @unit_price, @quantity,
             (SELECT COALESCE(MAX(sort_order), -1) + 1 FROM line_items WHERE quote_id = @quote_id), @updated_at)
            ON CONFLICT(id) DO UPDATE SET
                description=excluded.description, unit_price=excluded.unit_price,
                quantity=excluded.quantity, updated_at=excluded.updated_at
            """;
        cmd.Parameters.AddWithValue("@id", item.Id);
        cmd.Parameters.AddWithValue("@quote_id", item.QuoteId);
        cmd.Parameters.AddWithValue("@description", item.Description);
        cmd.Parameters.AddWithValue("@unit_price", (double)item.UnitPrice);
        cmd.Parameters.AddWithValue("@quantity", item.Quantity);
        cmd.Parameters.AddWithValue("@updated_at", item.UpdatedAt.ToString("O"));
        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Numbers the quote's items 0, 1, 2… in the given order, in one transaction. Rewriting every position (rather
    /// than swapping two) also repairs items that share a position.
    /// </summary>
    public async Task ReorderLineItemsAsync(string quoteId, IReadOnlyList<string> orderedIds)
    {
        await using var conn = await _db.OpenConnectionAsync();

        using var transaction = conn.BeginTransaction();
        var now = DateTimeOffset.UtcNow.ToString("O");
        for (var position = 0; position < orderedIds.Count; position++)
        {
            var cmd = conn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = """
                UPDATE line_items SET sort_order = @sort_order, updated_at = @updated_at
                WHERE id = @id AND quote_id = @quote_id
                """;
            cmd.Parameters.AddWithValue("@sort_order", position);
            cmd.Parameters.AddWithValue("@updated_at", now);
            cmd.Parameters.AddWithValue("@id", orderedIds[position]);
            cmd.Parameters.AddWithValue("@quote_id", quoteId);
            await cmd.ExecuteNonQueryAsync();
        }
        transaction.Commit();
    }

    public async Task DeleteLineItemAsync(string id)
    {
        await using var conn = await _db.OpenConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM line_items WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<LineItemEntity>> GetLineItemsAsync(string quoteId)
    {
        await using var conn = await _db.OpenConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM line_items WHERE quote_id = @quote_id ORDER BY sort_order, rowid";
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
        await using var conn = await _db.OpenConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM quotes WHERE is_deleted = 0 AND created_at >= @since";
        cmd.Parameters.AddWithValue("@since", since.ToString("O"));
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<bool> MarkExpiredAsync(string id)
    {
        await using var conn = await _db.OpenConnectionAsync();

        // Only from Sent or Viewed, in one statement: a second expiry of the same quote changes nothing.
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE quotes SET status = 'Expired', updated_at = @updated_at
            WHERE id = @id AND is_deleted = 0 AND status IN ('Sent', 'Viewed')
            """;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@updated_at", DateTimeOffset.UtcNow.ToString("O"));
        return await cmd.ExecuteNonQueryAsync() == 1;
    }

    /// <summary>Sent and Viewed quotes valid until before <paramref name="now"/>; a Draft never expires.</summary>
    public async Task<List<QuoteEntity>> GetExpirableAsync(DateTimeOffset now)
    {
        await using var conn = await _db.OpenConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT * FROM quotes
            WHERE is_deleted = 0
              AND status IN ('Sent', 'Viewed')
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
