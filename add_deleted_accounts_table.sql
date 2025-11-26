-- Create DeletedAccounts table
CREATE TABLE IF NOT EXISTS DeletedAccounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    DeletedAt TEXT NOT NULL,
    DeletionReason TEXT,
    FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX IF NOT EXISTS IX_DeletedAccounts_UserId ON DeletedAccounts(UserId);
CREATE INDEX IF NOT EXISTS IX_DeletedAccounts_DeletedAt ON DeletedAccounts(DeletedAt);

-- Add migration record to history
INSERT OR IGNORE INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20251126101836_AddDeletedAccountsTable', '8.0.17');

-- Verify
SELECT 'DeletedAccounts table created successfully!' as Result;
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId DESC LIMIT 3;
