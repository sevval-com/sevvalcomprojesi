using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using Sevval.Web.Models;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class VideolarSayfasiController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public VideolarSayfasiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index(string kategori, int page = 1, int size = 12)
    {
        // 1) API'den videoları çek (ortama göre base URL)
        var isLocal = string.Equals(Request?.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(Request?.Host.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
        // Geliştirme: local API; Test/Canlı: prod API
        var apiBase = isLocal
            ? "https://localhost:44347/api/v1"
            : "http://94.73.131.202:8090/api/v1";
        var apiUrl = $"{apiBase}/videos";
        List<Sevval.Web.Models.VideolarSayfasi> videolar = new();
        try
        {
            using var http = new HttpClient();
            using var resp = await http.GetAsync(apiUrl);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var parsed = System.Text.Json.JsonSerializer.Deserialize<ApiVideosResponse>(json, options);
            var data = parsed?.Data ?? new List<ApiVideo>();

            // 2) İsteğe göre kategori filtrele
            if (!string.IsNullOrEmpty(kategori))
            {
                data = data.Where(d => string.Equals(d.Category, kategori, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 3) Maple (liste sayfası Sevval.com'u sabit gösteriyor)
            var currentBase = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
            foreach (var v in data.OrderByDescending(d => d.CreatedDate))
            {
                var isYouTube = (v.VideoUrl ?? string.Empty).Contains("youtube", StringComparison.OrdinalIgnoreCase);
                var ytId = isYouTube ? ExtractYouTubeId(v.VideoUrl) : null;
                string Rehost(string input)
                {
                    if (string.IsNullOrWhiteSpace(input)) return input;
                    if (input.StartsWith("/")) return currentBase + input;
                    if (input.StartsWith("http://") || input.StartsWith("https://"))
                    {
                        try
                        {
                            var u = new Uri(input);
                            return currentBase + u.PathAndQuery;
                        }
                        catch { return input; }
                    }
                    return input;
                }
                videolar.Add(new Sevval.Web.Models.VideolarSayfasi
                {
                    Id = v.Id,
                    VideoAdi = v.Title ?? string.Empty,
                    VideoAciklamasi = v.Description ?? string.Empty,
                    IsYouTube = isYouTube,
                    // Liste ve tekil sayfa YouTube'da ID bekliyor; yerel dosyalarda mevcut hosta rehost et
                    VideoYolu = isYouTube ? (ytId ?? string.Empty) : Rehost(v.VideoUrl ?? string.Empty),
                    KapakFotografiYolu = string.IsNullOrWhiteSpace(v.ThumbnailUrl) ? null : Rehost(v.ThumbnailUrl),
                    Kategori = v.Category ?? string.Empty,
                    GoruntulenmeSayisi = v.ViewCount,
                    YuklenmeTarihi = v.CreatedDate
                });
            }

            // 4) Kategori rozetleri ve sayaçlar
            var kategoriler = (parsed?.Data ?? new List<ApiVideo>())
                .Where(d => !string.IsNullOrWhiteSpace(d.Category))
                .GroupBy(d => d.Category!.Trim())
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();
            ViewBag.Kategoriler = kategoriler;

            var kategoriSayilari = (parsed?.Data ?? new List<ApiVideo>())
                .Where(d => !string.IsNullOrWhiteSpace(d.Category))
                .GroupBy(d => d.Category!)
                .ToDictionary(g => g.Key, g => g.Count());
            ViewBag.KategoriSayilari = kategoriSayilari;
            ViewBag.ToplamVideoSayisi = (parsed?.Data ?? new List<ApiVideo>()).Count;

            // 4.1) Kategori ikon ve renkleri (Categories tablosundan)
            try
            {
                var allCategories = await _context.Categories.ToListAsync();
                ViewBag.KategoriIconlari = allCategories
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .GroupBy(c => c.Name.Trim())
                    .ToDictionary(g => g.Key, g => g.Last().Icon ?? string.Empty);

                ViewBag.KategoriRenkleri = allCategories
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .GroupBy(c => c.Name.Trim())
                    .ToDictionary(g => g.Key, g => string.IsNullOrWhiteSpace(g.Last().Color)
                        ? "bg-white text-blue-700 border-blue-200 hover:bg-blue-50"
                        : g.Last().Color);
            }
            catch { /* Categories tablosu yoksa sessizce geç */ }
        }
        catch
        {
            // API hatasında boş liste ile devam edelim
            // Fallback: yerel DB'den çek (canlı API sorunu olduğunda liste boş kalmasın)
            var fallback = await _context.VideolarSayfasi
                .Include(v => v.YukleyenKullanici)
                .Where(v => string.IsNullOrEmpty(kategori) || v.Kategori == kategori)
                .OrderByDescending(v => v.YuklenmeTarihi)
                .ToListAsync();
            videolar = fallback;
            ViewBag.Kategoriler = await _context.VideolarSayfasi.Select(v => v.Kategori).Distinct().ToListAsync();
            var kategoriSayilari = await _context.VideolarSayfasi
                .GroupBy(v => v.Kategori)
                .Select(g => new { Kategori = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Kategori, x => x.Count);
            ViewBag.KategoriSayilari = kategoriSayilari;
            ViewBag.ToplamVideoSayisi = await _context.VideolarSayfasi.CountAsync();

            // Fallback'te de kategori ikon/renkleri yükle
            try
            {
                var allCategories = await _context.Categories.ToListAsync();
                ViewBag.KategoriIconlari = allCategories
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .GroupBy(c => c.Name)
                    .ToDictionary(g => g.Key, g => g.Last().Icon ?? string.Empty);

                ViewBag.KategoriRenkleri = allCategories
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .GroupBy(c => c.Name)
                    .ToDictionary(g => g.Key, g => string.IsNullOrWhiteSpace(g.Last().Color)
                        ? "bg-white text-blue-700 border-blue-200 hover:bg-blue-50"
                        : g.Last().Color);
            }
            catch { }
        }

        // Yorum sayıları (şimdilik 0 - API sağlanırsa buradan doldurulacak)
        // API yorum/like sayıları yoksa 0; endpoint sağlanırsa burada doldurulur
        ViewBag.YorumSayilari = new Dictionary<int, int>();
        ViewBag.SeciliKategori = kategori;

        // watched ids per authenticated user (yerel DB)
        var userId = _userManager.GetUserId(User);
        if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userId))
        {
            var watchedIds = await _context.VideoWatches
                .Where(w => w.UserId == userId)
                .Select(w => w.VideoId)
                .ToListAsync();
            ViewBag.WatchedIds = new HashSet<int>(watchedIds);
        }
        else
        {
            ViewBag.WatchedIds = new HashSet<int>();
        }

        // 5) Sayfalama uygula (12'li)
        if (page < 1) page = 1;
        if (size < 1) size = 12;

        var totalCount = videolar.Count;
        var totalPages = (int)Math.Ceiling((double)Math.Max(1, totalCount) / size);
        if (page > totalPages) page = totalPages;
        var paged = videolar.Skip((page - 1) * size).Take(size).ToList();

        ViewBag.Page = page;
        ViewBag.Size = size;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;

        return View(paged);
    }

    private class ApiVideosResponse
    {
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public List<ApiVideo> Data { get; set; }
    }

    private static string ExtractYouTubeId(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        try
        {
            // Desteklenen formatlar: watch?v=ID, youtu.be/ID, /embed/ID
            var uri = new Uri(url);
            var host = uri.Host.ToLowerInvariant();
            if (host.Contains("youtu.be"))
            {
                return uri.AbsolutePath.Trim('/');
            }
            if (uri.AbsolutePath.Contains("/embed/", StringComparison.OrdinalIgnoreCase))
            {
                var parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var idx = Array.IndexOf(parts, "embed");
                if (idx >= 0 && idx + 1 < parts.Length) return parts[idx + 1];
            }
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            if (query.TryGetValue("v", out var v)) return v.ToString();
        }
        catch { }
        return null;
    }

    private class ApiVideo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ViewCount { get; set; }
        public int Duration { get; set; }
    }

    // YENİ EKLENEN METOT BAŞLANGICI
    public async Task<IActionResult> UserProfile(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userVideos = await _context.VideolarSayfasi
            .Where(v => v.YukleyenKullaniciId == id)
            .OrderByDescending(v => v.YuklenmeTarihi)
            .ToListAsync();

        var viewModel = new UserProfileViewModel
        {
            User = user,
            Videos = userVideos,
            TotalVideos = userVideos.Count
        };

        return View(viewModel);
    }
    // YENİ EKLENEN METOT BİTİŞİ

    [Authorize]
    public IActionResult Upload()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        // Sadece belirli hesap düzenleyebilir
        if (!string.Equals(User?.Identity?.Name, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var video = await _context.VideolarSayfasi
            .Include(v => v.YukleyenKullanici)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (video == null) return NotFound();

        var model = new Sevval.Web.Models.VideoEditViewModel
        {
            Id = video.Id,
            VideoAdi = video.VideoAdi,
            VideoAciklamasi = video.VideoAciklamasi,
            Kategori = video.Kategori,
            // Sadece ön doldurma amaçlı; gerçek dosya alanları boş kalır
            ExistingCoverPath = video.KapakFotografiYolu,
            ExistingIsYouTube = video.IsYouTube,
            ExistingVideoPath = video.VideoYolu,
            YouTubeLink = video.IsYouTube && !string.IsNullOrWhiteSpace(video.VideoYolu)
                ? ($"https://www.youtube.com/watch?v={video.VideoYolu}")
                : null
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(VideoUploadViewModel model)
    {
        const long OneGb = 1024L * 1024L * 1024L; // 1 GB
        if (model.VideoDosyasi == null && string.IsNullOrEmpty(model.YouTubeLink))
        {
            ModelState.AddModelError("", "Lütfen bir video dosyası yükleyin veya bir YouTube linki girin.");
        }
        // Sunucu tarafı boyut sınırı (1 GB)
        if (model.VideoDosyasi != null && model.VideoDosyasi.Length > OneGb)
        {
            ModelState.AddModelError("VideoDosyasi", "Video boyutu 1 GB'ı aşamaz.");
        }
        if (!ModelState.IsValid)
        {
            // AJAX isteklerinde JSON hata döndür
            if (Request.Headers.ContainsKey("X-Requested-With") &&
                string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                var errors = ModelState.Where(kvp => kvp.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(new { success = false, errors });
            }
            return View(model);
        }
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }
        var yeniVideo = new VideolarSayfasi
        {
            VideoAdi = model.VideoAdi,
            VideoAciklamasi = model.VideoAciklamasi,
            Kategori = model.Kategori,
            YukleyenKullaniciId = user.Id,
            YuklenmeTarihi = DateTime.UtcNow
        };
        string kapaklarKlasoru = Path.Combine(_webHostEnvironment.WebRootPath, "VideoSayfasi/Kapaklar");
        if (!Directory.Exists(kapaklarKlasoru)) Directory.CreateDirectory(kapaklarKlasoru);

        if (model.VideoDosyasi != null)
        {
            yeniVideo.IsYouTube = false;
            string videolarKlasoru = Path.Combine(_webHostEnvironment.WebRootPath, "VideoSayfasi/Videolar");
            if (!Directory.Exists(videolarKlasoru)) Directory.CreateDirectory(videolarKlasoru);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.VideoDosyasi.FileName;
            string videoFilePath = Path.Combine(videolarKlasoru, uniqueFileName);
            await using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
            {
                await model.VideoDosyasi.CopyToAsync(fileStream);
            }
            yeniVideo.VideoYolu = "/VideoSayfasi/Videolar/" + uniqueFileName;

            if (model.KapakFotografi != null)
            {
                string uniqueKapakFileName = Guid.NewGuid().ToString() + "_" + model.KapakFotografi.FileName;
                string kapakFilePath = Path.Combine(kapaklarKlasoru, uniqueKapakFileName);
                await using (var stream = new FileStream(kapakFilePath, FileMode.Create))
                {
                    await model.KapakFotografi.CopyToAsync(stream);
                }
                yeniVideo.KapakFotografiYolu = "/VideoSayfasi/Kapaklar/" + uniqueKapakFileName;
            }
            else
            {
                try
                {
                    yeniVideo.KapakFotografiYolu = await GenerateThumbnailAsync(videoFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Thumbnail generation failed: {ex.Message}");
                    yeniVideo.KapakFotografiYolu = null;
                }
            }
        }
        else if (!string.IsNullOrEmpty(model.YouTubeLink))
        {
            yeniVideo.IsYouTube = true;
            yeniVideo.VideoYolu = GetYouTubeVideoId(model.YouTubeLink);
            if (string.IsNullOrEmpty(yeniVideo.VideoYolu))
            {
                ModelState.AddModelError("YouTubeLink", "Geçersiz YouTube linki.");
                return View(model);
            }

            if (model.KapakFotografi == null)
            {
                yeniVideo.KapakFotografiYolu = $"https://img.youtube.com/vi/{yeniVideo.VideoYolu}/hqdefault.webp";
            }
            else
            {
                string uniqueKapakFileName = Guid.NewGuid().ToString() + "_" + model.KapakFotografi.FileName;
                string kapakFilePath = Path.Combine(kapaklarKlasoru, uniqueKapakFileName);
                await using (var stream = new FileStream(kapakFilePath, FileMode.Create))
                {
                    await model.KapakFotografi.CopyToAsync(stream);
                }
                yeniVideo.KapakFotografiYolu = "/VideoSayfasi/Kapaklar/" + uniqueKapakFileName;
            }
        }
        _context.VideolarSayfasi.Add(yeniVideo);
        await _context.SaveChangesAsync();
        // AJAX ise JSON redirect dön, değilse klasik redirect
        if (Request.Headers.ContainsKey("X-Requested-With") &&
            string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new { success = true, redirectUrl = Url.Action("Index") });
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VideoEditViewModel model)
    {
        if (!string.Equals(User?.Identity?.Name, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }
        if (!model.Id.HasValue)
        {
            return BadRequest();
        }

        var video = await _context.VideolarSayfasi.FirstOrDefaultAsync(v => v.Id == model.Id.Value);
        if (video == null) return NotFound();

        const long OneGb = 1024L * 1024L * 1024L;
        if (model.VideoDosyasi != null && model.VideoDosyasi.Length > OneGb)
        {
            ModelState.AddModelError("VideoDosyasi", "Video boyutu 1 GB'ı aşamaz.");
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        video.VideoAdi = model.VideoAdi;
        video.VideoAciklamasi = model.VideoAciklamasi;
        video.Kategori = model.Kategori;

        string kapaklarKlasoru = Path.Combine(_webHostEnvironment.WebRootPath, "VideoSayfasi/Kapaklar");
        if (!Directory.Exists(kapaklarKlasoru)) Directory.CreateDirectory(kapaklarKlasoru);

        if (model.VideoDosyasi != null)
        {
            // Yeni dosya yüklendiyse, yerel video olarak güncelle
            video.IsYouTube = false;
            string videolarKlasoru = Path.Combine(_webHostEnvironment.WebRootPath, "VideoSayfasi/Videolar");
            if (!Directory.Exists(videolarKlasoru)) Directory.CreateDirectory(videolarKlasoru);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.VideoDosyasi.FileName;
            string videoFilePath = Path.Combine(videolarKlasoru, uniqueFileName);
            await using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
            {
                await model.VideoDosyasi.CopyToAsync(fileStream);
            }
            video.VideoYolu = "/VideoSayfasi/Videolar/" + uniqueFileName;
        }
        else if (!string.IsNullOrEmpty(model.YouTubeLink))
        {
            video.IsYouTube = true;
            var ytId = GetYouTubeVideoId(model.YouTubeLink);
            if (string.IsNullOrEmpty(ytId))
            {
                ModelState.AddModelError("YouTubeLink", "Geçersiz YouTube linki.");
                return View(model);
            }
            video.VideoYolu = ytId;
            if (string.IsNullOrWhiteSpace(video.KapakFotografiYolu))
            {
                video.KapakFotografiYolu = $"https://img.youtube.com/vi/{ytId}/hqdefault.webp";
            }
        }

        if (model.KapakFotografi != null)
        {
            string uniqueKapakFileName = Guid.NewGuid().ToString() + "_" + model.KapakFotografi.FileName;
            string kapakFilePath = Path.Combine(kapaklarKlasoru, uniqueKapakFileName);
            await using (var stream = new FileStream(kapakFilePath, FileMode.Create))
            {
                await model.KapakFotografi.CopyToAsync(stream);
            }
            video.KapakFotografiYolu = "/VideoSayfasi/Kapaklar/" + uniqueKapakFileName;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Play), new { id = video.Id });
    }

    private async Task<string> GenerateThumbnailAsync(string videoPath)
    {
        string thumbnailsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "VideoSayfasi/Kapaklar");
        string thumbnailFileName = $"{Path.GetFileNameWithoutExtension(videoPath)}.webp";
        string thumbnailFilePath = Path.Combine(thumbnailsFolder, thumbnailFileName);
        string thumbnailWebPath = $"/VideoSayfasi/Kapaklar/{thumbnailFileName}";
        string arguments = $"-i \"{videoPath}\" -ss 00:00:05 -vframes 1 -q:v 2 \"{thumbnailFilePath}\"";
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using (var process = new Process { StartInfo = processStartInfo })
        {
            process.Start();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                throw new Exception($"FFmpeg failed with exit code {process.ExitCode}: {error}");
            }
        }
        return thumbnailWebPath;
    }

    private string GetYouTubeVideoId(string url)
    {
        var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:(?:youtube\.com\/watch\?[^?]*v=|youtu\.be\/)([\w\-]+))(?:[^\s?&]*)", RegexOptions.IgnoreCase);
        var match = regex.Match(url);
        return match.Success ? match.Groups[1].Value : null;
    }
    public async Task<IActionResult> Play(int id)
    {
        var video = await _context.VideolarSayfasi
            .Include(v => v.YukleyenKullanici)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (video == null)
        {
            return NotFound();
        }

        video.GoruntulenmeSayisi++;
        _context.Update(video);
        await _context.SaveChangesAsync();

        var oneriVideolar = await _context.VideolarSayfasi
            .Where(v => v.Kategori == video.Kategori && v.Id != id)
            .Include(v => v.YukleyenKullanici)
            .OrderByDescending(v => v.YuklenmeTarihi)
            .Take(10)
            .ToListAsync();

        var yorumlar = await _context.VideoYorumlari
            .Where(c => c.VideoId == id && c.ParentYorumId == null) // Sadece ana yorumları çek
            .Include(c => c.Kullanici)
            .Include(c => c.Yanitlar) // Yanıtları dahil et
                .ThenInclude(r => r.Kullanici) // Yanıtları yapan kullanıcıları dahil et
            .OrderByDescending(c => c.YorumTarihi)
            .ToListAsync();

        var currentUserVote = User.Identity.IsAuthenticated ?
            await _context.VideoLikes.FirstOrDefaultAsync(l => l.VideoId == id && l.UserId == _userManager.GetUserId(User)) :
            null;

        var viewModel = new VideoPlayViewModel
        {
            Video = video,
            OneriVideolar = oneriVideolar,
            Yorumlar = yorumlar,
            CurrentUserVote = currentUserVote?.IsLike
        };

        // Persist watched in DB for authenticated users
        if (User.Identity.IsAuthenticated)
        {
            var uid = _userManager.GetUserId(User);
            var exists = await _context.VideoWatches.AnyAsync(w => w.VideoId == id && w.UserId == uid);
            if (!exists)
            {
                _context.VideoWatches.Add(new VideoWatch { VideoId = id, UserId = uid, WatchedAtUtc = DateTime.UtcNow });
                await _context.SaveChangesAsync();
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var comment = await _context.VideoYorumlari
            .Include(c => c.Yanitlar)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return Json(new { success = false, message = "Yorum bulunamadı." });
        }

        // Önce yanıtları sil, sonra ana yorumu sil
        if (comment.Yanitlar != null && comment.Yanitlar.Count > 0)
        {
            _context.VideoYorumlari.RemoveRange(comment.Yanitlar);
        }
        _context.VideoYorumlari.Remove(comment);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var video = await _context.VideolarSayfasi.FirstOrDefaultAsync(v => v.Id == id);
        if (video == null)
        {
            return NotFound();
        }

        // Fiziksel dosyaları sil (YouTube olmayan videolar ve yerel kapak)
        try
        {
            if (!video.IsYouTube && !string.IsNullOrWhiteSpace(video.VideoYolu) && video.VideoYolu.StartsWith("/"))
            {
                var fullVideoPath = Path.Combine(_webHostEnvironment.WebRootPath, video.VideoYolu.TrimStart('/'));
                if (System.IO.File.Exists(fullVideoPath))
                {
                    System.IO.File.Delete(fullVideoPath);
                }
            }
            if (!string.IsNullOrWhiteSpace(video.KapakFotografiYolu) && video.KapakFotografiYolu.StartsWith("/VideoSayfasi/"))
            {
                var fullCoverPath = Path.Combine(_webHostEnvironment.WebRootPath, video.KapakFotografiYolu.TrimStart('/'));
                if (System.IO.File.Exists(fullCoverPath))
                {
                    System.IO.File.Delete(fullCoverPath);
                }
            }
        }
        catch
        {
            // Dosya silme hatalarını yutuyoruz; veritabanı silme engellenmesin
        }

        _context.VideolarSayfasi.Remove(video);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleVote(int videoId, bool isLike)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var video = await _context.VideolarSayfasi.FindAsync(videoId);
        if (video == null) return NotFound();

        var existingLike = await _context.VideoLikes.FirstOrDefaultAsync(l => l.VideoId == videoId && l.UserId == user.Id);
        bool? voteStatus = null;

        if (existingLike != null)
        {
            if (existingLike.IsLike == isLike)
            {
                // Kullanıcı aynı oyu geri alıyor
                _context.VideoLikes.Remove(existingLike);
                if (isLike) video.BegeniSayisi--; else video.DislikeSayisi--;
                voteStatus = null;
            }
            else
            {
                // Kullanıcı oyunu değiştiriyor
                if (isLike) { video.BegeniSayisi++; video.DislikeSayisi--; }
                else { video.BegeniSayisi--; video.DislikeSayisi++; }
                existingLike.IsLike = isLike;
                voteStatus = isLike;
            }
        }
        else
        {
            // Yeni oy
            var newLike = new VideoLike { VideoId = videoId, UserId = user.Id, IsLike = isLike };
            _context.VideoLikes.Add(newLike);
            if (isLike) video.BegeniSayisi++; else video.DislikeSayisi++;
            voteStatus = isLike;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, likeCount = video.BegeniSayisi, dislikeCount = video.DislikeSayisi, voteStatus });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> YorumEkle(int videoId, string yorumMetni, int? parentYorumId)
    {
        if (string.IsNullOrWhiteSpace(yorumMetni) || yorumMetni.Length > 1000)
        {
            return Json(new { success = false, message = "Yorum metni geçersiz." });
        }

        var user = await _userManager.GetUserAsync(User);
        var video = await _context.VideolarSayfasi.FindAsync(videoId);
        if (user == null || video == null)
        {
            return Json(new { success = false, message = "Geçersiz istek." });
        }

        if (parentYorumId.HasValue)
        {
            var parentYorum = await _context.VideoYorumlari.FindAsync(parentYorumId.Value);
            if (parentYorum == null || parentYorum.VideoId != videoId)
            {
                return Json(new { success = false, message = "Geçersiz ana yorum." });
            }
        }

        var yeniYorum = new VideoYorum
        {
            VideoId = videoId,
            YorumMetni = yorumMetni,
            UserId = user.Id,
            YorumTarihi = DateTime.UtcNow,
            ParentYorumId = parentYorumId
        };

        _context.VideoYorumlari.Add(yeniYorum);
        await _context.SaveChangesAsync();

        var isVideoOwner = video.YukleyenKullaniciId == user.Id;

        return Json(new
        {
            success = true,
            yorumId = yeniYorum.Id,
            parentYorumId = yeniYorum.ParentYorumId,
            yorumMetni = yeniYorum.YorumMetni,
            kullaniciAdi = $"{user.FirstName} {user.LastName}",
            profilResmi = user.ProfilePicturePath ?? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(user.FirstName)}+{Uri.EscapeDataString(user.LastName)}&background=random&color=fff",
            yorumTarihi = yeniYorum.YorumTarihi.ToString("d MMM yyyy"),
            isVideoOwner
        });
    }
}
