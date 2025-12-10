using System;
using System.Data;
using Microsoft.Data.Sqlite;

var dbPath = @"c:\Users\Msi\sevvalcomprojesi\Src\Presentation\Sevval.Api\sevvalemlak2.db";
var connectionString = $"Data Source={dbPath}";

Console.WriteLine("🔄 Database bağlantısı kuruluyor...");

using var connection = new SqliteConnection(connectionString);
connection.Open();

Console.WriteLine("✅ Bağlantı başarılı!");

// ========== DOCUMENT FIELDS MIGRATION ==========
Console.WriteLine("\n📝 Document1Path ve Document2Path alanları ekleniyor...");

// Document1Path ekle
try
{
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "ALTER TABLE AspNetUsers ADD COLUMN Document1Path TEXT NULL;";
        command.ExecuteNonQuery();
        Console.WriteLine("✅ Document1Path eklendi");
    }
}
catch (Exception ex)
{
    if (ex.Message.Contains("duplicate column"))
        Console.WriteLine("⚠️  Document1Path zaten var, atlanıyor");
    else
        Console.WriteLine($"❌ Document1Path hatası: {ex.Message}");
}

// Document2Path ekle
try
{
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "ALTER TABLE AspNetUsers ADD COLUMN Document2Path TEXT NULL;";
        command.ExecuteNonQuery();
        Console.WriteLine("✅ Document2Path eklendi");
    }
}
catch (Exception ex)
{
    if (ex.Message.Contains("duplicate column"))
        Console.WriteLine("⚠️  Document2Path zaten var, atlanıyor");
    else
        Console.WriteLine($"❌ Document2Path hatası: {ex.Message}");
}

// Geriye dönük veri migration
Console.WriteLine("\n🔄 Mevcut belgeler Document1/2Path'e kopyalanıyor...");
using (var command = connection.CreateCommand())
{
    command.CommandText = @"
        UPDATE AspNetUsers 
        SET Document1Path = Level5CertificatePath,
            Document2Path = TaxPlatePath
        WHERE (Level5CertificatePath IS NOT NULL OR TaxPlatePath IS NOT NULL)
          AND (Document1Path IS NULL OR Document2Path IS NULL);";
    
    int rowsAffected = command.ExecuteNonQuery();
    Console.WriteLine($"✅ {rowsAffected} kullanıcının belgeleri kopyalandı");
}

// Migration kaydı ekle
var insertMigrationSql = @"
INSERT OR IGNORE INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20251127132704_AddDocumentFieldsForAllUserTypes', '8.0.17');";

using (var command = connection.CreateCommand())
{
    command.CommandText = insertMigrationSql;
    command.ExecuteNonQuery();
    Console.WriteLine("✅ Migration kaydı eklendi");
}

// Doğrulama
Console.WriteLine("\n📊 Document alanları kontrol ediliyor...");
using (var command = connection.CreateCommand())
{
    command.CommandText = @"
        SELECT COUNT(*) as Total,
               SUM(CASE WHEN Document1Path IS NOT NULL THEN 1 ELSE 0 END) as WithDoc1,
               SUM(CASE WHEN Document2Path IS NOT NULL THEN 1 ELSE 0 END) as WithDoc2
        FROM AspNetUsers 
        WHERE UserTypes != 'Bireysel';";
    
    using var reader = command.ExecuteReader();
    if (reader.Read())
    {
        Console.WriteLine($"  📁 Toplam kurumsal kullanıcı: {reader["Total"]}");
        Console.WriteLine($"  📄 Document1Path olan: {reader["WithDoc1"]}");
        Console.WriteLine($"  📄 Document2Path olan: {reader["WithDoc2"]}");
    }
}

Console.WriteLine("\n✅ TÜM MIGRATION İŞLEMLERİ TAMAMLANDI!");
Console.WriteLine("\nDevam etmek için bir tuşa basın...");
Console.ReadKey();
