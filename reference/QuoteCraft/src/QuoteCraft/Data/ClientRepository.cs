using Microsoft.Data.Sqlite;
using QuoteCraft.Models;

namespace QuoteCraft.Data;

public interface IClientRepository
{
    Task<List<ClientEntity>> GetAllAsync();
    Task<ClientEntity?> GetByIdAsync(string id);
    Task SaveAsync(ClientEntity client);
    Task DeleteAsync(string id);
    Task<int> CountActiveAsync();
}

public class ClientRepository : IClientRepository
{
    private readonly AppDatabase _db;

    public ClientRepository(AppDatabase db) => _db = db;

    public async Task<List<ClientEntity>> GetAllAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM clients WHERE is_deleted = 0 ORDER BY name";

        var clients = new List<ClientEntity>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            clients.Add(ReadClient(reader));
        }
        return clients;
    }

    public async Task<ClientEntity?> GetByIdAsync(string id)
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM clients WHERE id = @id AND is_deleted = 0";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadClient(reader);
        }
        return null;
    }

    public async Task SaveAsync(ClientEntity client)
    {
        var conn = await _db.GetConnectionAsync();

        client.UpdatedAt = DateTimeOffset.UtcNow;

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO clients
            (id, name, email, phone, address, updated_at, synced_at, is_deleted)
            VALUES
            (@id, @name, @email, @phone, @address, @updated_at, @synced_at, @is_deleted)
            ON CONFLICT(id) DO UPDATE SET
                name=excluded.name, email=excluded.email, phone=excluded.phone,
                address=excluded.address, updated_at=excluded.updated_at,
                synced_at=excluded.synced_at, is_deleted=excluded.is_deleted
            """;
        cmd.Parameters.AddWithValue("@id", client.Id);
        cmd.Parameters.AddWithValue("@name", client.Name);
        cmd.Parameters.AddWithValue("@email", (object?)client.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", (object?)client.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@address", (object?)client.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@updated_at", client.UpdatedAt.ToString("O"));
        cmd.Parameters.AddWithValue("@synced_at", (object?)client.SyncedAt?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@is_deleted", client.IsDeleted ? 1 : 0);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var conn = await _db.GetConnectionAsync();

        using var tx = conn.BeginTransaction();
        try
        {
            var now = DateTimeOffset.UtcNow.ToString("O");

            // Soft-delete the client
            var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "UPDATE clients SET is_deleted = 1, updated_at = @updated_at WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@updated_at", now);
            await cmd.ExecuteNonQueryAsync();

            // Nullify client_id on associated quotes (keep client_name for historical display)
            var clearCmd = conn.CreateCommand();
            clearCmd.Transaction = tx;
            clearCmd.CommandText = "UPDATE quotes SET client_id = NULL, updated_at = @updated_at WHERE client_id = @id AND is_deleted = 0";
            clearCmd.Parameters.AddWithValue("@id", id);
            clearCmd.Parameters.AddWithValue("@updated_at", now);
            await clearCmd.ExecuteNonQueryAsync();

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<int> CountActiveAsync()
    {
        var conn = await _db.GetConnectionAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM clients WHERE is_deleted = 0";
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private static ClientEntity ReadClient(SqliteDataReader reader)
    {
        return new ClientEntity
        {
            Id = reader.GetString(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
            Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
            UpdatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("updated_at"))),
            IsDeleted = reader.GetInt32(reader.GetOrdinal("is_deleted")) == 1
        };
    }
}
