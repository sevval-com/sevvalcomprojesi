$dbPath = "C:\Users\Dell\Sevval\Src\Presentation\Sevval.Api\sevvalemlak2.db"
$sqlFile = "C:\Users\Dell\Sevval\add_deleted_accounts_table.sql"

Write-Host "Loading SQL commands..." -ForegroundColor Cyan
$sql = Get-Content $sqlFile -Raw

try {
    # Load SQLite assembly from NuGet package
    $sqliteDll = Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\microsoft.data.sqlite.core" -Filter "Microsoft.Data.Sqlite.dll" -Recurse | Select-Object -First 1
    
    if ($sqliteDll) {
        Add-Type -Path $sqliteDll.FullName
        Write-Host "SQLite assembly loaded from: $($sqliteDll.FullName)" -ForegroundColor Green
    } else {
        Write-Host "SQLite assembly not found in NuGet cache. Installing..." -ForegroundColor Yellow
        dotnet add package Microsoft.Data.Sqlite --version 8.0.0
        $sqliteDll = Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\microsoft.data.sqlite.core" -Filter "Microsoft.Data.Sqlite.dll" -Recurse | Select-Object -First 1
        Add-Type -Path $sqliteDll.FullName
    }

    # Create connection
    $connectionString = "Data Source=$dbPath"
    $connection = New-Object Microsoft.Data.Sqlite.SqliteConnection($connectionString)
    $connection.Open()
    Write-Host "Connected to database: $dbPath" -ForegroundColor Green

    # Execute SQL commands
    $commands = $sql -split ';' | Where-Object { $_.Trim() -ne '' }
    
    foreach ($cmdText in $commands) {
        if ($cmdText.Trim() -ne '') {
            try {
                $command = $connection.CreateCommand()
                $command.CommandText = $cmdText.Trim()
                $result = $command.ExecuteNonQuery()
                Write-Host "✓ Executed: $($cmdText.Substring(0, [Math]::Min(50, $cmdText.Length)))..." -ForegroundColor Gray
            } catch {
                Write-Host "⚠ Warning: $($_.Exception.Message)" -ForegroundColor Yellow
            }
        }
    }

    # Verify table exists
    $verifyCommand = $connection.CreateCommand()
    $verifyCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='DeletedAccounts';"
    $reader = $verifyCommand.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "`n✅ SUCCESS: DeletedAccounts table created!" -ForegroundColor Green
    } else {
        Write-Host "`n❌ ERROR: Table was not created!" -ForegroundColor Red
    }
    $reader.Close()

    $connection.Close()
    Write-Host "`nDatabase connection closed." -ForegroundColor Cyan

} catch {
    Write-Host "`n❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.Exception.StackTrace -ForegroundColor DarkRed
}

Write-Host "`nPress any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
