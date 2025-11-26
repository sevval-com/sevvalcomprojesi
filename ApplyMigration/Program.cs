using System;
using System.Data;
using Microsoft.Data.Sqlite;

var dbPath = @"C:\Users\Dell\Sevval\Src\Presentation\Sevval.Api\sevvalemlak2.db";
var connectionString = $"Data Source={dbPath}";

Console.WriteLine("Connecting to database...");

using var connection = new SqliteConnection(connectionString);
connection.Open();

Console.WriteLine("Connected successfully!");

// Create DeletedAccounts table
var createTableSql = @"
CREATE TABLE IF NOT EXISTS DeletedAccounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    DeletedAt TEXT NOT NULL,
    DeletionReason TEXT
);";

using (var command = connection.CreateCommand())
{
    command.CommandText = createTableSql;
    command.ExecuteNonQuery();
    Console.WriteLine("✓ DeletedAccounts table created");
}

// Create indexes
var createIndexSql1 = "CREATE INDEX IF NOT EXISTS IX_DeletedAccounts_UserId ON DeletedAccounts(UserId);";
var createIndexSql2 = "CREATE INDEX IF NOT EXISTS IX_DeletedAccounts_DeletedAt ON DeletedAccounts(DeletedAt);";

using (var command = connection.CreateCommand())
{
    command.CommandText = createIndexSql1;
    command.ExecuteNonQuery();
    Console.WriteLine("✓ Index IX_DeletedAccounts_UserId created");
    
    command.CommandText = createIndexSql2;
    command.ExecuteNonQuery();
    Console.WriteLine("✓ Index IX_DeletedAccounts_DeletedAt created");
}

// Add RecoveryToken column
try
{
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "ALTER TABLE DeletedAccounts ADD COLUMN RecoveryToken TEXT;";
        command.ExecuteNonQuery();
        Console.WriteLine("✓ RecoveryToken column added");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ RecoveryToken column might already exist: {ex.Message}");
}

// Create index on RecoveryToken
var createIndexSql3 = "CREATE INDEX IF NOT EXISTS IX_DeletedAccounts_RecoveryToken ON DeletedAccounts(RecoveryToken);";
using (var command = connection.CreateCommand())
{
    command.CommandText = createIndexSql3;
    command.ExecuteNonQuery();
    Console.WriteLine("✓ Index IX_DeletedAccounts_RecoveryToken created");
}

// Add migration record
var insertMigrationSql = @"
INSERT OR IGNORE INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20251126101836_AddDeletedAccountsTable', '8.0.17');";

using (var command = connection.CreateCommand())
{
    command.CommandText = insertMigrationSql;
    command.ExecuteNonQuery();
    Console.WriteLine("✓ Migration record added to history");
}

// Add new migration record for RecoveryToken
var insertMigrationSql2 = @"
INSERT OR IGNORE INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20251126_AddRecoveryToken', '8.0.17');";

using (var command = connection.CreateCommand())
{
    command.CommandText = insertMigrationSql2;
    command.ExecuteNonQuery();
    Console.WriteLine("✓ RecoveryToken migration record added");
}

// Verify table structure
using (var command = connection.CreateCommand())
{
    command.CommandText = "PRAGMA table_info(DeletedAccounts);";
    using var reader = command.ExecuteReader();
    
    Console.WriteLine("\n📋 DeletedAccounts table structure:");
    while (reader.Read())
    {
        Console.WriteLine($"  - {reader["name"]}: {reader["type"]}");
    }
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
