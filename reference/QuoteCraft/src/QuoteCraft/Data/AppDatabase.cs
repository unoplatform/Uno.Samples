using Microsoft.Data.Sqlite;

namespace QuoteCraft.Data;

public class AppDatabase
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SqliteConnection? _sharedConnection;
    private bool _initialized;

    public AppDatabase()
    {
        SQLitePCL.Batteries_V2.Init();

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuoteCraft",
            "quotecraft.db");

        var dir = Path.GetDirectoryName(dbPath)!;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

#if __BROWSERWASM__
        // WASM: browser storage is sandboxed, no encryption needed
        _connectionString = $"Data Source={dbPath}";
#else
        var key = GetOrCreateEncryptionKey(dir);
        _connectionString = $"Data Source={dbPath};Password={key}";
#endif
    }

#if !__BROWSERWASM__
    private static string GetOrCreateEncryptionKey(string appDir)
    {
        var keyFile = Path.Combine(appDir, ".dbkey");
        if (File.Exists(keyFile))
        {
            return File.ReadAllText(keyFile).Trim();
        }

        // Generate a random 32-char hex key
        var keyBytes = new byte[16];
        System.Security.Cryptography.RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToHexString(keyBytes);
        File.WriteAllText(keyFile, key);
        return key;
    }
#endif

    /// <summary>
    /// Returns the shared persistent connection. Opens and initializes on first call.
    /// Do NOT dispose the returned connection — it is shared across the app lifetime.
    /// </summary>
    public async Task<SqliteConnection> GetConnectionAsync()
    {
        if (_initialized && _sharedConnection is { State: System.Data.ConnectionState.Open })
            return _sharedConnection;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized && _sharedConnection is { State: System.Data.ConnectionState.Open })
                return _sharedConnection;

            // Open persistent connection
            _sharedConnection?.Dispose();
            _sharedConnection = new SqliteConnection(_connectionString);
            await _sharedConnection.OpenAsync();

            // PRAGMAs — set once on the persistent connection
            var pragmaCmd = _sharedConnection.CreateCommand();
            pragmaCmd.CommandText = "PRAGMA foreign_keys=ON; PRAGMA busy_timeout=5000;";
            await pragmaCmd.ExecuteNonQueryAsync();

            if (!_initialized)
            {
                await InitializeCoreAsync(_sharedConnection);
                _initialized = true;
            }

            return _sharedConnection;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Keep for backward compat during init — creates a temporary connection
    [Obsolete("Use GetConnectionAsync() instead")]
    public async Task InitializeAsync()
    {
        await GetConnectionAsync();
    }

    // Keep for backward compat — but callers should migrate to GetConnectionAsync()
    [Obsolete("Use GetConnectionAsync() instead")]
    public async Task<SqliteConnection> CreateConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var fkCmd = connection.CreateCommand();
        fkCmd.CommandText = "PRAGMA foreign_keys=ON; PRAGMA busy_timeout=5000;";
        await fkCmd.ExecuteNonQueryAsync();

        return connection;
    }

    private static async Task InitializeCoreAsync(SqliteConnection connection)
    {
        // Enable WAL mode for better concurrent read performance
        var walCmd = connection.CreateCommand();
        walCmd.CommandText = "PRAGMA journal_mode=WAL;";
        await walCmd.ExecuteNonQueryAsync();

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

        // Seed catalog items if table is empty
        var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM catalog_items";
        var count = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        if (count == 0)
        {
            await SeedCatalogItemsAsync(connection);
        }

        // Run migrations
        await RunMigrationsAsync(connection);
    }

    private static async Task RunMigrationsAsync(SqliteConnection connection)
    {
        // Get current schema version
        var versionCmd = connection.CreateCommand();
        versionCmd.CommandText = "SELECT COALESCE(MAX(version), 0) FROM schema_version";
        var currentVersion = Convert.ToInt32(await versionCmd.ExecuteScalarAsync());

        var migrations = new SortedDictionary<int, string>
        {
            { 2, "" },

            { 3, """
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
                """ },

            { 4, """
                ALTER TABLE business_profile ADD COLUMN website TEXT;
                ALTER TABLE business_profile ADD COLUMN business_number TEXT;
                """ },

            { 5, $$"""
                UPDATE quotes SET status='Sent',     valid_until='{{DateTimeOffset.UtcNow.AddDays(14):O}}', updated_at='{{DateTimeOffset.UtcNow:O}}' WHERE title='Drain Cleaning Service';
                UPDATE quotes SET status='Viewed',   valid_until='{{DateTimeOffset.UtcNow.AddDays(14):O}}', updated_at='{{DateTimeOffset.UtcNow:O}}' WHERE title='Office Plumbing Fit-out';
                UPDATE quotes SET status='Draft',    valid_until='{{DateTimeOffset.UtcNow.AddDays(14):O}}', updated_at='{{DateTimeOffset.UtcNow:O}}' WHERE title='Emergency Pipe Repair';
                UPDATE quotes SET status='Sent',     valid_until='{{DateTimeOffset.UtcNow.AddDays(14):O}}', updated_at='{{DateTimeOffset.UtcNow:O}}' WHERE title='Bathroom Remodel';
                UPDATE quotes SET valid_until='{{DateTimeOffset.UtcNow.AddDays(14):O}}', updated_at='{{DateTimeOffset.UtcNow:O}}' WHERE title='Kitchen Renovation';
                UPDATE quotes SET valid_until='{{DateTimeOffset.UtcNow.AddDays(14):O}}', updated_at='{{DateTimeOffset.UtcNow:O}}' WHERE title='Water Heater Install';
                """ },
        };

        foreach (var (version, sql) in migrations)
        {
            if (version <= currentVersion) continue;

            using var tx = connection.BeginTransaction();
            try
            {
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    var migCmd = connection.CreateCommand();
                    migCmd.CommandText = sql;
                    migCmd.Transaction = tx;
                    await migCmd.ExecuteNonQueryAsync();
                }

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
