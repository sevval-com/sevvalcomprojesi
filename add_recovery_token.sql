-- Add RecoveryToken column to DeletedAccounts table
ALTER TABLE DeletedAccounts ADD COLUMN RecoveryToken TEXT;

-- Create index on RecoveryToken for faster lookups
CREATE INDEX IF NOT EXISTS IX_DeletedAccounts_RecoveryToken ON DeletedAccounts(RecoveryToken);
