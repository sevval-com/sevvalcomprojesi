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
        public async Task<IActionResult> GetCategories()
        {
            if (User.Identity?.Name != "sftumen41@gmail.com")
            {
                return Forbid();
            }

            try
            {
                // Categories tablosundan kategorileri al
                var sql = @"
                    SELECT Id, Name, Icon, Color, CreatedAt, UpdatedAt 
                    FROM Categories 
                    ORDER BY Name";

                var categories = new List<object>();
                var idIsText = IsCategoriesIdText();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _context.Database.OpenConnection();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Resolve ordinals once
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
                            {
                                id = string.Empty;
                            }
                            else if (idIsText)
                            {
                                id = reader.GetString(idOrdinal);
                            }
                            else
                            {
                                id = reader.GetInt32(idOrdinal).ToString();
                            }
                            var name = reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal);
                            var icon = reader.IsDBNull(iconOrdinal) ? "" : reader.GetString(iconOrdinal);
                            var color = reader.IsDBNull(colorOrdinal) ? "bg-blue-100 text-blue-800 border-blue-200" : reader.GetString(colorOrdinal);

                            // Dates may be stored as TEXT in SQLite; parse safely
                            DateTime createdAt;
                            DateTime updatedAt;
                            if (!reader.IsDBNull(createdAtOrdinal))
                            {
                                if (reader.GetFieldType(createdAtOrdinal) == typeof(DateTime))
                                {
                                    createdAt = reader.GetDateTime(createdAtOrdinal);
                                }
                                else
                                {
                                    DateTime.TryParse(reader.GetString(createdAtOrdinal), out createdAt);
                                }
                            }
                            else
                            {
                                createdAt = DateTime.MinValue;
                            }

                            if (!reader.IsDBNull(updatedAtOrdinal))
                            {
                                if (reader.GetFieldType(updatedAtOrdinal) == typeof(DateTime))
                                {
                                    updatedAt = reader.GetDateTime(updatedAtOrdinal);
                                }
                                else
                                {
                                    DateTime.TryParse(reader.GetString(updatedAtOrdinal), out updatedAt);
                                }
                            }
                            else
                            {
                                updatedAt = DateTime.MinValue;
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

                return Json(new { success = true, categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kategoriler yüklenemedi: " + ex.Message });
            }
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
