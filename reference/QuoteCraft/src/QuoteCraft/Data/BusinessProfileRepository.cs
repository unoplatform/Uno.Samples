using Microsoft.Data.Sqlite;
using QuoteCraft.Models;

namespace QuoteCraft.Data;

public interface IBusinessProfileRepository
{
    Task<BusinessProfileEntity> GetAsync();
    Task SaveAsync(BusinessProfileEntity profile);
}

public class BusinessProfileRepository : IBusinessProfileRepository
{
    private readonly AppDatabase _db;
    private BusinessProfileEntity? _cache;

    public BusinessProfileRepository(AppDatabase db) => _db = db;

    public async Task<BusinessProfileEntity> GetAsync()
    {
        if (_cache is not null) return _cache;

        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM business_profile WHERE id = 'default'";

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            _cache = new BusinessProfileEntity
            {
                Id = reader.GetString(reader.GetOrdinal("id")),
                BusinessName = reader.IsDBNull(reader.GetOrdinal("business_name")) ? null : reader.GetString(reader.GetOrdinal("business_name")),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                Website = TryGetString(reader, "website"),
                BusinessNumber = TryGetString(reader, "business_number"),
                LogoPath = reader.IsDBNull(reader.GetOrdinal("logo_path")) ? null : reader.GetString(reader.GetOrdinal("logo_path")),
                DefaultTaxRate = (decimal)reader.GetDouble(reader.GetOrdinal("default_tax_rate")),
                DefaultMarkup = reader.IsDBNull(reader.GetOrdinal("default_markup")) ? 0m : (decimal)reader.GetDouble(reader.GetOrdinal("default_markup")),
                CurrencyCode = reader.GetString(reader.GetOrdinal("currency_code")),
                QuoteValidDays = reader.GetInt32(reader.GetOrdinal("quote_valid_days")),
                QuoteNumberPrefix = reader.GetString(reader.GetOrdinal("quote_number_prefix")),
                CustomFooter = reader.IsDBNull(reader.GetOrdinal("custom_footer")) ? null : reader.GetString(reader.GetOrdinal("custom_footer")),
                UpdatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("updated_at")))
            };
            return _cache;
        }

        // Return default profile if none exists
        _cache = new BusinessProfileEntity();
        return _cache;
    }

    public async Task SaveAsync(BusinessProfileEntity profile)
    {
        _cache = null; // Invalidate cache

        var conn = await _db.GetConnectionAsync();

        profile.UpdatedAt = DateTimeOffset.UtcNow;

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO business_profile
            (id, business_name, phone, email, address, website, business_number, logo_path, default_tax_rate, default_markup, currency_code, quote_valid_days, quote_number_prefix, custom_footer, updated_at)
            VALUES
            ('default', @business_name, @phone, @email, @address, @website, @business_number, @logo_path, @default_tax_rate, @default_markup, @currency_code, @quote_valid_days, @quote_number_prefix, @custom_footer, @updated_at)
            ON CONFLICT(id) DO UPDATE SET
                business_name=excluded.business_name, phone=excluded.phone, email=excluded.email,
                address=excluded.address, website=excluded.website, business_number=excluded.business_number,
                logo_path=excluded.logo_path,
                default_tax_rate=excluded.default_tax_rate, default_markup=excluded.default_markup,
                currency_code=excluded.currency_code, quote_valid_days=excluded.quote_valid_days,
                quote_number_prefix=excluded.quote_number_prefix, custom_footer=excluded.custom_footer,
                updated_at=excluded.updated_at
            """;
        cmd.Parameters.AddWithValue("@business_name", (object?)profile.BusinessName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", (object?)profile.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@email", (object?)profile.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@address", (object?)profile.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@website", (object?)profile.Website ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@business_number", (object?)profile.BusinessNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@logo_path", (object?)profile.LogoPath ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@default_tax_rate", (double)profile.DefaultTaxRate);
        cmd.Parameters.AddWithValue("@default_markup", (double)profile.DefaultMarkup);
        cmd.Parameters.AddWithValue("@currency_code", profile.CurrencyCode);
        cmd.Parameters.AddWithValue("@quote_valid_days", profile.QuoteValidDays);
        cmd.Parameters.AddWithValue("@quote_number_prefix", profile.QuoteNumberPrefix);
        cmd.Parameters.AddWithValue("@custom_footer", (object?)profile.CustomFooter ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@updated_at", profile.UpdatedAt.ToString("O"));
        await cmd.ExecuteNonQueryAsync();
    }

    private static string? TryGetString(SqliteDataReader reader, string column)
    {
        try
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null; // Column doesn't exist yet (pre-migration)
        }
    }
}
