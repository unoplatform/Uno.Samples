using Microsoft.Data.Sqlite;

namespace QuoteCraft.Data;

/// <summary>
/// The local SQLite database. Each operation opens its own connection and disposes it when done
/// (<see cref="OpenConnectionAsync"/>): MVUX feeds read on background threads while pages write on the UI thread, and
/// one shared connection let their commands and transactions interleave. Microsoft.Data.Sqlite pools the connections,
/// so the SQLCipher key is derived once per pooled connection, not per operation; WAL lets readers run beside a
/// writer, and a writer that finds the database busy retries for the command timeout (30 s).
/// </summary>
public class AppDatabase
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private volatile bool _initialized;

    public AppDatabase()
        : this(AppStorage.Root)
    {
    }

    /// <param name="dir">The folder that holds the database (and, on desktop, its key file); tests pass a temporary folder.</param>
    public AppDatabase(string dir)
    {
        SQLitePCL.Batteries_V2.Init();

        Directory.CreateDirectory(dir);
        var dbPath = Path.Combine(dir, "quotecraft.db");

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            ForeignKeys = true,
            Pooling = true,
        };
#if !__WASM__
        // WebAssembly: the database lives in the browser's IndexedDB storage for this site, where a key would have to
        // sit next to it, so SQLCipher runs without one there (docs/DECISIONS-production-readiness.md, D1).
        // Android, iOS and WinAppSDK keep the key in the platform's vault, desktop in a file beside the database (D13).
        connectionString.Password = new StoredSecret("database-key", Path.Combine(dir, ".dbkey"), StoredSecret.PlatformVault)
            .GetOrCreate(NewEncryptionKey);
#endif
        _connectionString = connectionString.ToString();
    }

#if !__WASM__
    /// <summary>A random 32-character hex key.</summary>
    private static string NewEncryptionKey()
    {
        var keyBytes = new byte[16];
        System.Security.Cryptography.RandomNumberGenerator.Fill(keyBytes);
        return Convert.ToHexString(keyBytes);
    }
#endif

#if DEBUG
    /// <summary>Debug verification only: operations fail until this moment (see <see cref="OpenConnectionAsync"/>).</summary>
    public static DateTimeOffset FailUntil { get; set; } = DateTimeOffset.MinValue;
#endif

    /// <summary>
    /// Opens a connection for one operation; the caller disposes it (<c>await using</c>). The first call creates or
    /// migrates the schema.
    /// </summary>
    public async Task<SqliteConnection> OpenConnectionAsync()
    {
#if DEBUG
        // Verification hook (--qc-fault=load, App.xaml.cs): every operation fails until the moment set, so the pages'
        // error states and Retry can be checked at runtime.
        if (DateTimeOffset.UtcNow < FailUntil)
        {
            throw new InvalidOperationException("Simulated database failure (--qc-fault=load).");
        }
#endif
        var connection = new SqliteConnection(_connectionString);
        try
        {
            await connection.OpenAsync();
            if (!_initialized)
            {
                await _initLock.WaitAsync();
                try
                {
                    if (!_initialized)
                    {
                        await InitializeCoreAsync(connection);
                        _initialized = true;
                    }
                }
                finally
                {
                    _initLock.Release();
                }
            }
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    private static async Task InitializeCoreAsync(SqliteConnection connection)
    {
#if !__WASM__
        // Enable WAL mode for better concurrent read performance. Not on WebAssembly: WAL is for concurrent readers and
        // the browser runtime has one thread, and the -wal/-shm side files would only add to what IndexedDB syncs.
        // The browser keeps SQLite's default rollback journal.
        var walCmd = connection.CreateCommand();
        walCmd.CommandText = "PRAGMA journal_mode=WAL;";
        await walCmd.ExecuteNonQueryAsync();
#endif

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS schema_version (
                version INTEGER PRIMARY KEY,
                applied_at TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS clients (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                email TEXT,
                phone TEXT,
                address TEXT,
                updated_at TEXT NOT NULL,
                is_deleted INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS quotes (
                id TEXT PRIMARY KEY,
                title TEXT NOT NULL,
                client_id TEXT,
                client_name TEXT,
                notes TEXT,
                tax_rate REAL NOT NULL DEFAULT 0,
                status TEXT NOT NULL DEFAULT 'Draft',
                quote_number TEXT NOT NULL,
                created_at TEXT NOT NULL,
                sent_at TEXT,
                valid_until TEXT,
                updated_at TEXT NOT NULL,
                is_deleted INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS line_items (
                id TEXT PRIMARY KEY,
                quote_id TEXT NOT NULL,
                description TEXT NOT NULL,
                unit_price REAL NOT NULL,
                quantity INTEGER NOT NULL DEFAULT 1,
                sort_order INTEGER NOT NULL DEFAULT 0,
                updated_at TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS business_profile (
                id TEXT PRIMARY KEY DEFAULT 'default',
                business_name TEXT,
                phone TEXT,
                email TEXT,
                address TEXT,
                logo_path TEXT,
                default_tax_rate REAL NOT NULL DEFAULT 0,
                default_markup REAL NOT NULL DEFAULT 0,
                currency_code TEXT NOT NULL DEFAULT 'USD',
                quote_valid_days INTEGER NOT NULL DEFAULT 14,
                quote_number_prefix TEXT NOT NULL DEFAULT 'QC-',
                custom_footer TEXT,
                updated_at TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS catalog_items (
                id TEXT PRIMARY KEY,
                description TEXT NOT NULL,
                unit_price REAL NOT NULL,
                category TEXT NOT NULL DEFAULT '',
                sort_order INTEGER NOT NULL DEFAULT 0,
                updated_at TEXT NOT NULL,
                is_deleted INTEGER NOT NULL DEFAULT 0
            );
            """;
        await cmd.ExecuteNonQueryAsync();

        var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM catalog_items";
        var count = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        if (count == 0)
        {
            await SeedCatalogItemsAsync(connection);
        }

        await RunMigrationsAsync(connection);
    }

    private static async Task RunMigrationsAsync(SqliteConnection connection)
    {
        var versionCmd = connection.CreateCommand();
        versionCmd.CommandText = "SELECT COALESCE(MAX(version), 0) FROM schema_version";
        var currentVersion = Convert.ToInt32(await versionCmd.ExecuteScalarAsync());

        // Each migration runs once, in its own transaction, in version order.
        var migrations = new SortedDictionary<int, Func<SqliteConnection, SqliteTransaction, Task>>
        {
            { 2, Sql("") },

            { 3, Sql("""
                ALTER TABLE quotes ADD COLUMN synced_at TEXT;
                ALTER TABLE quotes ADD COLUMN share_token TEXT;
                ALTER TABLE clients ADD COLUMN synced_at TEXT;
                ALTER TABLE line_items ADD COLUMN synced_at TEXT;
                ALTER TABLE business_profile ADD COLUMN synced_at TEXT;
                ALTER TABLE catalog_items ADD COLUMN synced_at TEXT;

                CREATE TABLE IF NOT EXISTS status_history (
                    id TEXT PRIMARY KEY,
                    quote_id TEXT NOT NULL,
                    status TEXT NOT NULL,
                    changed_at TEXT NOT NULL,
                    changed_by TEXT,
                    FOREIGN KEY (quote_id) REFERENCES quotes(id)
                );

                CREATE TABLE IF NOT EXISTS notifications (
                    id TEXT PRIMARY KEY,
                    type TEXT NOT NULL,
                    title TEXT NOT NULL,
                    message TEXT NOT NULL,
                    quote_id TEXT,
                    is_read INTEGER NOT NULL DEFAULT 0,
                    created_at TEXT NOT NULL,
                    synced_at TEXT
                );

                CREATE INDEX IF NOT EXISTS idx_status_history_quote_id ON status_history(quote_id);
                CREATE INDEX IF NOT EXISTS idx_notifications_is_read ON notifications(is_read);
                CREATE INDEX IF NOT EXISTS idx_quotes_share_token ON quotes(share_token);
                """) },

            { 4, Sql("""
                ALTER TABLE business_profile ADD COLUMN website TEXT;
                ALTER TABLE business_profile ADD COLUMN business_number TEXT;
                """) },

            // Migration 5 once set the status and dates of the old sample quotes, found by title, so it rewrote any
            // user quote that happened to share one of those titles. It is a no-op now; the number stays taken.
            { 5, Sql("") },

            { 6, NumberQuotesPerYearAsync },
        };

        foreach (var (version, migrate) in migrations)
        {
            if (version <= currentVersion) continue;

            using var tx = connection.BeginTransaction();
            try
            {
                await migrate(connection, tx);

                var insertVersion = connection.CreateCommand();
                insertVersion.CommandText = "INSERT INTO schema_version (version, applied_at) VALUES (@v, @t)";
                insertVersion.Parameters.AddWithValue("@v", version);
                insertVersion.Parameters.AddWithValue("@t", DateTimeOffset.UtcNow.ToString("O"));
                insertVersion.Transaction = tx;
                await insertVersion.ExecuteNonQueryAsync();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // Ensure version 1 is recorded for fresh installs
        if (currentVersion == 0)
        {
            var insertV1 = connection.CreateCommand();
            insertV1.CommandText = "INSERT OR IGNORE INTO schema_version (version, applied_at) VALUES (1, @t)";
            insertV1.Parameters.AddWithValue("@t", DateTimeOffset.UtcNow.ToString("O"));
            await insertV1.ExecuteNonQueryAsync();
        }
    }

    private static Func<SqliteConnection, SqliteTransaction, Task> Sql(string sql) => async (connection, tx) =>
    {
        if (!string.IsNullOrWhiteSpace(sql))
        {
            await ExecuteAsync(connection, tx, sql);
        }
    };

    private static async Task ExecuteAsync(SqliteConnection connection, SqliteTransaction tx, string sql)
    {
        var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Migration 6. Quote numbers came from the quote count plus one, so a delete made the next quote reuse a number,
    /// and the PDF export (named after the number) overwrote the other quote's file. Numbers now come from a per-year
    /// counter that never goes back, and a unique index keeps them distinct. Quotes that already share a number keep
    /// it on the first one (oldest, not deleted first); the others take the next numbers of their year.
    /// </summary>
    private static async Task NumberQuotesPerYearAsync(SqliteConnection connection, SqliteTransaction tx)
    {
        await ExecuteAsync(connection, tx,
            "CREATE TABLE quote_counters (year INTEGER PRIMARY KEY, last_number INTEGER NOT NULL)");

        var quotes = new List<(string Id, string Number)>();
        var read = connection.CreateCommand();
        read.Transaction = tx;
        read.CommandText = "SELECT id, quote_number FROM quotes ORDER BY is_deleted, created_at, rowid";
        using (var reader = await read.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                quotes.Add((reader.GetString(0), reader.GetString(1)));
            }
        }

        // Every counter starts after the highest number its year already has.
        var lastByYear = new Dictionary<int, int>();
        foreach (var (_, number) in quotes)
        {
            if (QuoteNumberGenerator.TryParse(number, out _, out var year, out var sequence))
            {
                lastByYear[year] = Math.Max(lastByYear.GetValueOrDefault(year), sequence);
            }
        }

        var taken = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (id, number) in quotes)
        {
            if (taken.Add(number))
            {
                continue;
            }

            string renumbered;
            if (QuoteNumberGenerator.TryParse(number, out var prefix, out var year, out _))
            {
                do
                {
                    lastByYear[year]++;
                    renumbered = QuoteNumberGenerator.Format(prefix, year, lastByYear[year]);
                }
                while (!taken.Add(renumbered));
            }
            else
            {
                var suffix = 2;
                do
                {
                    renumbered = $"{number}-{suffix++}";
                }
                while (!taken.Add(renumbered));
            }

            var update = connection.CreateCommand();
            update.Transaction = tx;
            update.CommandText = "UPDATE quotes SET quote_number = @number, updated_at = @now WHERE id = @id";
            update.Parameters.AddWithValue("@number", renumbered);
            update.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.ToString("O"));
            update.Parameters.AddWithValue("@id", id);
            await update.ExecuteNonQueryAsync();
        }

        foreach (var (year, last) in lastByYear)
        {
            var insert = connection.CreateCommand();
            insert.Transaction = tx;
            insert.CommandText = "INSERT INTO quote_counters (year, last_number) VALUES (@year, @last)";
            insert.Parameters.AddWithValue("@year", year);
            insert.Parameters.AddWithValue("@last", last);
            await insert.ExecuteNonQueryAsync();
        }

        await ExecuteAsync(connection, tx, "CREATE UNIQUE INDEX ux_quotes_quote_number ON quotes(quote_number)");
    }

    private static async Task SeedCatalogItemsAsync(SqliteConnection connection)
    {
        var now = DateTimeOffset.UtcNow.ToString("O");
        var seeds = new (string desc, double price, string cat, int sort)[]
        {
            // Plumbing
            ("Sink Installation", 350.00, "Plumbing", 1),
            ("Toilet Installation", 275.00, "Plumbing", 2),
            ("Water Heater Installation", 850.00, "Plumbing", 3),
            ("Dishwasher Hookup", 225.00, "Plumbing", 4),
            ("Faucet Replacement", 150.00, "Plumbing", 5),
            ("Pipe Leak Repair", 200.00, "Plumbing", 6),
            ("Drain Cleaning", 175.00, "Plumbing", 7),
            ("Valve Replacement", 125.00, "Plumbing", 8),

            // Electrical
            ("Outlet Installation", 185.00, "Electrical", 1),
            ("Light Fixture Install", 150.00, "Electrical", 2),
            ("Ceiling Fan Install", 225.00, "Electrical", 3),
            ("Panel Upgrade (100A)", 1800.00, "Electrical", 4),
            ("Panel Upgrade (200A)", 2800.00, "Electrical", 5),
            ("GFCI Outlet Install", 120.00, "Electrical", 6),
            ("Dedicated Circuit", 350.00, "Electrical", 7),
            ("EV Charger Install (Level 2)", 950.00, "Electrical", 8),
            ("Smoke Detector Install", 85.00, "Electrical", 9),
            ("Recessed Lighting (per unit)", 175.00, "Electrical", 10),
            ("Whole-House Surge Protector", 450.00, "Electrical", 11),
            ("Electrical Troubleshooting (per hr)", 110.00, "Electrical", 12),

            // General Contracting
            ("Drywall Repair (per sheet)", 75.00, "General Contracting", 1),
            ("Drywall Hang + Finish (per sheet)", 125.00, "General Contracting", 2),
            ("Door Installation (interior)", 250.00, "General Contracting", 3),
            ("Door Installation (exterior)", 450.00, "General Contracting", 4),
            ("Window Installation", 550.00, "General Contracting", 5),
            ("Baseboard Trim (per ft)", 8.50, "General Contracting", 6),
            ("Crown Molding (per ft)", 12.00, "General Contracting", 7),
            ("Deck Board (per sq ft)", 22.00, "General Contracting", 8),
            ("Fence Panel Install (per panel)", 195.00, "General Contracting", 9),
            ("Tile Install (per sq ft)", 14.00, "General Contracting", 10),
            ("Flooring Install (per sq ft)", 9.50, "General Contracting", 11),
            ("Demolition (per hr)", 85.00, "General Contracting", 12),
            ("Concrete Patch (per sq ft)", 18.00, "General Contracting", 13),

            // Painting
            ("Interior Wall Paint (per room)", 350.00, "Painting", 1),
            ("Interior Ceiling Paint (per room)", 225.00, "Painting", 2),
            ("Exterior Siding Paint (per side)", 600.00, "Painting", 3),
            ("Trim & Baseboard Paint (per room)", 150.00, "Painting", 4),
            ("Cabinet Refinishing (per unit)", 175.00, "Painting", 5),
            ("Deck/Fence Stain (per sq ft)", 3.50, "Painting", 6),
            ("Wallpaper Removal (per room)", 275.00, "Painting", 7),
            ("Pressure Washing (per sq ft)", 0.35, "Painting", 8),
            ("Primer Coat (per room)", 125.00, "Painting", 9),
            ("Touch-Up / Patch Paint", 95.00, "Painting", 10),

            // HVAC
            ("Furnace Tune-Up", 150.00, "HVAC", 1),
            ("AC Tune-Up", 150.00, "HVAC", 2),
            ("Thermostat Install (smart)", 275.00, "HVAC", 3),
            ("Duct Cleaning (whole house)", 450.00, "HVAC", 4),
            ("Filter Replacement", 45.00, "HVAC", 5),
            ("Refrigerant Recharge", 350.00, "HVAC", 6),
            ("Furnace Install", 4500.00, "HVAC", 7),
            ("AC Unit Install", 5200.00, "HVAC", 8),
            ("Mini-Split Install", 3800.00, "HVAC", 9),
            ("Duct Repair / Sealing", 250.00, "HVAC", 10),

            // Common Materials
            ("Copper Pipe (per ft)", 12.50, "Materials", 1),
            ("PVC Pipe (per ft)", 4.50, "Materials", 2),
            ("Pipe Fittings (set)", 35.00, "Materials", 3),
            ("Sealant / Adhesive", 18.00, "Materials", 4),
            ("Electrical Wire 14/2 (per ft)", 1.25, "Materials", 5),
            ("Electrical Wire 12/2 (per ft)", 1.75, "Materials", 6),
            ("Lumber 2x4 (8ft)", 6.50, "Materials", 7),
            ("Plywood Sheet (4x8)", 55.00, "Materials", 8),

            // Labor
            ("Standard Labor (per hr)", 95.00, "Labor", 1),
            ("Emergency Labor (per hr)", 150.00, "Labor", 2),
            ("Apprentice Labor (per hr)", 55.00, "Labor", 3),
            ("Overtime Labor (per hr)", 142.50, "Labor", 4),
        };

        foreach (var (desc, price, cat, sort) in seeds)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = """
                INSERT INTO catalog_items (id, description, unit_price, category, sort_order, updated_at, is_deleted)
                VALUES (@id, @description, @unit_price, @category, @sort_order, @updated_at, 0)
                """;
            cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@description", desc);
            cmd.Parameters.AddWithValue("@unit_price", price);
            cmd.Parameters.AddWithValue("@category", cat);
            cmd.Parameters.AddWithValue("@sort_order", sort);
            cmd.Parameters.AddWithValue("@updated_at", now);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
