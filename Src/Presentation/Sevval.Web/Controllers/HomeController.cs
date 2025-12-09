using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using Sevval.Persistence.Context.sevvalemlak.Models;
using sevvalemlak.csproj.ClientServices.AnnouncementService;
using sevvalemlak.csproj.ClientServices.CompanyService;
using sevvalemlak.csproj.ClientServices.VisitoryServices;
using sevvalemlak.csproj.Dto.Company;
using sevvalemlak.Dto;
using System.Net;
using System.Net.Mail;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context; // DbContext
    private readonly UserManager<ApplicationUser> _userManager; // UserManager
    private readonly IAnnouncementClientService _announcementClientService;
    private readonly ICompanyClientService _companyClientService;
    private readonly IVisitoryClientService _visitoryClientService;

    // Admin email sabit
    private const string AdminEmail = "sftumen41@gmail.com";

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        IAnnouncementClientService announcementClientService
, ICompanyClientService companyClientService, IVisitoryClientService visitoryClientService)
    {
        _context = context;
        _userManager = userManager;
        _announcementClientService = announcementClientService;
        _companyClientService = companyClientService;
        _visitoryClientService = visitoryClientService;
    }

    // İlan sayısını asenkron döndüren endpoint
    [HttpGet("api/ilan-sayisi")]
    public async Task<IActionResult> GetIlanSayisi()
    {
        var response = await _announcementClientService.GetActiveVisitorCount(new GetAnnouncementCountQueryRequest()
        {
            Status = "active"
        }, CancellationToken.None);

        return Ok(response);
    }

    [HttpGet("api/provinces")]
    public async Task<IActionResult> GetProvinces()
    {
        try
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "TR-İl-İlçe-Mahalle_(City-County-Neighborhood).json");
            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var data = System.Text.Json.JsonSerializer.Deserialize<List<CityData>>(jsonContent);

            var provinces = data?.Select(x => x.Il).Distinct().OrderBy(x => x).ToList();
            return Ok(provinces);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "İl listesi yüklenirken hata oluştu", message = ex.Message });
        }
    }

    [HttpGet("api/districts/{province}")]
    public async Task<IActionResult> GetDistricts(string province)
    {
        try
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "TR-İl-İlçe-Mahalle_(City-County-Neighborhood).json");
            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var data = System.Text.Json.JsonSerializer.Deserialize<List<CityData>>(jsonContent);

            var cityData = data?.FirstOrDefault(x => x.Il == province);
            var districts = cityData?.Ilce?.Select(x => x.Ilce).OrderBy(x => x).ToList() ?? new List<string>();

            return Ok(districts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "İlçe listesi yüklenirken hata oluştu", message = ex.Message });
        }
    }

    public class CityData
    {
        public string Il { get; set; }
        public List<DistrictData> Ilce { get; set; }
    }

    public class DistrictData
    {
        public string Ilce { get; set; }
        public List<string> Mahalle { get; set; }
    }


    public async Task<IActionResult> Firmalar(CompanySearchDto model)
    {
        if (model.Page < 1) model.Page = 1;
        if (model.Size < 1) model.Size = 20;

        var response = await _companyClientService.GetCompanies(new GetCompaniesQueryRequest()
        {
            Page = model.Page,
            Size = model.Size,
            CompanyName = model.Search,
            Search = model.Search,
            Province = model.Province,
            District = model.District,
            SortBy = model.SortBy,
            UserTypes = "Emlakçı",
            IsConsultant = "0",
            IsActive = "active"
        }, CancellationToken.None);

        var result = await _companyClientService.GetTotalConsultantCount(new GetTotalConsultantCountQueryRequest()
        , CancellationToken.None);

        ViewBag.CurrentPage = model.Page;
        ViewBag.PageSize = model.Size;
        ViewBag.CurrentSearch = model.Search;
        ViewBag.CurrentProvince = model.Province;
        ViewBag.CurrentDistrict = model.District;
        ViewBag.CurrentSortBy = model.SortBy;
        ViewBag.TotalItems = response?.Meta?.Pagination?.TotalItem;
        ViewBag.TotalPages = response?.Meta?.Pagination?.TotalPage;
        ViewBag.HasPreviousPage = model.Page > 1;
        ViewBag.HasNextPage = model.Page < (response?.Meta?.Pagination?.TotalPage ?? 0);
        ViewBag.TotalFirmCount = response?.Meta?.Pagination?.TotalItem ?? 0; ;
        ViewBag.TotalUserCount = result?.Data?.TotalCount ?? 0;

        model.Companies = response?.Data;

        return View(model);
    }

    [HttpGet]
    public async Task<JsonResult> SearchIlan(string searchTerm)
    {
        var results = await _context.IlanBilgileri
            .AsNoTracking()
            .Where(i => i.Title.Contains(searchTerm))
            .Select(i => new { i.Id, i.Title })
            .ToListAsync();
        return Json(results);
    }

    [HttpGet]
    public IActionResult AddComment()
    {
        return View();
    }

    public ActionResult GununIlaniSiparisi()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(string content, string firstName = null, string lastName = null)
    {
        var comment = new Comment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            IsApproved = false // Varsayılan onaysız
        };

        if (HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            comment.UserFullName = $"{user.FirstName} {user.LastName}";
            comment.UserId = user.Id;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            if (!string.IsNullOrEmpty(content) && !content.Contains("http"))
            {
                comment.UserFullName = $"{firstName} {lastName}";
                comment.UserId = null;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        if (User.Identity.Name != AdminEmail && User.Identity.Name != "exdel.txt@gmail.com")
        {
            return Unauthorized();
        }

        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Yorum Onaylama
    [HttpPost]
    public async Task<IActionResult> ApproveComment(int commentId)
    {
        if (User.Identity.Name != AdminEmail)
        {
            return Unauthorized();
        }

        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }

        comment.IsApproved = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

    public async Task<IActionResult> TumYorumlar()
    {
        var commentsQuery = _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .AsQueryable();

        // Admin değilse sadece onaylıları gör
        if (User.Identity.Name != AdminEmail)
        {
            commentsQuery = commentsQuery.Where(c => c.IsApproved);
        }

        var yorumlar = await commentsQuery.Take(20).ToListAsync();
        return View(yorumlar);
    }

    [HttpGet("arama")]
    public async Task<IActionResult> Arama([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Arama terimi boş olamaz.");
        }
        var ilanlar = await _context.IlanBilgileri
            .AsNoTracking()
            .Where(i => i.Title.Contains(term))
            .ToListAsync();
        return Ok(ilanlar);
    }

    public async Task<IActionResult> Index(int? ilanId)
    {
        var bugun = DateTime.Today;
        string userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        userIpAddress = userIpAddress.Replace(":", "_").Replace(".", "_");

        var gununIlan = await _context.GununIlanlari.FirstOrDefaultAsync(ilan => ilan.YayinlanmaTarihi.Date == bugun)
            ?? await _context.GununIlanlari
                .OrderByDescending(ilan => ilan.YayinlanmaTarihi)
                .FirstOrDefaultAsync();

        if (gununIlan != null)
        {
            var ipAddressKey = $"IP_{userIpAddress}_{gununIlan.Id}";

            if (HttpContext.Request.Cookies[ipAddressKey] == null)
            {
                try
                {
                    gununIlan.GoruntulenmeSayisi += 1;
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE IlanBilgileri SET GoruntulenmeSayisi = GoruntulenmeSayisi + 1, GoruntulenmeTarihi = {0} WHERE Id = {1}",
                        DateTime.Now, gununIlan.Id);
                    await _context.SaveChangesAsync();
                }
                catch { }

                CookieOptions cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
                Response.Cookies.Append(ipAddressKey, "1", cookieOptions);
            }
            ViewBag.GununIlan = gununIlan;
            var ilgiliFotograflar = await _context.Photos.AsNoTracking().Where(photo => photo.IlanId == gununIlan.Id).ToListAsync();
            ViewBag.Fotograflar = ilgiliFotograflar;

            var kullanici = await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == gununIlan.Email);
            if (kullanici != null)
            {
                ViewBag.KullaniciAdSoyad = $"{kullanici.FirstName} {kullanici.LastName}";
                string companyName = !string.IsNullOrEmpty(kullanici.CompanyName)
                                    ? kullanici.CompanyName
                                    : await _context.ConsultantInvitations.AsNoTracking().Where(ci => ci.Email == gununIlan.Email).Select(ci => ci.CompanyName).FirstOrDefaultAsync();
                ViewBag.CompanyName = companyName;
            }
        }
        else
        {
            ViewBag.GununIlan = null;
            ViewBag.Fotograflar = null;
            ViewBag.KullaniciAdSoyad = null;
            ViewBag.CompanyName = null;
        }

        var tumIlanlarDto = new TumIlanlarDTO
        {
            KonutIlanlariCount = 0,
            IsYeriIlanlariCount = 0,
            TuristikTesisIlanlariCount = 0,
            ArsaIlanlariCount = 0,
            BahceIlanlariCount = 0,
            TarlaIlanlariCount = 0,
            TotalIlanlar = 0,
            AllIlanlar = new List<IlanModel>(),
            SehirIlanSayilari = new List<TumIlanlarDTO.SehirIlanSayisiDTO>()
        };

        // --- ÜYE GÖRÜNTÜLEME İŞLEMİ ---
        // Profil fotoğrafı olmayanlar da dahil tüm kullanıcılar çekiliyor (UserTypes dolu olanlar)
        var allUsers = _context.Users
            .AsNoTracking()
            .Where(u => !string.IsNullOrEmpty(u.UserTypes)) // Sadece kullanıcı tipi olanlar
            .OrderByDescending(u => u.RegistrationDate)
            .Take(10) // İsteğe göre sayıyı artırabilirsiniz
            .ToList();

        // Yolların düzeltilmesi
        foreach (var user in allUsers)
        {
            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                string path = user.ProfilePicturePath.Replace("\\", "/");

                if (path.Contains("wwwroot/"))
                {
                    path = path.Substring(path.IndexOf("wwwroot/") + 8);
                }
                else if (path.Contains("wwwroot"))
                {
                    path = path.Substring(path.IndexOf("wwwroot") + 7);
                }

                if (!path.StartsWith("/"))
                {
                    path = "/" + path;
                }

                user.ProfilePicturePath = path;
            }
            // Not: Profil fotoğrafı yoksa (null/empty) Index.cshtml'de default resim göstereceğiz.
        }

        tumIlanlarDto.Users = allUsers;

        // Yorumlar: Admin ise onaysızları da görsün
        var commentsQuery = _context.Comments.AsNoTracking().OrderByDescending(c => c.CreatedAt).AsQueryable();

        if (User.Identity.Name != AdminEmail)
        {
            commentsQuery = commentsQuery.Where(c => c.IsApproved);
        }

        tumIlanlarDto.Comments = await commentsQuery.Take(3).ToListAsync();

        ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        ViewBag.UserId = User.Identity?.Name;

        return View(tumIlanlarDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetDailyOfferViewCount()
    {
        var bugun = DateTime.Today;
        var gununIlan = await _context.GununIlanlari.FirstOrDefaultAsync(ilan => ilan.YayinlanmaTarihi.Date == bugun)
            ?? await _context.GununIlanlari.OrderByDescending(ilan => ilan.YayinlanmaTarihi).FirstOrDefaultAsync();

        if (gununIlan == null) return Json(new { success = false, count = 0 });

        string userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        userIpAddress = userIpAddress.Replace(":", "_").Replace(".", "_");
        var ipAddressKey = $"IP_{userIpAddress}_{gununIlan.Id}";

        if (HttpContext.Request.Cookies[ipAddressKey] == null)
        {
            try
            {
                gununIlan.GoruntulenmeSayisi += 1;
                await _context.Database.ExecuteSqlRawAsync("UPDATE IlanBilgileri SET GoruntulenmeSayisi = GoruntulenmeSayisi + 1, GoruntulenmeTarihi = {0} WHERE Id = {1}", DateTime.Now, gununIlan.Id);
                await _context.SaveChangesAsync();
            }
            catch { }
            Response.Cookies.Append(ipAddressKey, "1", new CookieOptions { Expires = DateTime.Now.AddDays(1) });
        }

        return Json(new { success = true, count = gununIlan.GoruntulenmeSayisi });
    }

    [HttpGet]
    public async Task<IActionResult> TestGununIlanlari()
    {
        var bugun = DateTime.Today;
        var bugununIlanlari = await _context.GununIlanlari.AsNoTracking().Where(ilan => ilan.YayinlanmaTarihi.Date == bugun).ToListAsync();
        var tumGununIlanlari = await _context.GununIlanlari.AsNoTracking().OrderByDescending(ilan => ilan.YayinlanmaTarihi).Take(5)
            .Select(ilan => new { ilan.Id, ilan.Title, ilan.YayinlanmaTarihi, ilan.Price }).ToListAsync();

        return Json(new
        {
            BugununIlanlari = bugununIlanlari,
            TumGununIlanlari = tumGununIlanlari,
            BugunTarihi = bugun.ToString("yyyy-MM-dd"),
            ToplamGununIlanSayisi = await _context.GununIlanlari.CountAsync()
        });
    }

    private async Task UpdateVisitorCountsAsync()
    {
        var activeVisitorCount = await _context.Visitors.AsNoTracking().Where(v => v.VisitTime > DateTime.Now.AddMinutes(-10)).CountAsync();
        var visitorCount = await _context.VisitorCounts.AsNoTracking().FirstOrDefaultAsync();
        if (visitorCount != null)
        {
            var existingVisitorCount = await _context.VisitorCounts.FirstOrDefaultAsync();
            if (existingVisitorCount != null)
            {
                existingVisitorCount.ActiveVisitors = activeVisitorCount;
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            var totalVisitors = await _context.Visitors.CountAsync();
            visitorCount = new VisitorCount { ActiveVisitors = activeVisitorCount, TotalVisitors = totalVisitors };
            _context.VisitorCounts.Add(visitorCount);
            await _context.SaveChangesAsync();
        }
    }

    [HttpGet]
    public async Task<IActionResult> About()
    {
        var aboutUsContents = await _context.AboutUsContents.AsNoTracking().ToListAsync();
        if (!aboutUsContents.Any())
        {
            _context.AboutUsContents.AddRange(
                new AboutUsContent { Key = "intro-text", Content = "SİZ DE BİZİM KİM OLDUĞUMUZU MERAK EDİP HİKAYEMİZİ OKUYORSANIZ, SİZDE BİZDEN BİRİSİNİZ DEMEKTİR. ÇÜNKÜ BİZ ŞUNU SÖYLÜYORUZ: “VARSA BİZDEN İYİSİ, O DA BİZDEN BİRİSİ.”" },
                new AboutUsContent { Key = "final-text", Content = "BİZDEN BİRİSİ OLUN, EN İYİSİNE ULAŞIN! <br /> <br /> <strong>“SEVVAL.COM EKİBİ”</strong>" }
            );
            await _context.SaveChangesAsync();
            aboutUsContents = await _context.AboutUsContents.AsNoTracking().ToListAsync();
        }
        ViewBag.AboutUsContents = aboutUsContents.ToDictionary(c => c.Key, c => c.Content);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAboutUsContent([FromBody] AboutUsContentUpdateDto model)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user != null && user.Email == "sftumen41@gmail.com")
        {
            var contentToUpdate = await _context.AboutUsContents.FirstOrDefaultAsync(c => c.Key == model.Key);
            if (contentToUpdate == null) { contentToUpdate = new AboutUsContent { Key = model.Key, Content = model.Content }; _context.AboutUsContents.Add(contentToUpdate); }
            else { contentToUpdate.Content = model.Content; _context.AboutUsContents.Update(contentToUpdate); }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "İçerik başarıyla güncellendi." });
        }
        return Unauthorized(new { success = false, message = "Yetkisiz erişim." });
    }

    public class AboutUsContentUpdateDto { public string Key { get; set; } public string Content { get; set; } }
    public class SocialMediaUpdateDto { public string Platform { get; set; } public string Title { get; set; } public string Description { get; set; } public string Url { get; set; } public string ImagePath { get; set; } }
    public class ContactInfoUpdateDto { public string Field { get; set; } public string Value { get; set; } }
    public class SocialMediaOverridesDto { public Dictionary<string, Dictionary<string, string>> Overrides { get; set; } = new(); }

    private static string BuildSocialKey(string platform, string field) { return $"social:{platform}:{field}"; }

    [HttpGet("api/social-media/overrides")]
    public async Task<IActionResult> GetSocialMediaOverrides()
    {
        var keys = await _context.AboutUsContents.AsNoTracking().Where(c => c.Key.StartsWith("social:")).ToListAsync();
        var result = new SocialMediaOverridesDto();
        foreach (var item in keys)
        {
            var parts = item.Key.Split(':');
            if (parts.Length == 3)
            {
                var platform = parts[1]; var field = parts[2];
                if (!result.Overrides.ContainsKey(platform)) result.Overrides[platform] = new Dictionary<string, string>();
                result.Overrides[platform][field] = item.Content;
            }
        }
        return Json(new { success = true, data = result });
    }

    [HttpPost("api/social-media/update")]
    public async Task<IActionResult> UpdateSocialMedia([FromBody] SocialMediaUpdateDto model)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user != null && user.Email == "sftumen41@gmail.com")
        {
            if (string.IsNullOrWhiteSpace(model?.Platform)) return BadRequest(new { success = false, message = "Platform zorunludur." });
            var normalized = model.Platform.Trim();
            var keysToUpsert = new List<(string Key, string Value)>
            {
                (BuildSocialKey(normalized, "title"), model.Title ?? string.Empty),
                (BuildSocialKey(normalized, "description"), model.Description ?? string.Empty),
                (BuildSocialKey(normalized, "url"), model.Url ?? string.Empty),
                (BuildSocialKey(normalized, "imagePath"), model.ImagePath ?? string.Empty)
            };
            foreach (var (key, value) in keysToUpsert)
            {
                var existing = await _context.AboutUsContents.FirstOrDefaultAsync(c => c.Key == key);
                if (existing == null) _context.AboutUsContents.Add(new AboutUsContent { Key = key, Content = value });
                else { existing.Content = value; _context.AboutUsContents.Update(existing); }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sosyal medya içeriği güncellendi." });
        }
        return Unauthorized(new { success = false, message = "Yetkisiz erişim." });
    }

    [HttpGet("api/contact-info/overrides")]
    public async Task<IActionResult> GetContactInfoOverrides()
    {
        var overrides = await _context.AboutUsContents.AsNoTracking().Where(c => c.Key.StartsWith("contact:")).ToListAsync();
        var contactOverrides = new Dictionary<string, string>();
        foreach (var item in overrides) { var field = item.Key.Replace("contact:", ""); contactOverrides[field] = item.Content; }
        return Json(new { success = true, data = new { overrides = contactOverrides } });
    }

    [HttpPost("api/contact-info/update")]
    public async Task<IActionResult> UpdateContactInfoOverride([FromBody] ContactInfoUpdateDto model)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null || user.Email != "sftumen41@gmail.com") return Unauthorized(new { success = false, message = "Yetkisiz erişim." });
        var key = $"contact:{model.Field}";
        var contentToUpdate = await _context.AboutUsContents.FirstOrDefaultAsync(c => c.Key == key);
        if (contentToUpdate == null) _context.AboutUsContents.Add(new AboutUsContent { Key = key, Content = model.Value });
        else { contentToUpdate.Content = model.Value; _context.AboutUsContents.Update(contentToUpdate); }
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = $"{model.Field} iletişim bilgisi başarıyla güncellendi." });
    }

    [HttpGet("api/proxy/social-media")]
    public async Task<IActionResult> GetSocialMediaProxy()
    {
        try { using var httpClient = new HttpClient(); var response = await httpClient.GetAsync("http://94.73.131.202:8090/api/v1/social-media"); if (response.IsSuccessStatusCode) return Content(await response.Content.ReadAsStringAsync(), "application/json"); return StatusCode((int)response.StatusCode, "External API error"); } catch (Exception ex) { return StatusCode(500, $"Proxy error: {ex.Message}"); }
    }

    [HttpGet("api/proxy/contact-info")]
    public async Task<IActionResult> GetContactInfoProxy()
    {
        try { using var httpClient = new HttpClient(); var response = await httpClient.GetAsync("http://94.73.131.202:8090/api/v1/contact-info"); if (response.IsSuccessStatusCode) return Content(await response.Content.ReadAsStringAsync(), "application/json"); return StatusCode((int)response.StatusCode, "External API error"); } catch (Exception ex) { return StatusCode(500, $"Proxy error: {ex.Message}"); }
    }

    [HttpGet("api/proxy/videos/{id}")]
    public async Task<IActionResult> GetVideoDetailProxy(int id)
    {
        try { using var httpClient = new HttpClient(); var response = await httpClient.GetAsync($"http://94.73.131.202:8090/api/v1/videos/{id}"); if (response.IsSuccessStatusCode) return Content(await response.Content.ReadAsStringAsync(), "application/json"); return StatusCode((int)response.StatusCode, "External API error"); } catch (Exception ex) { return StatusCode(500, $"Proxy error: {ex.Message}"); }
    }

    [HttpGet("api/proxy/videos/suggested")]
    public async Task<IActionResult> GetSuggestedVideosProxy([FromQuery] int currentVideoId, [FromQuery] string category, [FromQuery] int limit = 6)
    {
        try { using var httpClient = new HttpClient(); var url = $"http://94.73.131.202:8090/api/v1/videos/suggested?currentVideoId={currentVideoId}&category={Uri.EscapeDataString(category ?? string.Empty)}&limit={limit}"; var response = await httpClient.GetAsync(url); if (response.IsSuccessStatusCode) return Content(await response.Content.ReadAsStringAsync(), "application/json"); return StatusCode((int)response.StatusCode, "External API error"); } catch (Exception ex) { return StatusCode(500, $"Proxy error: {ex.Message}"); }
    }

    [HttpPost("api/proxy/videos/{id}/vote")]
    public async Task<IActionResult> VoteVideoProxy(int id, [FromBody] object body)
    {
        try { using var httpClient = new HttpClient(); var json = body?.ToString() ?? "{}"; var response = await httpClient.PostAsync($"http://94.73.131.202:8090/api/v1/videos/{id}/vote", new StringContent(json, System.Text.Encoding.UTF8, "application/json")); if (response.IsSuccessStatusCode) return Content(await response.Content.ReadAsStringAsync(), "application/json"); return StatusCode((int)response.StatusCode, "External API error"); } catch (Exception ex) { return StatusCode(500, $"Proxy error: {ex.Message}"); }
    }

    public IActionResult Danismanlar() { return View(); }
    public IActionResult Channels() { return View(); }
    public IActionResult SatisSonrasiIslemler() { return View(); }
    public IActionResult Videos() { return View(); }

    [HttpPost]
    public async Task<IActionResult> SendEmail(string fullName, string userEmail, string subject, string message)
    {
        try { var mailMessage = new MailMessage { From = new MailAddress(userEmail), Subject = subject, Body = $"{fullName} ({userEmail}) mesaj gönderdi:<br/><br/>{message}", IsBodyHtml = true, }; mailMessage.To.Add("sevvalemlakiletisim@gmail.com"); using (var smtpClient = new SmtpClient("smtp.your-email-provider.com")) { smtpClient.Port = 587; smtpClient.Credentials = new NetworkCredential("your-email@example.com", "your-email-password"); smtpClient.EnableSsl = true; await smtpClient.SendMailAsync(mailMessage); } ViewBag.Message = "Mesajınız başarıyla gönderildi."; } catch (Exception ex) { ViewBag.Message = $"Mesaj gönderimi sırasında bir hata oluştu: {ex.Message}"; }
        return View();
    }
}