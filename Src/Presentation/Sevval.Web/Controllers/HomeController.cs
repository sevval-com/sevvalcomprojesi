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

    // İlan sayısını asenkron döndüren endpoint - AsNoTracking eklendi
    [HttpGet("api/ilan-sayisi")]
    public async Task<IActionResult> GetIlanSayisi()
    {
        var response = await _announcementClientService.GetActiveVisitorCount(new GetAnnouncementCountQueryRequest()
        {
            Status = "active"
        }, CancellationToken.None);

        return Ok(response);
    }

    // İl listesini döndüren endpoint (Performans için sadece il isimleri)
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

    // Seçilen ile ait ilçeleri döndüren endpoint
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

    // JSON deserialize için model sınıfları (HomeController sınıfı dışında tanımlanmalı)
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
        // Ensure valid pagination parameters
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

            // === İSTEĞE GÖRE EKLENEN FİLTRELER ===
            // Sadece Emlakçı (eski Kurumsal dahil), Danışman olmayan (0) ve Aktif olan firmalar listelenecek.
            UserTypes = "Emlakçı",  // Not: Backend'de "Kurumsal" değeri de otomatik destekleniyor
            IsConsultant = "0",
            IsActive = "active"
            // ======================================

        }, CancellationToken.None);

        // Bu sorgunun IsConsultant = "1" olan danışmanları getirdiği varsayılıyor.
        // Arama çubuğundaki "... danışman" sayımı için kullanılıyor.
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

        // Filtrelenmiş firma sayısı (IsConsultant = "0" olanlar)
        // Arama çubuğundaki "... Firma" sayımı için
        ViewBag.TotalFirmCount = response?.Meta?.Pagination?.TotalItem ?? 0; ;

        // Danışman sayısı (IsConsultant = "1" olanlar - GetTotalConsultantCount'tan gelen)
        // Arama çubuğundaki "... danışman" sayımı için
        ViewBag.TotalUserCount = result?.Data?.TotalCount ?? 0;

        model.Companies = response?.Data;

        return View(model);
    }










    // Asenkron arama metodu - Sadece gerekli alanlar seçilerek performans artırıldı
    [HttpGet]
    public async Task<JsonResult> SearchIlan(string searchTerm)
    {
        // AsNoTracking() ile sadece okuma amaçlı sorgularda performans artışı sağlanır.
        var results = await _context.IlanBilgileri
            .AsNoTracking() // Veriyi sadece okuduğumuz için izlemeyi kapatıyoruz.
            .Where(i => i.Title.Contains(searchTerm))
            .Select(i => new { i.Id, i.Title })
            .ToListAsync();
        return Json(results);
    }

    // Yorum eklemek için GET ve POST metodları
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
        // Kullanıcı giriş yapmışsa
        if (HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                // Kullanıcı bulunamazsa login sayfasına yönlendir
                return RedirectToAction("Login", "Account");
            }
            var comment = new Comment
            {
                Content = content,
                UserFullName = $"{user.FirstName} {user.LastName}",
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            //kullanıcının giriş yapmadığı durumlarda yorumda yapmaması lazım
            if (!string.IsNullOrEmpty(content) && !content.Contains("http")) //link paylaşımına izin verme
            {
                // Üye olmayan kullanıcı için isim ve soyisimden yorum oluşturma
                var comment = new Comment
                {
                    Content = content,
                    UserFullName = $"{firstName} {lastName}",
                    UserId = null, // Üye olmayan kullanıcı için UserId boş bırakılır
                    CreatedAt = DateTime.Now
                };
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
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Tüm yorumları gösteren sayfa - Performans için AsNoTracking eklendi
    public async Task<IActionResult> TumYorumlar()
    {
        var yorumlar = await _context.Comments
            .AsNoTracking() // Sadece okuma amaçlı olduğu için AsNoTracking kullanıldı.
            .Include(c => c.User) // User bilgileri ile birlikte çekiliyor
            .Take(10).OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(yorumlar);
    }

    [HttpGet("arama")]
    public async Task<IActionResult> Arama([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Arama terimi boş olamaz.");
        }
        // Loglama için (idealde ILogger kullanmalısınız)
        Console.WriteLine($"Arama yapılan terim: {term}");
        // AsNoTracking() ile sadece okuma amaçlı sorgularda performans artışı sağlanır.
        var ilanlar = await _context.IlanBilgileri
            .AsNoTracking()
            .Where(i => i.Title.Contains(term))
            .ToListAsync();
        Console.WriteLine($"Bulunan ilan sayısı: {ilanlar.Count}");
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

            /*var result = await _visitoryClientService.GetActiveVisitorCount(new GetActiveVisitorCountQueryRequest(), CancellationToken.None);

            if (result.IsSuccessfull)
                gununIlan.GoruntulenmeSayisi = result.Data.ActiveVisitorCount;*/

            // Sadece ilk kez görüntüleyen IP için 1 kez say (cookie 1 gün)
            if (HttpContext.Request.Cookies[ipAddressKey] == null)
            {
                try
                {
                    // 1) Günün ilanı sayacını artır
                    gununIlan.GoruntulenmeSayisi += 1;

                    // 2) Aynı zamanda, ilgili ilanın toplam görüntülenme sayısını da artır (raw SQL ile garanti altına al)
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE IlanBilgileri SET GoruntulenmeSayisi = GoruntulenmeSayisi + 1, GoruntulenmeTarihi = {0} WHERE Id = {1}",
                        DateTime.Now, gununIlan.Id);

                    await _context.SaveChangesAsync();
                }
                catch
                {
                }

                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                };
                Response.Cookies.Append(ipAddressKey, "1", cookieOptions);
            }
            ViewBag.GununIlan = gununIlan;
            // Fotoğrafları AsNoTracking ile getir
            var ilgiliFotograflar = await _context.Photos
                .AsNoTracking()
                .Where(photo => photo.IlanId == gununIlan.Id)
                .ToListAsync();
            ViewBag.Fotograflar = ilgiliFotograflar;
            // Kullanıcı ve firma bilgilerini asenkron ve performanslı şekilde getir
            var kullanici = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == gununIlan.Email);
            if (kullanici != null)
            {
                ViewBag.KullaniciAdSoyad = $"{kullanici.FirstName} {kullanici.LastName}";
                string companyName = !string.IsNullOrEmpty(kullanici.CompanyName)
                                    ? kullanici.CompanyName
                                    : await _context.ConsultantInvitations
                                              .AsNoTracking()
                                              .Where(ci => ci.Email == gununIlan.Email)
                                              .Select(ci => ci.CompanyName)
                                              .FirstOrDefaultAsync();
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



        var usersWithValidPhotos = _context.Users
            .AsNoTracking()
            .Where(u => !string.IsNullOrEmpty(u.UserTypes) && !string.IsNullOrEmpty(u.ProfilePicturePath))
            .OrderByDescending(u => u.RegistrationDate)
            .Take(10)
            .ToList();


        tumIlanlarDto.Users = usersWithValidPhotos;

        // Yorumları AsNoTracking ile getir
        var comments = await _context.Comments
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Take(3)
            .ToListAsync();
        tumIlanlarDto.Comments = comments;

        // Kullanıcı giriş durumunu ViewBag'e set et
        ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        ViewBag.UserId = User.Identity?.Name;

        return View(tumIlanlarDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetDailyOfferViewCount()
    {
        var bugun = DateTime.Today;
        var gununIlan = await _context.GununIlanlari.FirstOrDefaultAsync(ilan => ilan.YayinlanmaTarihi.Date == bugun)
            ?? await _context.GununIlanlari
                .OrderByDescending(ilan => ilan.YayinlanmaTarihi)
                .FirstOrDefaultAsync();

        if (gununIlan == null)
            return Json(new { success = false, count = 0 });

        // Benzersiz IP bazlı günlük sayım (Index ile aynı anahtar formatı)
        string userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        userIpAddress = userIpAddress.Replace(":", "_").Replace(".", "_");
        var ipAddressKey = $"IP_{userIpAddress}_{gununIlan.Id}";

        if (HttpContext.Request.Cookies[ipAddressKey] == null)
        {
            try
            {
                // Günün ilanı sayaç artışı
                gununIlan.GoruntulenmeSayisi += 1;

                // İlanın toplam görüntülenmesini de artır
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE IlanBilgileri SET GoruntulenmeSayisi = GoruntulenmeSayisi + 1, GoruntulenmeTarihi = {0} WHERE Id = {1}",
                    DateTime.Now, gununIlan.Id);

                await _context.SaveChangesAsync();
            }
            catch { }

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(ipAddressKey, "1", cookieOptions);
        }

        return Json(new { success = true, count = gununIlan.GoruntulenmeSayisi });
    }

    // Günün ilanlarını kontrol etmek için test endpoint'i
    [HttpGet]
    public async Task<IActionResult> TestGununIlanlari()
    {
        var bugun = DateTime.Today;

        // Bugünün tarihine sahip ilanları getir
        var bugununIlanlari = await _context.GununIlanlari
            .AsNoTracking()
            .Where(ilan => ilan.YayinlanmaTarihi.Date == bugun)
            .ToListAsync();

        // Tüm günün ilanlarını getir (son 5 tanesi)
        var tumGununIlanlari = await _context.GununIlanlari
            .AsNoTracking()
            .OrderByDescending(ilan => ilan.YayinlanmaTarihi)
            .Take(5)
            .Select(ilan => new {
                ilan.Id,
                ilan.Title,
                ilan.YayinlanmaTarihi,
                ilan.Price
            })
            .ToListAsync();

        return Json(new
        {
            BugununIlanlari = bugununIlanlari,
            TumGununIlanlari = tumGununIlanlari,
            BugunTarihi = bugun.ToString("yyyy-MM-dd"),
            ToplamGununIlanSayisi = await _context.GununIlanlari.CountAsync()
        });
    }

    // Ziyaretçi sayılarının güncellenmesi (asenkron) - AsNoTracking eklendi
    private async Task UpdateVisitorCountsAsync()
    {
        var activeVisitorCount = await _context.Visitors
            .AsNoTracking()
            .Where(v => v.VisitTime > DateTime.Now.AddMinutes(-10))
            .CountAsync();

        var visitorCount = await _context.VisitorCounts.AsNoTracking().FirstOrDefaultAsync();
        if (visitorCount != null)
        {
            // Eğer sadece ActiveVisitors güncellenecekse, nesneyi çekip sonra güncelleyip SaveChangesAsync yapmak yerine
            // direkt olarak veritabanı update sorgusu yazılabilir (örneğin ExecuteUpdateAsync() ile EF Core 7+).
            // Ancak mevcut haliyle de çalışır, sadece küçük bir performans maliyeti olabilir.
            var existingVisitorCount = await _context.VisitorCounts.FirstOrDefaultAsync();
            if (existingVisitorCount != null)
            {
                existingVisitorCount.ActiveVisitors = activeVisitorCount;
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            // Yeni bir VisitorCount kaydı ekleniyorsa, total visitor count'ı da çekmek gerekir.
            // Bu sorgu AsNoTracking olmayacak çünkü bir nesne ekleniyor.
            var totalVisitors = await _context.Visitors.CountAsync();
            visitorCount = new VisitorCount
            {
                ActiveVisitors = activeVisitorCount,
                TotalVisitors = totalVisitors
            };
            _context.VisitorCounts.Add(visitorCount);
            await _context.SaveChangesAsync();
        }
    }


    [HttpGet]
    public async Task<IActionResult> About()
    {
        // Tüm AboutUsContent verilerini veritabanından çek
        var aboutUsContents = await _context.AboutUsContents.AsNoTracking().ToListAsync();

        // Eğer veritabanında hiç içerik yoksa veya eksikse varsayılan içerikleri ekle
        if (!aboutUsContents.Any())
        {
            // Veritabanına varsayılan içerikleri ekle
            _context.AboutUsContents.AddRange(
                new AboutUsContent { Key = "intro-text", Content = "SİZ DE BİZİM KİM OLDUĞUMUZU MERAK EDİP HİKAYEMİZİ OKUYORSANIZ, SİZDE BİZDEN BİRİSİNİZ DEMEKTİR. ÇÜNKÜ BİZ ŞUNU SÖYLÜYORUZ: “VARSA BİZDEN İYİSİ, O DA BİZDEN BİRİSİ.”" },
                new AboutUsContent { Key = "color-choice", Content = "MAVİ VE BEYAZ RENKLERİ TERCİH ETTİK. ÇÜNKÜ MAVİ, HUZUR VE GÜVENİ; BEYAZ İSE SADELİĞİ VE SAFLIĞI HİSSETTİRİYOR." },
                new AboutUsContent { Key = "interface-design", Content = "ARAYÜZÜMÜZÜ ÇOK YUMUŞAK VE KOLAY YAPMAYA ÇALIŞTIK. MİSAFİRLERİMİZ PEK RAHAT GİREBİLSİNLER VE BEKLEMELER OLMASIN İSTEDİK." },
                new AboutUsContent { Key = "website-vision", Content = "SİTEMİZ SADECE EMLAK SAYFASI DEĞİLDİR. 2 YIL BOYUNCA SADECE EMLAK İLANLARI YAYINI YAPMAK İSTİYORUZ. BU KONUDA TREND SAĞLADIKTAN SONRA GALERİ VE DİĞER TÜM TİCARET DALLARINDA HİZMET VERECEĞİZ." },
                new AboutUsContent { Key = "additional-features-1", Content = "DİĞER BAZI SİTELERDE HALEN AKTİF OLMADIĞINI DÜŞÜNDÜĞÜMÜZ ÖZEL SEKMELERİ VE GÜZELLİKLERİ DAHA İLK BAŞTAN SİTEMİZE EKLEDİK. BUNLARI SIRALAMAK İSTİYORUZ." },
                new AboutUsContent { Key = "feature-1", Content = "KALİTESİZ KARINCALI VİDEOLAR İZLETMİYORUZ. HER İLAN İÇİN 200 MB KAPASİTE AÇTIK." },
                new AboutUsContent { Key = "feature-2", Content = "VİDEOLARDA 1 DAKİKA SINIRI KOYMADIK. TAM 2 KATI 2 DAKİKALIK İLAN VEREBİLİRSİNİZ." },
                new AboutUsContent { Key = "feature-3", Content = "VİDEOLARI TEK TIKLA TELEFONUNUZA VEYA BİLGİSAYARINIZA İNDİREBİLİRSİNİZ." },
                new AboutUsContent { Key = "feature-4", Content = "FOTO SINIRIMIZ 60 ADET. 30 DEĞİL. TAM İKİ KATI." },
                new AboutUsContent { Key = "feature-5", Content = "FOTOĞRAFLARI TEK TIKLA TELEFONUNUZA VEYA BİLGİSAYARINIZA İNDİREBİLİRSİNİZ." },
                new AboutUsContent { Key = "feature-6", Content = "FOTOĞRAFLARI TERS DÖNDÜRMÜYORUZ." },
                new AboutUsContent { Key = "feature-7", Content = "TELEFON UYGULAMASINDA FAVORİ ARAMALARI YAPARKEN GERİ DÖNÜŞE MÜSAADE EDİYORUZ. DİREK ÇIKIŞ YAPTIRMIYORUZ." },
                new AboutUsContent { Key = "feature-8", Content = "MESAJLAŞIRKEN GERİYE DÖNMEK İSTEDİĞİNİZDE EN BAŞA ATMIYORUZ." },
                new AboutUsContent { Key = "feature-9", Content = "EMLAKÇININ MAĞAZASINA ARAMA MOTORU KOYDUK. BEĞENDİĞİNİZ EMLAKÇININ İLANLARINA ANINDA ULAŞABİLECEKSİNİZ." },
                new AboutUsContent { Key = "feature-10", Content = "HER İLIN EMLAKÇILARINI GÖREBİLECEĞİNİZ ŞEKİLDE ARAMA MOTORU HAZIRLADIK." },
                new AboutUsContent { Key = "feature-11", Content = "HER SEKMEYE PUAN VERDİK. BU SAYEDE KALİTELİ İLANLARI ÖNE ÇIKARTIYORUZ." },
                new AboutUsContent { Key = "feature-12", Content = "İLANIN FİYATINI DEĞİŞTİRMEK İSTİYORSANIZ KALEM İŞARETİ KOYDUK. 1 SANİYEDE FİYATI DEĞİŞTİREBİLECEKSİNİZ." },
                new AboutUsContent { Key = "feature-13", Content = "1 TL LİK İLANLARIN YAYINLANMASINI ENGELLEDİK. EN BASİT İLAN 1.000 TL OLABİLİR ŞEKİLDE AYARLADIK." },
                new AboutUsContent { Key = "feature-14", Content = "GÜNLÜK KİRALIK EVLERE 30 TL 40 TL YAZMALARA MÜSAADE ETMEDİK. MİNİMUM 300 TL YAZILACAK ŞEKİLDE AYARLADIK." },
                new AboutUsContent { Key = "feature-15", Content = "KAT KARŞILIĞI İLANLARINA 0 TL DEĞERİ VERİP ALGORİTMYI KARIŞTIRMADIK. KAT KARŞILIĞINA FİYAT KOYDURMUYORUZ." },
                new AboutUsContent { Key = "feature-16", Content = "PARAYI BASTIRAN İLANI ÖNE ÇIKARTIR ŞEKLİNDE BİR POLİTİKAMIZ YOK. EN GÜZEL İLANI OLAN KİŞİ HEP ÖNE ÇIKARILIR." },
                new AboutUsContent { Key = "feature-17", Content = "AYNI IP'DEN BİR KERE DAHA GİRİŞ YAPSANIZ BİLE TEK GÖSTERİM OLARAK SAYIYORUZ." },
                new AboutUsContent { Key = "feature-18", Content = "DOPİNGLERİMİZDE GÖRÜNTÜLEME SÖZÜ VERİYORUZ. VE UYGUN OLMAYAN DOPİNGDE FİYAT HATALI İSE YARDIMCI OLMUYORUZ." },
                new AboutUsContent { Key = "feature-19", Content = "İLANLARA BEĞENİ ÖZELELİĞİ EKLEDİK. VE BU ÖZELLİĞİ HER MÜŞTERİMİZ GÖRSÜN VE ETKİLENSİN ONA GÖRE KARAR VERSİN." },
                new AboutUsContent { Key = "feature-20", Content = "İLANLARA YORUM YAPABİLME ÖZELLİĞİ EKLEDİK. HAKARET İÇERMEDİĞİ SÜRECE HER TÜRLÜ YORUMU YAPABİLİRSİNİZ." },
                new AboutUsContent { Key = "feature-21", Content = "TELEFONDA DANIŞMANLAR ARASI İLAN ATAMAYA MÜSAADE ETTİK." },
                new AboutUsContent { Key = "feature-22", Content = "EMLAKÇI PANELİNDE İLAN ARAMASI YAPARKEN TÜM DANIŞMANLAR SEKMESİNİ KOYDUK. TEK TEK HEPSİNİ SEÇMEK ZORUNDA KALMIYORSUNUZ." },
                new AboutUsContent { Key = "feature-23", Content = "EMLAKÇI PANELİNDE DİĞER DANIŞMAN ARKADAŞLARINIZIN YAYINDA OLMAYAN İLANLARINI GÖREBİLİYORSUNUZ." },
                new AboutUsContent { Key = "feature-24", Content = "AÇIKLAMA KISMINDA 1 SATIR ATLIYORUM. 2 SATIR ATLIYOR. KOPYALA YAPIŞTIR YAPSAK DA GİBİ ŞİKAYETLER ALMAZSINIZ." },
                new AboutUsContent { Key = "feature-25", Content = "OFİSİNİZİN CAM İLANLARINI SÜSLEYEBİLMENİZ İÇİN 5 ÇEŞİT ÇIKTI ALMA ÖZELLİĞİ EKLEDİK." },
                new AboutUsContent { Key = "additional-features-2", Content = "KULLANICILARIMIZA TİCARET YAPARKEN HERHANGİ BİR CEZA VEYA KISITLAMA UYGULAMIYORUZ" },
                new AboutUsContent { Key = "restriction-1", Content = "HARİTALARDA POLİGON DIŞINA TAŞTI DİYE CEZA VERMİYORUZ." },
                new AboutUsContent { Key = "restriction-2", Content = "GÜNLÜK 100 MESAJ SINIRI KOYMUYORUZ. DİLEDİĞİNİZ KADAR MESAJ ATABİLİRSİNİZ." },
                new AboutUsContent { Key = "restriction-3", Content = "İLANINIZIN VİTRİNE İSTEDİĞİNİZ FOTOĞRAFI KOYABİLİRSİNİZ." },
                new AboutUsContent { Key = "restriction-4", Content = "İLANINIZIN VİTRİN FOTOSUNA YAZI KOYAMAZSINIZ DEMİYORUZ." },
                new AboutUsContent { Key = "restriction-5", Content = "BİR HATA OLDUĞUNDA YAPAY ZEKA MANTIĞIYLA İLANI SORMADAN ETMEDEN YAYINDAN KALDIRMIYORUZ. SADECE YAYINI ASKIYA ALIYORUZ." },
                new AboutUsContent { Key = "restriction-6", Content = "EMLAKÇININ VEYA BİREYSEL MÜŞTERİLERİMİZİN İLANLARINDA GÖRÜNEN TELEFON NUMARASINI KOPYALAYAMAZ YAPMIYORUZ." },
                new AboutUsContent { Key = "restriction-7", Content = "DİYELİM Kİ TATİLE ÇIKTINIZ. TÜM İLANLARINIZI 1 TIKLA PASİF YAPABİLİRSİNİZ. VEYA SADECE MESAJ İLE 'BANA ULAŞILSIN' DİYEBİLİRSİNİZ. TÜM İLANLARLA TEK TEK UĞRAŞMIYORSUNUZ." },
                new AboutUsContent { Key = "restriction-8", Content = "KENDİ İLAN AÇIKLAMANIZA BAŞKA SİTELERİN LİNKLERİNİ YAZMAYA MÜSAADE EDİYORUZ." },
                new AboutUsContent { Key = "restriction-9", Content = "MESAJLAŞIRKEN BAŞKA SİTELERİN LİNKLERİNİ KOYMAYI ENGELLEMİYORUZ." },
                new AboutUsContent { Key = "restriction-10", Content = "FAZLA İLAN SEKMESİ AÇILDIĞINDA 'BEN ROBOT DEĞİLİM' GİBİ ENGELLEMELER GETİRMİYORUZ. BİZİM İLANLARIMIZDAN 1000 TANE BİLE AYNI ANDA SEKME AÇABİLİRSİNİZ. SINIR KOYMUYORUZ." },
                new AboutUsContent { Key = "additional-features-3", Content = "DOPİNGLERİMİZ" },
                new AboutUsContent { Key = "doping-1", Content = "GÜNÜN İLANI DOPİNGİ: BU DOPİNGİMİZDE GÖRÜNTÜLEME SÖZÜ VERİYORUZ. GÖRÜNTÜLEME SÖZÜ VEREN ŞU AN İÇİN HİÇBİR FİRMA YOK. BU DOPİNGİMİZ ZATEN SİTEMİZİN ANA SAYFASINDA VE HERKESİN KESİNLİKLE GÖRDÜĞÜ BİR ALAN. ONUN İÇİN KENDİMİZE GÜVENİYORUZ. SÖZ VERDİĞİMİZ GÖRÜNTÜLEME GERÇEKLEŞMEZSE EKSİK GÖRÜNTÜLEME MİKTARININ ÜCRETİNİ İADE EDİYORUZ." },
                new AboutUsContent { Key = "doping-2", Content = "ANA SAYFA VİTRİN FOTOĞRAFI DOPİNGİ DEYİP BONCUK, ELEKTRİK SÜPÜRGESİ SATILAN EMLAK ARAMASI YAPAMAYACAĞI SAYFADA KÜÇÜCÜK FOTOYLA GÖSTERİM YAPIP FAHİŞ FİYATLAR İSTEMİYORUZ." },
                new AboutUsContent { Key = "additional-features-4", Content = "GÖRÜNTÜLEME ARTIRMAK KONULARI" },
                new AboutUsContent { Key = "view-increase-1", Content = "HER GÜN 10 ADET İLANI MANUEL OLARAK SİZ TARİH GÜNCELLEYİN. GÜNCELLEMEZSENİZ TABİ Kİ GÖRÜNTÜLEMENİZ ZAYIF OLUR DEMEYECEĞİZ." },
                new AboutUsContent { Key = "additional-features-5", Content = "MÜŞTERİ TEMSİLCİLİĞİ" },
                new AboutUsContent { Key = "customer-service-1", Content = "HİÇ BİR İŞİ DÜZELTMEYİP TELEFONU KAPATIRKEN YARDIMCI OLABİLECEĞİM BAŞKA BİR KONU VARMIDIR DİYE SORMUYORUZ." },
                new AboutUsContent { Key = "customer-service-2", Content = "YARIM SAAT ASANSÖR MÜZİĞİ DİNLETMİYORUZ." },
                new AboutUsContent { Key = "customer-service-3", Content = "TALEP VE ŞİKÂYET OLUŞTURDUĞUNUZDA İŞİNİZİ ÇÖZEMEDİĞİMİZDE TALEBİNİZ ÇÖZÜMLENDİ GİBİ MAİLLER ATMIYORUZ." },
                new AboutUsContent { Key = "additional-features-6", Content = "PAKET DEĞİŞİKLİKLERİ VE MUHASEBE DURUMLARI" },
                new AboutUsContent { Key = "package-change-1", Content = "HER TÜRLÜ PAKETE İSTEDİĞİ GİBİ GEÇİŞ YAPMA İMKANI VERİYORUZ. SADECE 1 PAKET DÜŞEBİLİRSİNİZ ŞARTI KOYMUYORUZ." },
                new AboutUsContent { Key = "package-change-2", Content = "HELE Kİ PAKETİNİZİ HİÇ DEĞİŞTİREMEZSİNİZ DEMİYORUZ." },
                new AboutUsContent { Key = "package-change-3", Content = "ÖDEME YAPILAMADIĞINDA HEMEN 10 GÜN İÇİNDE İLAN VERMEYİ DURDURMUYORUZ. 30 GÜN BOYUNCA İLAN VERİLEBİLİYOR." },
                new AboutUsContent { Key = "package-change-4", Content = "30 GÜN SONRA SADECE İLAN VERMEYİ DURDURUYORUZ. YENİ İLAN GİRİŞİNE MÜSAADE ETMEYECEĞİZ. 30 GÜN SONRA İLAN YENİLEYEBİLEBİLİYORSUNUZ." },
                new AboutUsContent { Key = "package-change-5", Content = "60 GÜNE KADAR VAR OLAN İLANLARI DÜZENLEYEBİLECEKSİNİZ." },
                new AboutUsContent { Key = "package-6", Content = "60 GÜN SONRA PANELE GİRİŞ YAPMASINI ENGELLEYECEĞİZ. DÜZENLEME YAPAMACAYAKSINIZ." },
                new AboutUsContent { Key = "package-change-7", Content = "120 GÜNE KADAR EVRAKLARI AVUKATA TESLİM ETMİYORUZ." },
                new AboutUsContent { Key = "additional-features-7", Content = "AVUKATLIK DURUMLAR" },
                new AboutUsContent { Key = "legal-1", Content = "MÜŞTERİNİZLE VEYA EMLKAÇINIZLA AVUKATLIK OLDUĞUNUZDA İLANLARINIZI 2 YIL BOYUNCA ARŞİVİMİZDEN ÇIKARTIP ANINDA SİZE TESLİM EDİYORUZ. ZATEN MÜŞTERİMİZ OLAN PARASINI ALDIĞIMIZ ÜRÜN İÇİN İKİNCİ BİR ÜCRET TALEP ETMİYORUZ. (TOPLU İLAN TALEPLERİ HARİÇ)" },
                new AboutUsContent { Key = "final-text", Content = "BİZİ SEÇTİĞİNİZDE, HER ZAMAN MÜŞTERİ ODAKLI, ESNEK, HIZLI VE GÜVENİLİR BİR HİZMET ALIRSINIZ. EN İYİ DENEYİMİ SAĞLAMAK İÇİN SÜREKLİ YENİLİKLER EKLİYOR, SİZLERE KALİTELİ VE KESİNTİSİZ BİR PLATFORM SUNMAK İÇİN ÇALIŞIYORUZ. SADECE BİR İLAN DEĞİL, TAMAMEN SİZİN İŞİNİZİ KOLAYLAŞTIRACAK BİR SİSTEM TASARLADIK. BİZDEN BİRİSİ OLUN, EN İYİSİNE ULAŞIN! <br /> <br /> <strong>“SEVVAL.COM EKİBİ”</strong>" }
            );
            await _context.SaveChangesAsync();
            aboutUsContents = await _context.AboutUsContents.AsNoTracking().ToListAsync(); // Yeni eklenenleri tekrar çek
        }

        ViewBag.AboutUsContents = aboutUsContents.ToDictionary(c => c.Key, c => c.Content);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAboutUsContent([FromBody] AboutUsContentUpdateDto model)
    {
        // Kullanıcının kimliğini UserManager üzerinden al
        var user = await _userManager.GetUserAsync(HttpContext.User);

        // Yalnızca belirli bir e-posta adresine sahip kullanıcının düzenlemesine izin ver
        if (user != null && user.Email == "sftumen41@gmail.com")
        {
            var contentToUpdate = await _context.AboutUsContents.FirstOrDefaultAsync(c => c.Key == model.Key);
            if (contentToUpdate == null)
            {
                // Eğer içerik mevcut değilse, yeni bir kayıt olarak ekle
                contentToUpdate = new AboutUsContent { Key = model.Key, Content = model.Content };
                _context.AboutUsContents.Add(contentToUpdate);
            }
            else
            {
                // Mevcut içeriği güncelle
                contentToUpdate.Content = model.Content;
                _context.AboutUsContents.Update(contentToUpdate);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "İçerik başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"İçerik güncellenirken bir hata oluştu: {ex.Message}" });
            }
        }
        return Unauthorized(new { success = false, message = "Yetkisiz erişim." });
    }

    // Hakkımızda içeriğini güncellemek için kullanılacak DTO
    public class AboutUsContentUpdateDto
    {
        public string Key { get; set; }
        public string Content { get; set; }
    }

    // Sosyal medya güncellemeleri için DTO
    public class SocialMediaUpdateDto
    {
        public string Platform { get; set; } // facebook, instagram, youtube, google, tikTok
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
    }

    public class ContactInfoUpdateDto
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }

    // Sosyal medya override'larını yüklemek için DTO
    public class SocialMediaOverridesDto
    {
        public Dictionary<string, Dictionary<string, string>> Overrides { get; set; } = new();
    }

    private static string BuildSocialKey(string platform, string field)
    {
        return $"social:{platform}:{field}";
    }

    [HttpGet("api/social-media/overrides")]
    public async Task<IActionResult> GetSocialMediaOverrides()
    {
        var keys = await _context.AboutUsContents
            .AsNoTracking()
            .Where(c => c.Key.StartsWith("social:"))
            .ToListAsync();

        var result = new SocialMediaOverridesDto();

        foreach (var item in keys)
        {
            // key format: social:{platform}:{field}
            var parts = item.Key.Split(':');
            if (parts.Length == 3)
            {
                var platform = parts[1];
                var field = parts[2];
                if (!result.Overrides.ContainsKey(platform))
                {
                    result.Overrides[platform] = new Dictionary<string, string>();
                }
                result.Overrides[platform][field] = item.Content;
            }
        }

        return Json(new { success = true, data = result });
    }

    [HttpPost("api/social-media/update")]
    public async Task<IActionResult> UpdateSocialMedia([FromBody] SocialMediaUpdateDto model)
    {
        // Kullanıcının kimliğini UserManager üzerinden al
        var user = await _userManager.GetUserAsync(HttpContext.User);

        // Yalnızca belirli bir e-posta adresine sahip kullanıcının düzenlemesine izin ver
        if (user != null && user.Email == "sftumen41@gmail.com")
        {
            if (string.IsNullOrWhiteSpace(model?.Platform))
            {
                return BadRequest(new { success = false, message = "Platform zorunludur." });
            }

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
                if (existing == null)
                {
                    _context.AboutUsContents.Add(new AboutUsContent { Key = key, Content = value });
                }
                else
                {
                    existing.Content = value;
                    _context.AboutUsContents.Update(existing);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sosyal medya içeriği güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Güncelleme sırasında hata: {ex.Message}" });
            }
        }

        return Unauthorized(new { success = false, message = "Yetkisiz erişim." });
    }

    [HttpGet("api/contact-info/overrides")]
    public async Task<IActionResult> GetContactInfoOverrides()
    {
        var overrides = await _context.AboutUsContents
            .AsNoTracking()
            .Where(c => c.Key.StartsWith("contact:"))
            .ToListAsync();

        var contactOverrides = new Dictionary<string, string>();
        foreach (var item in overrides)
        {
            var field = item.Key.Replace("contact:", "");
            contactOverrides[field] = item.Content;
        }

        return Json(new { success = true, data = new { overrides = contactOverrides } });
    }

    [HttpPost("api/contact-info/update")]
    public async Task<IActionResult> UpdateContactInfoOverride([FromBody] ContactInfoUpdateDto model)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null || user.Email != "sftumen41@gmail.com") // Yetkilendirme kontrolü
        {
            return Unauthorized(new { success = false, message = "Yetkisiz erişim." });
        }

        var key = $"contact:{model.Field}";
        var contentToUpdate = await _context.AboutUsContents.FirstOrDefaultAsync(c => c.Key == key);

        if (contentToUpdate == null)
        {
            _context.AboutUsContents.Add(new AboutUsContent { Key = key, Content = model.Value });
        }
        else
        {
            contentToUpdate.Content = model.Value;
            _context.AboutUsContents.Update(contentToUpdate);
        }

        try
        {
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"{model.Field} iletişim bilgisi başarıyla güncellendi." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Güncelleme sırasında bir hata oluştu: {ex.Message}" });
        }
    }

    // Proxy endpoints to handle mixed content issues
    [HttpGet("api/proxy/social-media")]
    public async Task<IActionResult> GetSocialMediaProxy()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://94.73.131.202:8090/api/v1/social-media");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode, "External API error");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Proxy error: {ex.Message}");
        }
    }

    [HttpGet("api/proxy/contact-info")]
    public async Task<IActionResult> GetContactInfoProxy()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://94.73.131.202:8090/api/v1/contact-info");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode, "External API error");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Proxy error: {ex.Message}");
        }
    }

    // Proxy: video detay
    [HttpGet("api/proxy/videos/{id}")]
    public async Task<IActionResult> GetVideoDetailProxy(int id)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://94.73.131.202:8090/api/v1/videos/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return StatusCode((int)response.StatusCode, "External API error");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Proxy error: {ex.Message}");
        }
    }

    // Proxy: önerilen videolar
    [HttpGet("api/proxy/videos/suggested")]
    public async Task<IActionResult> GetSuggestedVideosProxy([FromQuery] int currentVideoId, [FromQuery] string category, [FromQuery] int limit = 6)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = $"http://94.73.131.202:8090/api/v1/videos/suggested?currentVideoId={currentVideoId}&category={Uri.EscapeDataString(category ?? string.Empty)}&limit={limit}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return StatusCode((int)response.StatusCode, "External API error");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Proxy error: {ex.Message}");
        }
    }

    // Proxy: beğeni/oylama
    [HttpPost("api/proxy/videos/{id}/vote")]
    public async Task<IActionResult> VoteVideoProxy(int id, [FromBody] object body)
    {
        try
        {
            using var httpClient = new HttpClient();
            var json = body?.ToString() ?? "{}";
            var response = await httpClient.PostAsync(
                $"http://94.73.131.202:8090/api/v1/videos/{id}/vote",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return StatusCode((int)response.StatusCode, "External API error");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Proxy error: {ex.Message}");
        }
    }

    public IActionResult Danismanlar()
    {
        return View();
    }

    public IActionResult Channels()
    {
        return View();
    }

    public IActionResult SatisSonrasiIslemler()
    {
        return View();
    }

    public IActionResult Videos()
    {
        return View();
    }

    // SendEmail metodunu asenkron hale getirdik (zaten asenkrondur)
    [HttpPost]
    public async Task<IActionResult> SendEmail(string fullName, string userEmail, string subject, string message)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(userEmail),
                Subject = subject,
                Body = $"{fullName} ({userEmail}) mesaj gönderdi:<br/><br/>{message}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add("sevvalemlakiletisim@gmail.com");

            using (var smtpClient = new SmtpClient("smtp.your-email-provider.com")) // SMTP sunucunuzu buraya yazın
            {
                smtpClient.Port = 587; // Genellikle 587 veya 465
                smtpClient.Credentials = new NetworkCredential("your-email@example.com", "your-email-password"); // Kendi e-posta bilgileriniz
                smtpClient.EnableSsl = true; // SSL/TLS etkinleştir
                await smtpClient.SendMailAsync(mailMessage);
            }
            ViewBag.Message = "Mesajınız başarıyla gönderildi.";
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Mesaj gönderimi sırasında bir hata oluştu: {ex.Message}";
        }
        return View();
    }


}