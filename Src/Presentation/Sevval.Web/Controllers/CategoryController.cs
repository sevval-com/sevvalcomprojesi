using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevval.Web.Models;
using Sevval.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Sevval.Domain.Entities;

namespace Sevval.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private bool? _categoriesIdIsText;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsCategoriesIdText()
        {
            if (_categoriesIdIsText.HasValue) return _categoriesIdIsText.Value;
            // SQLite PRAGMA table_info ile Id tipini tespit etmeye çalış
            try
            {
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(Categories)";
                    _context.Database.OpenConnection();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader[1]?.ToString(); // name column
                            var type = reader[2]?.ToString()?.ToUpperInvariant(); // type column
                            if (string.Equals(name, "Id", StringComparison.OrdinalIgnoreCase))
                            {
                                _categoriesIdIsText = !(type?.Contains("INT") == true);
                                return _categoriesIdIsText.Value;
                            }
                        }
                    }
                }
            }
            catch { }
            _categoriesIdIsText = false; // varsayılan INT kabul et
            return _categoriesIdIsText.Value;
        }

        public IActionResult Index()
        {
            // Sadece sftumen41 hesabı erişebilir
            if (User.Identity?.Name != "sftumen41@gmail.com")
            {
                return Forbid();
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                // Önce Categories tablosunun var olup olmadığını kontrol et
                var tableExists = false;
                using (var checkCmd = connection.CreateCommand())
                {
                    checkCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Categories'";
                    var result = await checkCmd.ExecuteScalarAsync();
                    tableExists = result != null;
                }

                // Tablo yoksa oluştur ve mevcut kategorileri ekle
                if (!tableExists)
                {
                    using (var createCmd = connection.CreateCommand())
                    {
                        createCmd.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Categories (
                                Id TEXT PRIMARY KEY,
                                Name TEXT NOT NULL,
                                Icon TEXT,
                                Color TEXT,
                                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                            )";
                        await createCmd.ExecuteNonQueryAsync();
                    }

                    // VideolarSayfasi'ndaki mevcut kategorileri al ve ekle
                    var existingCategories = await _context.VideolarSayfasi
                        .Where(v => !string.IsNullOrEmpty(v.Kategori))
                        .Select(v => v.Kategori)
                        .Distinct()
                        .ToListAsync();

                    foreach (var cat in existingCategories)
                    {
                        var catName = cat?.Trim().TrimEnd('.') ?? "";
                        if (string.IsNullOrEmpty(catName)) continue;

                        using (var insertCmd = connection.CreateCommand())
                        {
                            insertCmd.CommandText = @"
                                INSERT OR IGNORE INTO Categories (Id, Name, Icon, Color, CreatedAt, UpdatedAt)
                                VALUES (@id, @name, @icon, @color, datetime('now'), datetime('now'))";
                            
                            var idParam = insertCmd.CreateParameter();
                            idParam.ParameterName = "@id";
                            idParam.Value = Guid.NewGuid().ToString();
                            insertCmd.Parameters.Add(idParam);

                            var nameParam = insertCmd.CreateParameter();
                            nameParam.ParameterName = "@name";
                            nameParam.Value = catName;
                            insertCmd.Parameters.Add(nameParam);

                            var iconParam = insertCmd.CreateParameter();
                            iconParam.ParameterName = "@icon";
                            iconParam.Value = "";
                            insertCmd.Parameters.Add(iconParam);

                            var colorParam = insertCmd.CreateParameter();
                            colorParam.ParameterName = "@color";
                            colorParam.Value = "bg-blue-100 text-blue-800 border-blue-200";
                            insertCmd.Parameters.Add(colorParam);

                            await insertCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                // Categories tablosundan kategorileri al
                var categories = new List<object>();
                var idIsText = IsCategoriesIdText();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, Name, Icon, Color, CreatedAt, UpdatedAt 
                        FROM Categories 
                        ORDER BY Name";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var idOrdinal = reader.GetOrdinal("Id");
                        var nameOrdinal = reader.GetOrdinal("Name");
                        var iconOrdinal = reader.GetOrdinal("Icon");
                        var colorOrdinal = reader.GetOrdinal("Color");
                        var createdAtOrdinal = reader.GetOrdinal("CreatedAt");
                        var updatedAtOrdinal = reader.GetOrdinal("UpdatedAt");

                        while (await reader.ReadAsync())
                        {
                            string id;
                            if (reader.IsDBNull(idOrdinal))
                                id = string.Empty;
                            else if (idIsText)
                                id = reader.GetString(idOrdinal);
                            else
                                id = reader.GetInt32(idOrdinal).ToString();

                            var name = reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal);
                            var icon = reader.IsDBNull(iconOrdinal) ? "" : reader.GetString(iconOrdinal);
                            var color = reader.IsDBNull(colorOrdinal) ? "bg-blue-100 text-blue-800 border-blue-200" : reader.GetString(colorOrdinal);

                            DateTime createdAt = DateTime.MinValue;
                            DateTime updatedAt = DateTime.MinValue;

                            if (!reader.IsDBNull(createdAtOrdinal))
                            {
                                var createdAtValue = reader.GetValue(createdAtOrdinal);
                                if (createdAtValue is DateTime dt)
                                    createdAt = dt;
                                else
                                    DateTime.TryParse(createdAtValue?.ToString(), out createdAt);
                            }

                            if (!reader.IsDBNull(updatedAtOrdinal))
                            {
                                var updatedAtValue = reader.GetValue(updatedAtOrdinal);
                                if (updatedAtValue is DateTime dt)
                                    updatedAt = dt;
                                else
                                    DateTime.TryParse(updatedAtValue?.ToString(), out updatedAt);
                            }

                            categories.Add(new
                            {
                                id,
                                name,
                                icon,
                                color,
                                createdAt,
                                updatedAt
                            });
                        }
                    }
                }

                if (categories.Count == 0)
                {
                    await SeedDefaultCategoryAsync(connection);
                    categories.Add(new { 
                        id = Guid.NewGuid().ToString(), 
                        name = "Genel", 
                        icon = "", 
                        color = "bg-blue-100 text-blue-800 border-blue-200", 
                        createdAt = DateTime.UtcNow, 
                        updatedAt = DateTime.UtcNow 
                    });
                }

                return Json(new { success = true, categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kategoriler yüklenemedi: " + ex.Message });
            }
        }

        private async Task SeedDefaultCategoryAsync(System.Data.Common.DbConnection connection)
        {
             try
             {
                var categoryName = "Genel";
                var categoryColor = "bg-blue-100 text-blue-800 border-blue-200";
                
                using (var insertCmd = connection.CreateCommand())
                {
                    insertCmd.CommandText = @"
                        INSERT INTO Categories (Id, Name, Icon, Color, CreatedAt, UpdatedAt)
                        VALUES (@id, @name, @icon, @color, datetime('now'), datetime('now'))";

                    if (!IsCategoriesIdText())
                    {
                         insertCmd.CommandText = @"
                        INSERT INTO Categories (Name, Icon, Color, CreatedAt, UpdatedAt)
                        VALUES (@name, @icon, @color, datetime('now'), datetime('now'))";
                    }
                    else
                    {
                        var idParam = insertCmd.CreateParameter();
                        idParam.ParameterName = "@id";
                        idParam.Value = Guid.NewGuid().ToString();
                        insertCmd.Parameters.Add(idParam);
                    }

                    var nameParam = insertCmd.CreateParameter();
                    nameParam.ParameterName = "@name";
                    nameParam.Value = categoryName;
                    insertCmd.Parameters.Add(nameParam);

                    var iconParam = insertCmd.CreateParameter();
                    iconParam.ParameterName = "@icon";
                    iconParam.Value = "";
                    insertCmd.Parameters.Add(iconParam);

                    var colorParam = insertCmd.CreateParameter();
                    colorParam.ParameterName = "@color";
                    colorParam.Value = categoryColor;
                    insertCmd.Parameters.Add(colorParam);

                    await insertCmd.ExecuteNonQueryAsync();
                }
             }
             catch { }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (User.Identity?.Name != "sftumen41@gmail.com")
            {
                return Forbid();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Json(new { success = false, message = "Kategori adı gerekli" });
                }

                // Kategori zaten var mı kontrol et
                var existingCategory = await _context.Categories
                    .AnyAsync(c => c.Name == request.Name.Trim());

                if (existingCategory)
                {
                    return Json(new { success = false, message = "Bu kategori zaten mevcut" });
                }

                var categoryName = request.Name.Trim();
                var categoryIcon = request.Icon ?? string.Empty;
                var categoryColor = request.Color ?? "bg-blue-100 text-blue-800 border-blue-200";

                // Id sütunu tipine göre ekleme yap (TEXT ise Id'yi biz üretelim, INTEGER ise DB üretsin)
                string? insertedIdText = null;
                int? insertedIdInt = null;

                if (IsCategoriesIdText())
                {
                    // TEXT Id: GUID üret ve Id ile birlikte ekle
                    insertedIdText = Guid.NewGuid().ToString();
                    var insertSql = @"INSERT INTO Categories (Id, Name, Icon, Color, CreatedAt, UpdatedAt)
                                       VALUES (@Id, @Name, @Icon, @Color, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = insertSql;
                        command.Parameters.Add(new SqliteParameter("@Id", insertedIdText) { DbType = System.Data.DbType.String });
                        command.Parameters.Add(new SqliteParameter("@Name", categoryName) { DbType = System.Data.DbType.String });
                        command.Parameters.Add(new SqliteParameter("@Icon", categoryIcon) { DbType = System.Data.DbType.String });
                        command.Parameters.Add(new SqliteParameter("@Color", categoryColor) { DbType = System.Data.DbType.String });

                        _context.Database.OpenConnection();
                        await command.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    // INTEGER Id: Id kolonunu belirtmeden ekle, ardından last_insert_rowid() al
                    var insertSql = @"INSERT INTO Categories (Name, Icon, Color, CreatedAt, UpdatedAt)
                                       VALUES (@Name, @Icon, @Color, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = insertSql;
                        command.Parameters.Add(new SqliteParameter("@Name", categoryName) { DbType = System.Data.DbType.String });
                        command.Parameters.Add(new SqliteParameter("@Icon", categoryIcon) { DbType = System.Data.DbType.String });
                        command.Parameters.Add(new SqliteParameter("@Color", categoryColor) { DbType = System.Data.DbType.String });

                        _context.Database.OpenConnection();
                        await command.ExecuteNonQueryAsync();
                    }

                    using (var idCmd = _context.Database.GetDbConnection().CreateCommand())
                    {
                        idCmd.CommandText = "SELECT last_insert_rowid()";
                        _context.Database.OpenConnection();
                        var scalar = await idCmd.ExecuteScalarAsync();
                        if (scalar != null && int.TryParse(Convert.ToString(scalar), out var lastId))
                        {
                            insertedIdInt = lastId;
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Kategori başarıyla eklendi",
                    category = new
                    {
                        id = (object?)insertedIdInt ?? insertedIdText,
                        name = categoryName,
                        icon = categoryIcon,
                        color = categoryColor
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kategori eklenemedi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request)
        {
            if (User.Identity?.Name != "sftumen41@gmail.com")
            {
                return Forbid();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return Json(new { success = false, message = "Geçersiz kategori Id" });
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Json(new { success = false, message = "Kategori adı gerekli" });
                }

                // Aynı isimde başka kategori var mı? (kendisi hariç)
                var nameExists = await _context.Categories
                    .AnyAsync(c => c.Name == request.Name.Trim() && c.Id != request.Id);
                if (nameExists)
                {
                    return Json(new { success = false, message = "Bu kategori adı zaten mevcut" });
                }

                var updateSql = @"UPDATE Categories 
                                   SET Name = @Name, Icon = @Icon, Color = @Color, UpdatedAt = CURRENT_TIMESTAMP 
                                   WHERE Id = @Id";

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = updateSql;
                    command.Parameters.Add(new SqliteParameter("@Name", request.Name.Trim()) { DbType = System.Data.DbType.String });
                    command.Parameters.Add(new SqliteParameter("@Icon", request.Icon ?? string.Empty) { DbType = System.Data.DbType.String });
                    command.Parameters.Add(new SqliteParameter("@Color", request.Color ?? "bg-blue-100 text-blue-800 border-blue-200") { DbType = System.Data.DbType.String });
                    // Tablo Id'niz INTEGER ise Int32, TEXT ise String kullanın. Biz SELECT'te Id'yi int okuyoruz,
                    // fakat DB tarafında TEXT ise stringe çevirerek gönderiyoruz.
                    var idParam = new SqliteParameter("@Id", request.Id);
                    if (IsCategoriesIdText())
                    {
                        idParam.Value = request.Id;
                        idParam.DbType = System.Data.DbType.String;
                    }
                    else
                    {
                        if (!int.TryParse(request.Id, out var idInt))
                        {
                            return Json(new { success = false, message = "Geçersiz kategori Id" });
                        }
                        idParam.Value = idInt;
                        idParam.DbType = System.Data.DbType.Int32;
                    }
                    command.Parameters.Add(idParam);

                    _context.Database.OpenConnection();
                    var affected = await command.ExecuteNonQueryAsync();
                    if (affected == 0)
                    {
                        return Json(new { success = false, message = "Kategori bulunamadı" });
                    }
                }

                return Json(new { success = true, message = "Kategori güncellendi", category = new { id = request.Id, name = request.Name.Trim(), icon = request.Icon ?? string.Empty, color = request.Color ?? "bg-blue-100 text-blue-800 border-blue-200" } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteCategoryRequest request)
        {
            if (User.Identity?.Name != "sftumen41@gmail.com")
            {
                return Forbid();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(request.Id) && string.IsNullOrWhiteSpace(request.Name))
                {
                    return Json(new { success = false, message = "Geçersiz istek" });
                }

                // Bu kategori ismi ile ilişkili video var mı? (şu an videolarda CategoryId yerine string Kategori kullanılıyor)
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var hasVideos = await _context.VideolarSayfasi.AnyAsync(v => v.Kategori == request.Name);
                    if (hasVideos)
                    {
                        return Json(new { success = false, message = "Bu kategoriye ait videolar var. Önce videoları silin veya başka kategoriye taşıyın." });
                    }
                }

                // Videoları ve ilişkili kayıtları sil, ardından kategoriyi sil
                await using var tx = await _context.Database.BeginTransactionAsync();

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var videos = await _context.VideolarSayfasi
                        .Where(v => v.Kategori == request.Name)
                        .ToListAsync();

                    foreach (var video in videos)
                    {
                        var likes = await _context.VideoLikes.Where(l => l.VideoId == video.Id).ToListAsync();
                        var comments = await _context.VideoYorumlari.Where(y => y.VideoId == video.Id).ToListAsync();
                        var watches = await _context.VideoWatches.Where(w => w.VideoId == video.Id).ToListAsync();

                        _context.VideoLikes.RemoveRange(likes);
                        _context.VideoYorumlari.RemoveRange(comments);
                        _context.VideoWatches.RemoveRange(watches);
                        _context.VideolarSayfasi.Remove(video);
                    }

                    await _context.SaveChangesAsync();
                }

                // Kategori kaydını sil
                var deleteSql = "DELETE FROM Categories WHERE Id = @Id";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = deleteSql;
                    var idParam = new SqliteParameter("@Id", request.Id);
                    if (IsCategoriesIdText())
                    {
                        idParam.Value = request.Id;
                        idParam.DbType = System.Data.DbType.String;
                    }
                    else
                    {
                        if (!int.TryParse(request.Id, out var idInt))
                        {
                            return Json(new { success = false, message = "Geçersiz kategori Id" });
                        }
                        idParam.Value = idInt;
                        idParam.DbType = System.Data.DbType.Int32;
                    }
                    command.Parameters.Add(idParam);

                    _context.Database.OpenConnection();
                    var affected = await command.ExecuteNonQueryAsync();
                    if (affected == 0)
                    {
                        await tx.RollbackAsync();
                        return Json(new { success = false, message = "Kategori bulunamadı" });
                    }
                }

                await tx.CommitAsync();

                return Json(new { success = true, message = "Kategori ve ilişkili videolar silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    public class DeleteCategoryRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
