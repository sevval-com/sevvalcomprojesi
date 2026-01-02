using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.Services;
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
    private readonly IYouTubeUrlParser _youTubeUrlParser;
    private readonly IVideoApprovalService _videoApprovalService;

    public VideolarSayfasiController(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager, 
        IWebHostEnvironment webHostEnvironment,
        IYouTubeUrlParser youTubeUrlParser,
        IVideoApprovalService videoApprovalService)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _youTubeUrlParser = youTubeUrlParser;
        _videoApprovalService = videoApprovalService;
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
            http.Timeout = TimeSpan.FromSeconds(5); // 5 saniye timeout - yavaşsa fallback'e düş
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
                var videoItem = new Sevval.Web.Models.VideolarSayfasi
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
                };
                // Yükleyen kullanıcı bilgisini ekle
                if (v.YukleyenKullanici != null)
                {
                    videoItem.YukleyenKullanici = new Sevval.Domain.Entities.ApplicationUser
                    {
                        FirstName = v.YukleyenKullanici.FirstName ?? "Sevval",
                        LastName = v.YukleyenKullanici.LastName ?? "",
                        ProfilePicturePath = v.YukleyenKullanici.ProfilePicturePath
                    };
                }
                videolar.Add(videoItem);
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
            ViewBag.ToplamIzlenme = (parsed?.Data ?? new List<ApiVideo>()).Sum(v => v.ViewCount);

            // 4.1) Kategori ikon ve renkleri (Categories tablosundan)
            try
            {
                var allCategories = await _context.Categories.AsNoTracking().ToListAsync();
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
            // API hatasında veya timeout'ta yerel DB'den çek
            var fallback = await _context.VideolarSayfasi
                .AsNoTracking()
                .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                .Where(v => string.IsNullOrEmpty(kategori) || v.Kategori == kategori)
                .OrderByDescending(v => v.YuklenmeTarihi)
                .ToListAsync();
            videolar = fallback;
            
            ViewBag.Kategoriler = await _context.VideolarSayfasi
                .AsNoTracking()
                .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                .Select(v => v.Kategori).Distinct().ToListAsync();
            
            var kategoriSayilari = await _context.VideolarSayfasi
                .AsNoTracking()
                .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                .GroupBy(v => v.Kategori)
                .Select(g => new { Kategori = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Kategori, x => x.Count);
            ViewBag.KategoriSayilari = kategoriSayilari;
            
            ViewBag.ToplamVideoSayisi = await _context.VideolarSayfasi
                .AsNoTracking()
                .CountAsync(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved);
            
            ViewBag.ToplamIzlenme = await _context.VideolarSayfasi
                .AsNoTracking()
                .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                .SumAsync(v => v.GoruntulenmeSayisi);

            // Fallback'te de kategori ikon/renkleri yükle
            try
            {
                var allCategories = await _context.Categories.AsNoTracking().ToListAsync();
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
                .AsNoTracking()
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

        // Kullanıcı bilgilerini ViewBag ile geçir
        var videoIds = paged.Select(v => v.Id).ToList();
        var userInfoDict = new Dictionary<int, (string Name, string Photo)>();
        
        try
        {
            var videoUsers = await _context.VideolarSayfasi
                .AsNoTracking()
                .Include(v => v.YukleyenKullanici)
                .Where(v => videoIds.Contains(v.Id))
                .Select(v => new { v.Id, v.YukleyenKullanici })
                .ToListAsync();
            
            foreach (var vu in videoUsers)
            {
                if (vu.YukleyenKullanici != null)
                {
                    var name = $"{vu.YukleyenKullanici.FirstName} {vu.YukleyenKullanici.LastName}".Trim();
                    var photo = vu.YukleyenKullanici.ProfilePicturePath ?? "";
                    userInfoDict[vu.Id] = (string.IsNullOrWhiteSpace(name) ? "Sevval.com" : name, photo);
                }
            }
        }
        catch { }
        
        ViewBag.VideoUserInfo = userInfoDict;

        return View(paged);
    }

    private class ApiVideosResponse
    {
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public List<ApiVideo> Data { get; set; }
    }

    private string ExtractYouTubeId(string url)
    {
        return _youTubeUrlParser.ExtractVideoId(url);
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
        public ApiVideoUser YukleyenKullanici { get; set; }
    }

    private class ApiVideoUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicturePath { get; set; }
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

        // Mevcut kullanıcı kendi profilini mi görüntülüyor?
        var currentUser = await _userManager.GetUserAsync(User);
        var isOwnProfile = currentUser != null && currentUser.Id == id;
        var isSuperAdmin = currentUser != null && _videoApprovalService.IsSuperAdmin(currentUser.Email);

        IQueryable<VideolarSayfasi> query = _context.VideolarSayfasi
            .Where(v => v.YukleyenKullaniciId == id);

        // Kendi profili veya Super Admin ise tüm videoları göster, değilse sadece onaylıları
        if (!isOwnProfile && !isSuperAdmin)
        {
            query = query.Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved);
        }

        var userVideos = await query
            .OrderByDescending(v => v.YuklenmeTarihi)
            .ToListAsync();

        var viewModel = new UserProfileViewModel
        {
            User = user,
            Videos = userVideos,
            TotalVideos = userVideos.Count
        };

        // Kendi profili ise video durumlarını göster
        ViewBag.IsOwnProfile = isOwnProfile;

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

        // Super Admin kontrolü - Super Admin'in videoları otomatik onaylanır
        var isSuperAdmin = _videoApprovalService.IsSuperAdmin(user.Email);
        var approvalStatus = isSuperAdmin 
            ? Sevval.Domain.Enums.VideoApprovalStatus.Approved 
            : Sevval.Domain.Enums.VideoApprovalStatus.Pending;

        var yeniVideo = new VideolarSayfasi
        {
            VideoAdi = model.VideoAdi,
            VideoAciklamasi = model.VideoAciklamasi,
            Kategori = model.Kategori ?? "Genel",
            YukleyenKullaniciId = user.Id,
            YuklenmeTarihi = DateTime.UtcNow,
            ApprovalStatus = approvalStatus,
            ApprovalDate = isSuperAdmin ? DateTime.UtcNow : null,
            ApprovedByUserId = isSuperAdmin ? user.Id : null
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
                ModelState.AddModelError("YouTubeLink", "Geçersiz YouTube linki. Desteklenen formatlar: youtube.com/watch?v=ID, youtu.be/ID, youtube.com/embed/ID");
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
        
        // Onay durumuna göre mesaj belirle
        var successMessage = isSuperAdmin 
            ? "Video başarıyla yüklendi ve yayına alındı." 
            : "Video başarıyla yüklendi. Videonuz onaylandığı zaman yayına girecektir.";
        
        // AJAX ise JSON redirect dön, değilse klasik redirect
        if (Request.Headers.ContainsKey("X-Requested-With") &&
            string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new { success = true, message = successMessage, redirectUrl = Url.Action("Index") });
        }
        TempData["SuccessMessage"] = successMessage;
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
                ModelState.AddModelError("YouTubeLink", "Geçersiz YouTube linki. Desteklenen formatlar: youtube.com/watch?v=ID, youtu.be/ID, youtube.com/embed/ID");
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
        return _youTubeUrlParser.ExtractVideoId(url);
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

        // Görüntüleme sayısını artır (24 saat içinde aynı kişi 1 kez - cookie kontrolü)
        var cookieKey = $"video_view_{id}";
        if (!Request.Cookies.ContainsKey(cookieKey))
        {
            video.GoruntulenmeSayisi++;
            await _context.SaveChangesAsync();
            
            // 24 saatlik cookie set et
            Response.Cookies.Append(cookieKey, "1", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(24),
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
        }

        var oneriVideolar = await _context.VideolarSayfasi
            .Where(v => v.Kategori == video.Kategori && v.Id != id)
            .Include(v => v.YukleyenKullanici)
            .OrderByDescending(v => v.YuklenmeTarihi)
            .Take(10)
            .ToListAsync();

        var yorumlar = await _context.VideoYorumlari
            .Where(c => c.VideoId == id && c.ParentYorumId == null)
            .Include(c => c.Kullanici)
            .Include(c => c.Yanitlar)
                .ThenInclude(r => r.Kullanici)
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

        // Persist watched in DB for authenticated users & get watched IDs for suggested videos
        if (User.Identity.IsAuthenticated)
        {
            var uid = _userManager.GetUserId(User);
            var exists = await _context.VideoWatches.AnyAsync(w => w.VideoId == id && w.UserId == uid);
            if (!exists)
            {
                _context.VideoWatches.Add(new VideoWatch { VideoId = id, UserId = uid, WatchedAtUtc = DateTime.UtcNow });
                await _context.SaveChangesAsync();
            }
            
            // İzlenen video ID'lerini ViewBag'e ekle (önerilen videolarda "İzlendi" etiketi için)
            var watchedIds = await _context.VideoWatches
                .AsNoTracking()
                .Where(w => w.UserId == uid)
                .Select(w => w.VideoId)
                .ToListAsync();
            ViewBag.WatchedIds = new HashSet<int>(watchedIds);
        }
        else
        {
            ViewBag.WatchedIds = new HashSet<int>();
        }

        return View(viewModel);
    }

    /// <summary>
    /// Video görüntülenme sayısını artırır (sayfa açıldığında çağrılır)
    /// 24 saat içinde aynı kişi sadece 1 kez görüntüleme artırabilir (cookie ile kontrol)
    /// </summary>
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Route("VideolarSayfasi/IncrementViewCount")]
    public async Task<IActionResult> IncrementViewCount(int id)
    {
        try
        {
            // Cookie key: video_view_{id}
            var cookieKey = $"video_view_{id}";
            
            // Cookie var mı kontrol et (24 saat içinde izlemiş mi?)
            if (Request.Cookies.ContainsKey(cookieKey))
            {
                // Zaten izlemiş, artırma - 200 OK döndür (UI güncellemesi yapılmasın)
                return Ok(new { incremented = false });
            }
            
            var video = await _context.VideolarSayfasi.FindAsync(id);
            if (video != null)
            {
                video.GoruntulenmeSayisi++;
                await _context.SaveChangesAsync();
                
                // 24 saatlik cookie set et
                Response.Cookies.Append(cookieKey, "1", new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(24),
                    HttpOnly = true,
                    Secure = false, // localhost için false
                    SameSite = SameSiteMode.Lax
                });
                
                // Başarılı - 204 No Content döndür (UI güncellensin)
                return NoContent();
            }
        }
        catch { }

        return Ok(new { incremented = false });
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

    // ==================== VIDEO ONAY SİSTEMİ ====================

    /// <summary>
    /// Bekleyen videoları listeler (Sadece Super Admin)
    /// </summary>
    [Authorize]
    public async Task<IActionResult> PendingVideos()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !_videoApprovalService.IsSuperAdmin(user.Email))
        {
            return Forbid();
        }

        var pendingVideos = await _videoApprovalService.GetPendingVideosAsync();
        var viewModels = pendingVideos.Select(v => new PendingVideoViewModel
        {
            Id = v.Id,
            VideoAdi = v.VideoAdi,
            KapakFotografiYolu = v.KapakFotografiYolu,
            VideoYolu = v.VideoYolu,
            IsYouTube = v.IsYouTube,
            UploaderName = v.YukleyenKullanici != null 
                ? $"{v.YukleyenKullanici.FirstName} {v.YukleyenKullanici.LastName}" 
                : "Bilinmiyor",
            UploaderEmail = v.YukleyenKullanici?.Email ?? "Bilinmiyor",
            YuklenmeTarihi = v.YuklenmeTarihi,
            Kategori = v.Kategori
        }).ToList();

        ViewBag.PendingCount = viewModels.Count;
        return View(viewModels);
    }

    /// <summary>
    /// Videoyu onaylar (Sadece Super Admin)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveVideo(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !_videoApprovalService.IsSuperAdmin(user.Email))
        {
            return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
        }

        var result = await _videoApprovalService.ApproveVideoAsync(id, user.Id);
        if (result)
        {
            return Json(new { success = true, message = "Video başarıyla onaylandı." });
        }

        return Json(new { success = false, message = "Video onaylanamadı. Video bulunamadı veya zaten işlenmiş." });
    }

    /// <summary>
    /// Videoyu reddeder (Sadece Super Admin)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectVideo(int id, string? reason)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !_videoApprovalService.IsSuperAdmin(user.Email))
        {
            return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
        }

        var result = await _videoApprovalService.RejectVideoAsync(id, user.Id, reason);
        if (result)
        {
            return Json(new { success = true, message = "Video reddedildi." });
        }

        return Json(new { success = false, message = "Video reddedilemedi. Video bulunamadı veya zaten işlenmiş." });
    }

    /// <summary>
    /// Bekleyen video sayısını JSON olarak döner (Badge için)
    /// </summary>
    [Authorize]
    public async Task<IActionResult> GetPendingVideoCount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !_videoApprovalService.IsSuperAdmin(user.Email))
        {
            return Json(new { count = 0 });
        }

        var count = await _videoApprovalService.GetPendingVideoCountAsync();
        return Json(new { count });
    }

    // ==================== VİDEOLARIM (KULLANICI VİDEOLARI) ====================

    /// <summary>
    /// Kullanıcının kendi videolarını listeler
    /// </summary>
    [Authorize]
    public async Task<IActionResult> Videolarim()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        // Kullanıcının tüm videolarını getir (tüm durumlar)
        var userVideos = await _videoApprovalService.GetUserVideosByStatusAsync(user.Id, null);

        return View(userVideos);
    }

    /// <summary>
    /// Kullanıcının kendi videosunu silmesi (Sadece kendi videoları)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserVideo(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
        }

        var video = await _context.VideolarSayfasi.FirstOrDefaultAsync(v => v.Id == id);
        if (video == null)
        {
            return Json(new { success = false, message = "Video bulunamadı." });
        }

        // Sadece kendi videosunu silebilir
        if (video.YukleyenKullaniciId != user.Id)
        {
            return Json(new { success = false, message = "Bu videoyu silme yetkiniz yok." });
        }

        // Fiziksel dosyaları sil
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
            // Dosya silme hatalarını yutuyoruz
        }

        _context.VideolarSayfasi.Remove(video);
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Video başarıyla silindi." });
    }

    // ==================== ÖNERİLEN VİDEOLAR API ====================

    /// <summary>
    /// Önerilen videoları JSON olarak döner (Fallback endpoint)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSuggestedVideos(int currentVideoId, string? category = null, int limit = 6)
    {
        try
        {
            var query = _context.VideolarSayfasi
                .Include(v => v.YukleyenKullanici)
                .Where(v => v.Id != currentVideoId && v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                .AsQueryable();

            // Önce aynı kategorideki videoları, sonra en son yüklenenleri getir
            var normalizedCategory = !string.IsNullOrEmpty(category) ? category.Trim().TrimEnd('.') : "";
            
            var suggestedVideos = await query
                .OrderByDescending(v => !string.IsNullOrEmpty(normalizedCategory) && 
                    v.Kategori != null && v.Kategori.Trim().TrimEnd('.') == normalizedCategory ? 1 : 0)
                .ThenByDescending(v => v.YuklenmeTarihi)
                .Take(limit)
                .Select(v => new
                {
                    id = v.Id,
                    videoAdi = v.VideoAdi,
                    videoAciklamasi = v.VideoAciklamasi,
                    videoYolu = v.VideoYolu,
                    kapakFotografiYolu = v.KapakFotografiYolu,
                    kategori = v.Kategori,
                    goruntulenmeSayisi = v.GoruntulenmeSayisi,
                    begeniSayisi = v.BegeniSayisi,
                    dislikeSayisi = v.DislikeSayisi,
                    yuklenmeTarihi = v.YuklenmeTarihi,
                    isYouTube = v.IsYouTube,
                    yukleyenKullanici = new
                    {
                        firstName = v.YukleyenKullanici != null ? v.YukleyenKullanici.FirstName : "Anonim",
                        lastName = v.YukleyenKullanici != null ? v.YukleyenKullanici.LastName : "",
                        profilePicturePath = v.YukleyenKullanici != null && v.YukleyenKullanici.ProfilePicturePath != null 
                            ? v.YukleyenKullanici.ProfilePicturePath 
                            : null
                    }
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                videos = suggestedVideos
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message,
                videos = new List<object>()
            });
        }
    }

    /// <summary>
    /// Tüm videolarda arama yapar (tüm kategoriler, tüm sayfalar)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search(string q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(new { success = true, videos = new List<object>() });
            }

            var searchTerm = NormalizeTurkish(q.Trim());

            // API'den tüm videoları çek
            var isLocal = string.Equals(Request?.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(Request?.Host.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
            var apiBase = isLocal
                ? "https://localhost:44347/api/v1"
                : "http://94.73.131.202:8090/api/v1";
            var apiUrl = $"{apiBase}/videos";

            List<object> results = new();

            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(5);
                using var resp = await http.GetAsync(apiUrl);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parsed = System.Text.Json.JsonSerializer.Deserialize<ApiVideosResponse>(json, options);
                var data = parsed?.Data ?? new List<ApiVideo>();

                // Arama filtresi uygula - Türkçe karakterleri normalize ederek
                var filtered = data.Where(v => 
                    (!string.IsNullOrEmpty(v.Title) && NormalizeTurkish(v.Title).Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(v.Description) && NormalizeTurkish(v.Description).Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(v.Category) && NormalizeTurkish(v.Category).Contains(searchTerm))
                ).ToList();

                var currentBase = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
                
                results = filtered.OrderByDescending(v => v.CreatedDate).Select(v => {
                    var isYouTube = (v.VideoUrl ?? string.Empty).Contains("youtube", StringComparison.OrdinalIgnoreCase);
                    var ytId = isYouTube ? ExtractYouTubeId(v.VideoUrl) : null;
                    
                    string Rehost(string input)
                    {
                        if (string.IsNullOrWhiteSpace(input)) return input;
                        if (input.StartsWith("/")) return currentBase + input;
                        if (input.StartsWith("http://") || input.StartsWith("https://"))
                        {
                            try { var u = new Uri(input); return currentBase + u.PathAndQuery; }
                            catch { return input; }
                        }
                        return input;
                    }

                    return new {
                        id = v.Id,
                        videoAdi = v.Title ?? string.Empty,
                        videoAciklamasi = v.Description ?? string.Empty,
                        videoYolu = isYouTube ? (ytId ?? string.Empty) : Rehost(v.VideoUrl ?? string.Empty),
                        kapakFotografiYolu = string.IsNullOrWhiteSpace(v.ThumbnailUrl) ? null : Rehost(v.ThumbnailUrl),
                        kategori = v.Category ?? string.Empty,
                        goruntulenmeSayisi = v.ViewCount,
                        yuklenmeTarihi = v.CreatedDate.ToString("d MMMM yyyy"),
                        isYouTube = isYouTube
                    };
                }).Cast<object>().ToList();
            }
            catch
            {
                // API hatasında yerel DB'den ara
                var allVideos = await _context.VideolarSayfasi
                    .AsNoTracking()
                    .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                    .OrderByDescending(v => v.YuklenmeTarihi)
                    .ToListAsync();

                // Türkçe normalize ederek filtrele
                var localFiltered = allVideos.Where(v =>
                    (!string.IsNullOrEmpty(v.VideoAdi) && NormalizeTurkish(v.VideoAdi).Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(v.VideoAciklamasi) && NormalizeTurkish(v.VideoAciklamasi).Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(v.Kategori) && NormalizeTurkish(v.Kategori).Contains(searchTerm))
                ).ToList();

                results = localFiltered.Select(v => new {
                    id = v.Id,
                    videoAdi = v.VideoAdi,
                    videoAciklamasi = v.VideoAciklamasi,
                    videoYolu = v.VideoYolu,
                    kapakFotografiYolu = v.KapakFotografiYolu,
                    kategori = v.Kategori,
                    goruntulenmeSayisi = v.GoruntulenmeSayisi,
                    yuklenmeTarihi = v.YuklenmeTarihi.ToString("d MMMM yyyy"),
                    isYouTube = v.IsYouTube
                }).Cast<object>().ToList();
            }

            return Json(new { success = true, videos = results });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message, videos = new List<object>() });
        }
    }

    /// <summary>
    /// Türkçe karakterleri normalize eder (büyük/küçük harf duyarsız arama için)
    /// </summary>
    private static string NormalizeTurkish(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        
        return text
            .ToUpperInvariant()
            .Replace("İ", "I")
            .Replace("Ş", "S")
            .Replace("Ğ", "G")
            .Replace("Ü", "U")
            .Replace("Ö", "O")
            .Replace("Ç", "C")
            .Replace("ı", "I")
            .Replace("ş", "S")
            .Replace("ğ", "G")
            .Replace("ü", "U")
            .Replace("ö", "O")
            .Replace("ç", "C")
            .Replace("i", "I");
    }

    /// <summary>
    /// Tüm videoları client-side cache için döner (anlık arama için)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllVideosForSearch()
    {
        try
        {
            // API'den tüm videoları çek
            var isLocal = string.Equals(Request?.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(Request?.Host.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
            var apiBase = isLocal
                ? "https://localhost:44347/api/v1"
                : "http://94.73.131.202:8090/api/v1";
            var apiUrl = $"{apiBase}/videos";

            List<object> results = new();

            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(5);
                using var resp = await http.GetAsync(apiUrl);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parsed = System.Text.Json.JsonSerializer.Deserialize<ApiVideosResponse>(json, options);
                var data = parsed?.Data ?? new List<ApiVideo>();

                var currentBase = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
                
                results = data.OrderByDescending(v => v.CreatedDate).Select(v => {
                    var isYouTube = (v.VideoUrl ?? string.Empty).Contains("youtube", StringComparison.OrdinalIgnoreCase);
                    var ytId = isYouTube ? ExtractYouTubeId(v.VideoUrl) : null;
                    
                    string Rehost(string input)
                    {
                        if (string.IsNullOrWhiteSpace(input)) return input;
                        if (input.StartsWith("/")) return currentBase + input;
                        if (input.StartsWith("http://") || input.StartsWith("https://"))
                        {
                            try { var u = new Uri(input); return currentBase + u.PathAndQuery; }
                            catch { return input; }
                        }
                        return input;
                    }

                    return new {
                        id = v.Id,
                        videoAdi = v.Title ?? string.Empty,
                        videoAciklamasi = v.Description ?? string.Empty,
                        videoYolu = isYouTube ? (ytId ?? string.Empty) : Rehost(v.VideoUrl ?? string.Empty),
                        kapakFotografiYolu = string.IsNullOrWhiteSpace(v.ThumbnailUrl) ? null : Rehost(v.ThumbnailUrl),
                        kategori = v.Category ?? string.Empty,
                        goruntulenmeSayisi = v.ViewCount,
                        yuklenmeTarihi = v.CreatedDate.ToString("d MMMM yyyy"),
                        isYouTube = isYouTube,
                        yukleyenKullanici = v.YukleyenKullanici != null ? new {
                            firstName = v.YukleyenKullanici.FirstName ?? "Anonim",
                            lastName = v.YukleyenKullanici.LastName ?? "",
                            profilePicturePath = v.YukleyenKullanici.ProfilePicturePath
                        } : new { firstName = "Anonim", lastName = "", profilePicturePath = (string?)null }
                    };
                }).Cast<object>().ToList();
            }
            catch
            {
                // API hatasında yerel DB'den al
                var allVideos = await _context.VideolarSayfasi
                    .AsNoTracking()
                    .Include(v => v.YukleyenKullanici)
                    .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Approved)
                    .OrderByDescending(v => v.YuklenmeTarihi)
                    .ToListAsync();

                results = allVideos.Select(v => new {
                    id = v.Id,
                    videoAdi = v.VideoAdi,
                    videoAciklamasi = v.VideoAciklamasi,
                    videoYolu = v.VideoYolu,
                    kapakFotografiYolu = v.KapakFotografiYolu,
                    kategori = v.Kategori,
                    goruntulenmeSayisi = v.GoruntulenmeSayisi,
                    yuklenmeTarihi = v.YuklenmeTarihi.ToString("d MMMM yyyy"),
                    isYouTube = v.IsYouTube,
                    yukleyenKullanici = new {
                        firstName = v.YukleyenKullanici != null ? v.YukleyenKullanici.FirstName : "Anonim",
                        lastName = v.YukleyenKullanici != null ? v.YukleyenKullanici.LastName : "",
                        profilePicturePath = v.YukleyenKullanici?.ProfilePicturePath
                    }
                }).Cast<object>().ToList();
            }

            return Json(new { success = true, videos = results });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message, videos = new List<object>() });
        }
    }
}
