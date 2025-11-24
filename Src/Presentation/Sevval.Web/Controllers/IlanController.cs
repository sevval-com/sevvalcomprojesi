using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using sevvalemlak.csproj.ClientServices.AnnouncementService;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Web.Dto.User;
using sevvalemlak.csproj.ClientServices.CompanyService;
using sevvalemlak.csproj.ClientServices.ConsultantService;
using sevvalemlak.csproj.ClientServices.RecentlyVisitedAnnouncement;
using sevvalemlak.csproj.Dto.Company;
using sevvalemlak.Dto;
using SixLabors.ImageSharp;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement;

public class IlanController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IRecentlyVisitedAnnouncementClientService _recentlyVisitedAnnouncementService;
    private readonly ICompanyClientService _companyClientService;
    private readonly IConsultantClientService _consultantClientService;
    private readonly IAnnouncementClientService _announcementClientService;
    // Bu liste geçici olarak ilanları saklayacak
    private static List<IlanModel> ilanlar = new List<IlanModel>();

    public IlanController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration
,
        IRecentlyVisitedAnnouncementClientService recentlyVisitedAnnouncementService,
        ICompanyClientService companyClientService,
        IConsultantClientService consultantClientService,
        IAnnouncementClientService announcementClientService)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _recentlyVisitedAnnouncementService = recentlyVisitedAnnouncementService;
        _companyClientService = companyClientService;
        _consultantClientService = consultantClientService;
        _announcementClientService = announcementClientService;
    }


    /// <summary>
    /// Firmaya ait danışman listesini getirir
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetConsultants(string userId)
    {
        try
        {
            var consultants = await _consultantClientService.GetConsultantsByCompany(new GetConsultantsByCompanyQueryRequest { UserId = userId,
            Page=1,
            Size=200
            }, CancellationToken.None);


            return PartialView("_ConsultantsPartial", consultants?.Data ?? new List<GetConsultantsByCompanyQueryResponse>());
        }
        catch (Exception)
        {
            // Return empty list on error
            return PartialView("_ConsultantsPartial", new List<GetConsultantsByCompanyQueryResponse>());
        }
    }



    public async Task<IActionResult> Details(string id, int page = 1, int size = 20)
    {
        CompanyDetailDto companyDetailDto = new CompanyDetailDto();
        companyDetailDto.UserId = id;
        companyDetailDto.CurrentPage = page;
        companyDetailDto.PageSize = size;

        var announcements = await
            _announcementClientService.GetAnnouncementsByCompany(new GetAnnouncementsByCompanyQueryRequest
            {
                UserId = id,
                Status = "active",
                Page = page,
                Size = size

            }, CancellationToken.None);


        if (announcements.IsSuccessfull)
        {
            companyDetailDto.Announcements = announcements?.Data ?? new List<GetAnnouncementsByCompanyQueryResponse>();
            
          
            if (announcements.Data?.Any() == true)
            {
               
                companyDetailDto.TotalCount = announcements?.Meta?.Pagination?.TotalItem??0;
                companyDetailDto.TotalPages = announcements?.Meta?.Pagination?.TotalPage??0;
                companyDetailDto.HasPreviousPage = page > 1;
                companyDetailDto.HasNextPage = page < (announcements?.Meta?.Pagination?.TotalPage ?? 1);
            }
        }

        var about = await _companyClientService.GetAboutUs(new GetAboutUsQueryRequest { UserId = id }, CancellationToken.None);

        if (about.IsSuccessfull)
        {
            companyDetailDto.About = about?.Data?? new GetAboutUsQueryResponse();
        }

        return View(companyDetailDto);



        /* var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);

         if (user == null)
         {
             return NotFound();
         }

         var userEmail = user.Email;
         List<IlanModel> ilanlar;

         if (!user.IsConsultant) // Kullanıcı şirket sahibi ise
         {
             var consultantEmails = _context.ConsultantInvitations.AsNoTracking()
                 .Where(ci => ci.InvitedBy == user.Id)
                 .Select(ci => ci.Email)
                 .ToList();

             ilanlar = _context.IlanBilgileri.AsNoTracking()
                 .Where(i => (i.Email == userEmail || consultantEmails.Contains(i.Email))
                     && i.Status == "active"
                     && i.Title != null
                     && i.Description != null
                     && i.Price != null)
                 .ToList();
         }
         else // Kullanıcı danışman ise
         {
             ilanlar = _context.IlanBilgileri.AsNoTracking()
                 .Where(i => i.Email == userEmail && i.Status == "active")
                 .ToList();
         }

         var ilanIds = ilanlar.Select(i => i.Id).ToList();
         var fotos = _context.Photos.AsNoTracking().Where(p => ilanIds.Contains(p.IlanId)).ToList();

         var sehirIlanSayilari = ilanlar
             .Where(i => !string.IsNullOrEmpty(i.sehir) && i.Status == "active")
             .GroupBy(i => i.sehir)
             .Select(g => new { Sehir = g.Key, IlanSayisi = g.Count() })
             .ToList();

         ViewBag.SehirIlanSayilari = sehirIlanSayilari;

         var firmaKullanicilari = _context.Users.AsNoTracking()
             .Where(u => u.Id == user.Id ||
                         _context.ConsultantInvitations.Any(ci => ci.InvitedBy == user.Id && ci.Email == u.Email))
             .ToList();

         var tumIlanlarDto = new TumIlanlarDTO
         {
             _Ilanlar = ilanlar,
             _Fotograflar = fotos,
             User = user,
             Users = firmaKullanicilari,
             KonutIlanlariCount = ilanlar.Count(i => i.Category == "Konut (Yaşam Alanı)"),
             IsYeriIlanlariCount = ilanlar.Count(i => i.Category == "İş Yeri"),
             TuristikTesisIlanlariCount = ilanlar.Count(i => i.Category == "Turistik Tesis"),
             ArsaIlanlariCount = ilanlar.Count(i => i.Category == "Arsa"),
             BahceIlanlariCount = ilanlar.Count(i => i.Category == "Bahçe"),
             TarlaIlanlariCount = ilanlar.Count(i => i.Category == "Tarla"),
         };

         return View(tumIlanlarDto);*/
        return View();
    }


    public async Task<IActionResult> UserIlanlar(UserAnnouncementsDto model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return NotFound();
        }

        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == model.Email);
        if (user == null)
        {
            return NotFound();
        }

   
        var request = new GetAnnouncementsByUserQueryRequest
        {
            Email = model.Email,
            Status = model.Status,
            Page = model.CurrentPage,
            Size = model.PageSize,
            SortBy = model.SortBy,
            SortOrder = model.SortOrder
        };

        var apiResponse = await _announcementClientService.GetAnnouncementsByUser(request, CancellationToken.None);

        model.User = user;

        if (apiResponse.IsSuccessfull && apiResponse.Data != null)
        {
            model.Announcements = apiResponse.Data;

             if (apiResponse.Meta?.Pagination != null)
            {
                var pagination = apiResponse.Meta.Pagination;
                model.CurrentPage = pagination.PageNumber;
                model.PageSize = pagination.PageSize;
                model.TotalPages = pagination.TotalPage;
                model.TotalCount = pagination.TotalItem;
                model.HasPreviousPage = pagination.PageNumber > 1;
                model.HasNextPage = pagination.PageNumber < pagination.TotalPage;
            }
        }

        return View(model);
    }




    [HttpGet("arama")]
    public async Task<IActionResult> Arama([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Arama terimi boş olamaz.");
        }
        // Arama kriterini logla
        Console.WriteLine($"Arama terimi: {term}");
        // Arama sorgusunu gerçekleştir
        var ilanlar = await _context.IlanBilgileri.AsNoTracking().Where(i => i.Title.Contains(term)).ToListAsync();
        // Sorgu sonucunu logla
        Console.WriteLine($"Bulunan ilan sayısı: {ilanlar.Count}");
        return Ok(ilanlar);
    }

    public async Task<IActionResult> Panel()
    {
        var userEmail = User.Identity.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            // Kullanıcı oturum açmamışsa veya e-postası yoksa giriş sayfasına yönlendir
            return RedirectToAction("Login", "Account");
        }

        // 1. Kullanıcıyı getir
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null)
        {
            // Kullanıcı bulunamazsa giriş sayfasına yönlendir
            return RedirectToAction("Login", "Account");
        }

        // 2. Aktif ilanları getir
        var ilanlar = await _context.IlanBilgileri
            .AsNoTracking()
            .Where(i => i.Email == userEmail && i.Status == "active")
            .ToListAsync();

        // 3. Haftalık arama, favoriler ve görüntülenmeleri getir
        var userWeeklySearch = await _context.HaftalikAramalar.FirstOrDefaultAsync(h => h.UserEmail == userEmail);
        var haftalikBegeniler = await _context.HaftalikBegeniler.FirstOrDefaultAsync(h => h.UserEmail == userEmail);
        var haftalikGoruntulenme = await _context.HaftalikGoruntulenmeler.FirstOrDefaultAsync(h => h.UserEmail == userEmail);

        // Haftalık arama verilerini yoksa başlat
        if (userWeeklySearch == null)
        {
            userWeeklySearch = new HaftalikArama
            {
                UserEmail = userEmail,
                Pazar = 0,
                Pazartesi = 0,
                Sali = 0,
                Carsamba = 0,
                Persembe = 0,
                Cuma = 0,
                Cumartesi = 0,
                Toplam = 0,
                LastResetDate = DateTime.Now // İlk oluşturulduğunda sıfırlama tarihini ayarla
            };
            _context.HaftalikAramalar.Add(userWeeklySearch);
        }
        else // Haftalık arama verisi varsa, haftalık sıfırlama kontrolü yap
        {
            // Haftanın ilk günü Pazartesi olarak kabul edelim
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)firstDayOfWeek);

            // Eğer bugün haftanın ilk günü ise ve son sıfırlama bu haftadan önceyse sıfırla
            if (DateTime.Today.DayOfWeek == firstDayOfWeek && userWeeklySearch.LastResetDate.Date < startOfWeek.Date)
            {
                userWeeklySearch.Pazar = 0;
                userWeeklySearch.Pazartesi = 0;
                userWeeklySearch.Sali = 0;
                userWeeklySearch.Carsamba = 0;
                userWeeklySearch.Persembe = 0;
                userWeeklySearch.Cuma = 0;
                userWeeklySearch.Cumartesi = 0;
                userWeeklySearch.Toplam = 0;
                userWeeklySearch.LastResetDate = DateTime.Now; // Sıfırlama tarihini güncelle
            }
        }

        // Haftalık favori verilerini yoksa başlat
        if (haftalikBegeniler == null)
        {
            haftalikBegeniler = new HaftalikBegeniler
            {
                UserEmail = userEmail,
                Pazar = 0,
                Pazartesi = 0,
                Sali = 0,
                Carsamba = 0,
                Persembe = 0,
                Cuma = 0,
                Cumartesi = 0,
                Toplam = 0
            };
            _context.HaftalikBegeniler.Add(haftalikBegeniler);
        }

        // Haftalık görüntülenme verilerini yoksa başlat
        if (haftalikGoruntulenme == null)
        {
            haftalikGoruntulenme = new HaftalikGoruntulenme { UserEmail = userEmail };
            _context.HaftalikGoruntulenmeler.Add(haftalikGoruntulenme);
        }

        // Haftalık görüntülenme değişimi için sözlük
        var gunlukGoruntulenmeDegisimi = new Dictionary<int, int>
        {
            { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 },
        };

        // Görüntülenme sayıları üzerinden hesaplamalar
        foreach (var ilan in ilanlar)
        {
            // GoruntulenmeTarihi boş değilse ve son 7 gün içindeyse işlenir
            if (ilan.GoruntulenmeTarihi != DateTime.MinValue && ilan.GoruntulenmeTarihi.Date >= DateTime.Now.AddDays(-7).Date)
            {
                int gun = (int)ilan.GoruntulenmeTarihi.DayOfWeek;
                gunlukGoruntulenmeDegisimi[gun] += ilan.GoruntulenmeSayisi; // Günlük görüntülenme sayısını artır
            }
        }

        // Haftalık görüntülenme verilerini güncelle
        haftalikGoruntulenme.Pazar = gunlukGoruntulenmeDegisimi[0];
        haftalikGoruntulenme.Pazartesi = gunlukGoruntulenmeDegisimi[1];
        haftalikGoruntulenme.Sali = gunlukGoruntulenmeDegisimi[2];
        haftalikGoruntulenme.Carsamba = gunlukGoruntulenmeDegisimi[3];
        haftalikGoruntulenme.Persembe = gunlukGoruntulenmeDegisimi[4];
        haftalikGoruntulenme.Cuma = gunlukGoruntulenmeDegisimi[5];
        haftalikGoruntulenme.Cumartesi = gunlukGoruntulenmeDegisimi[6];
        haftalikGoruntulenme.Toplam = gunlukGoruntulenmeDegisimi.Values.Sum();
        _context.HaftalikGoruntulenmeler.Update(haftalikGoruntulenme);

        // Haftalık arama verilerini güncelle (Bu veriler direkt HaftalikAramalar tablosundan alınacak)
        // userWeeklySearch zaten güncellenmiş durumda (UpdateTelefonAramaSayisi metodu tarafından) veya sıfırlanmış durumda.
        _context.HaftalikAramalar.Update(userWeeklySearch);


        // Değişiklikleri veritabanına kaydet
        await _context.SaveChangesAsync();

        // İlan durumlarına göre sayım (sadece "active" durumdaki ilanlar)
        var konutDurumlari = new Dictionary<string, int>
        {
            { "Satılık", ilanlar.Count(i => i.KonutDurumu == "Satılık") },
            { "Kiralık", ilanlar.Count(i => i.KonutDurumu == "Kiralık") },
            { "Devren Satılık", ilanlar.Count(i => i.KonutDurumu == "Devren Satılık") },
            { "Devren Kiralık", ilanlar.Count(i => i.KonutDurumu == "Devren Kiralık") }
        };

        // Kategorilere göre ilan sayımları
        var kategoriAdlari = new List<string>
        {
            "Konut (Yaşam Alanı)", "İş Yeri", "Turistik Tesis", "Arsa", "Bahçe", "Tarla"
        };
        var kategoriVerileri = kategoriAdlari
            .Select(kategori => ilanlar.Count(i => i.Category == kategori))
            .ToList();

        // Kullanıcının şehrini al
        string userCity = user?.City;
        int talepCount = 0;
        // Kullanıcının şehri varsa, satış taleplerini kontrol et
        if (!string.IsNullOrEmpty(userCity))
        {
            talepCount = await _context.SatisTalepleri.AsNoTracking()
                .CountAsync(st => st.ResidentialCity == userCity || st.LandCity == userCity);
        }

        // Nihai veriyi DTO'ya aktar
        var tumIlanlarDTO = new TumIlanlarDTO
        {
            WeeklyFavorites = new List<int> // haftalikBegeniler'den doldur
            {
                haftalikBegeniler.Pazar, haftalikBegeniler.Pazartesi, haftalikBegeniler.Sali,
                haftalikBegeniler.Carsamba, haftalikBegeniler.Persembe, haftalikBegeniler.Cuma,
                haftalikBegeniler.Cumartesi
            },
            ToplamFavori = haftalikBegeniler.Toplam,
            WeeklySearches = new List<int> // userWeeklySearch'ten doldur
            {
                userWeeklySearch.Pazartesi, userWeeklySearch.Sali, userWeeklySearch.Carsamba, // Pazartesi ile başlat
                userWeeklySearch.Persembe, userWeeklySearch.Cuma, userWeeklySearch.Cumartesi, userWeeklySearch.Pazar
            },
            WeeklyViews = new List<int> // haftalikGoruntulenme'den doldur
            {
                haftalikGoruntulenme.Pazar, haftalikGoruntulenme.Pazartesi, haftalikGoruntulenme.Sali,
                haftalikGoruntulenme.Carsamba, haftalikGoruntulenme.Persembe, haftalikGoruntulenme.Cuma,
                haftalikGoruntulenme.Cumartesi
            },
            WeekDays = new List<string> { "Pzt", "Salı", "Çrş", "Prş", "Cuma", "Cmt", "Pzr" }, // Pazartesi ile başlat
            ToplamGoruntulenme = haftalikGoruntulenme.Toplam,
            ToplamArama = userWeeklySearch.Toplam,
            EnCokGoruntulenenIlanlar = ilanlar.OrderByDescending(i => i.GoruntulenmeSayisi).Take(3).ToList(),
            IlanSayisi = ilanlar.Count,
            KonutDurumlari = konutDurumlari,
            // WeeklySearchData ve WeekDaysLabels doğrudan HaftalikAramalar'dan gelen verilerle eşleşecek
            WeeklySearchData = new List<int> {
                userWeeklySearch.Pazartesi, userWeeklySearch.Sali, userWeeklySearch.Carsamba,
                userWeeklySearch.Persembe, userWeeklySearch.Cuma, userWeeklySearch.Cumartesi, userWeeklySearch.Pazar
            },
            WeekDaysLabels = new List<string> { "Pzt", "Salı", "Çrş", "Prş", "Cuma", "Cmt", "Pzr" },
            KategoriAdlari = kategoriAdlari,
            KategoriVerileri = kategoriVerileri,
            TalepCount = talepCount
        };

        return View(tumIlanlarDTO);
    }


    public async Task<IActionResult> VerifyUser(string userId, string verificationCode)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        var userVerification = new UserVerification
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            VerificationCode = verificationCode,
            VerificationDate = DateTime.Now,
            VerificationExpiryDate = DateTime.Now.AddMinutes(2), // Doğrulama kodunun geçerlilik süresi
            IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
        };
        _context.UserVerifications.Add(userVerification);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult DetayliArama()
    {
        // Detaylı arama sayfasına yönlendir
        return View();
    }

    [HttpGet]
    public IActionResult YayındaOlmayan()
    {
        // Yayında olmayan ilanlar sayfasına yönlendir
        return View();
    }

    // Görüntülenme sayısını artırma
    [HttpPost]
    public IActionResult IncrementViewCount(int ilanId)
    {
        var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == ilanId);
        if (ilan != null)
        {
            ilan.GoruntulenmeSayisi += 1;
            _context.SaveChanges();
        }
        return Json(new { success = true });
    }

    public IActionResult Mesajlar()
    {
        var userEmail = User.Identity.Name;
        // Kullanıcının dahil olduğu sohbetleri grupla
        var chats = _context.Messages.AsNoTracking()
            .Where(m => m.SenderEmail == userEmail || m.ReceiverEmail == userEmail)
            .GroupBy(m => m.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessage = g.OrderByDescending(m => m.SentDate).FirstOrDefault().Content,
                LastMessageTime = g.OrderByDescending(m => m.SentDate).FirstOrDefault().SentDate,
                SenderEmail = g.FirstOrDefault().SenderEmail,
                ReceiverEmail = g.FirstOrDefault().ReceiverEmail,
                SenderFullName = g.FirstOrDefault().SenderFullName,
                ReceiverFullName = g.FirstOrDefault().ReceiverFullName,
                HasUnread = g.Any(m => m.ReceiverEmail == userEmail && !m.IsRead),
                IlanId = g.OrderByDescending(m => m.SentDate).FirstOrDefault().IlanId  // IlanId'yi al
            })
            .ToList();

        ViewBag.Chats = chats;
        return View();
    }

    public JsonResult GetMessages(int chatId)
    {
        var userEmail = User.Identity.Name;
        var messages = _context.Messages.AsNoTracking()
            .Where(m => m.ChatId == chatId && (m.SenderEmail == userEmail || m.ReceiverEmail == userEmail))
            .OrderBy(m => m.SentDate)
            .Select(m => new
            {
                content = m.Content,
                sentDate = m.SentDate,
                senderEmail = m.SenderEmail,
                senderFullName = m.SenderFullName,
                isRead = m.IsRead,
                ilanId = m.IlanId
            })
            .ToList();

        var unreadMessages = _context.Messages.Where(m => m.ChatId == chatId && m.ReceiverEmail == userEmail && !m.IsRead).ToList();
        if (unreadMessages.Any())
        {
            unreadMessages.ForEach(m => m.IsRead = true);
            _context.SaveChanges();
        }
        return Json(messages);
    }

    [HttpPost]
    public IActionResult SendMessage(int chatId, string content, string receiverEmail, int? ilanId) // IlanId parametresi eklendi
    {
        var receiver = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == receiverEmail);
        var sender = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

        if (receiver == null)
        {
            return BadRequest("Alıcı bulunamadı.");
        }
        if (sender == null)
        {
            return BadRequest("Gönderen bulunamadı.");
        }

        var receiverFullName = receiver.FirstName + " " + receiver.LastName;
        var senderFullName = sender.FirstName + " " + sender.LastName;

        var message = new Message
        {
            ChatId = chatId,
            Content = content,
            SenderEmail = User.Identity.Name,
            ReceiverEmail = receiverEmail,
            ReceiverFullName = receiverFullName,
            SenderFullName = senderFullName,
            SentDate = DateTime.Now,
            IsRead = false,
            IlanId = ilanId  // IlanId atandı
        };

        _context.Messages.Add(message);
        _context.SaveChanges();

        return Ok(new { success = true });
    }

    [HttpPost]
    public IActionResult MarkChatAsRead(int chatId)
    {
        var userEmail = User.Identity.Name;
        var unreadMessages = _context.Messages.Where(m => m.ChatId == chatId && m.ReceiverEmail == userEmail && !m.IsRead);
        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }
        try
        {
            _context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult MarkAllChatsAsRead()
    {
        var userEmail = User.Identity.Name;
        var unreadMessages = _context.Messages.Where(m => m.ReceiverEmail == userEmail && !m.IsRead);
        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }
        try
        {
            _context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    public JsonResult GetChatList()
    {
        var userEmail = User.Identity.Name;
        var chats = _context.Messages.AsNoTracking()
            .Where(m => m.SenderEmail == userEmail || m.ReceiverEmail == userEmail)
            .GroupBy(m => m.ChatId)
            .Select(g => new
            {
                chatId = g.Key,
                lastMessage = g.OrderByDescending(m => m.SentDate).FirstOrDefault().Content,
                lastMessageTime = g.OrderByDescending(m => m.SentDate).FirstOrDefault().SentDate,
                senderEmail = g.FirstOrDefault().SenderEmail,
                receiverEmail = g.FirstOrDefault().ReceiverEmail,
                senderFullName = g.FirstOrDefault().SenderFullName,
                receiverFullName = g.FirstOrDefault().ReceiverFullName,
                hasUnread = g.Any(m => m.ReceiverEmail == userEmail && !m.IsRead),
                ilanId = g.OrderByDescending(m => m.SentDate).FirstOrDefault().IlanId // IlanId eklendi
            })
            .ToList();
        return Json(chats);
    }

    public JsonResult GetIlanDetails(int ilanId)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().Where(i => i.Id == ilanId).Select(i => new
        {
            baslik = i.Title,
            aciklama = i.Description,
            fiyat = i.Price,
            tarih = i.GirisTarihi
        }).FirstOrDefault();
        return Json(ilan);
    }

    public async Task<IActionResult> Danismanlar()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(); // Veya RedirectToLogin()
        }

        // Kullanıcıları ve aktif ilan sayılarını verimli bir şekilde getir
        var users = await _context.Users.AsNoTracking()
            .Where(x => x.IsConsultant || x.Id == user.Id) // Danışman veya oturum açmış kullanıcı
            .Where(x => x.IsActive == "active") // Sadece aktif kullanıcılar
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.PhoneNumber,
                u.IsConsultant,
                u.ProfilePicturePath,
                IlanSayisi = _context.IlanBilgileri.Count(i => i.Email == u.Email && i.Status == "active") // Aktif ilanları say
            })
            .ToListAsync();

        // Şirket sahibi tarafından davet edilen danışmanların e-postalarını al
        var consultantInvitations = await _context.ConsultantInvitations.AsNoTracking()
            .Where(x => x.Status == "Completed" && x.InvitedBy == user.Id)
            .Select(ci => ci.Email)
            .ToListAsync();

        // ApplicationUser'a filtrele ve eşle
        var danismanlar = users
            .Where(u => consultantInvitations.Contains(u.Email) || u.Id == user.Id) // Şirket sahibini de dahil et
            .Select(u => new ApplicationUser
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsConsultant = u.IsConsultant,
                ProfilePicturePath = u.ProfilePicturePath,
                IlanSayisi = u.IlanSayisi // Zaten hesaplanmış sayıyı kullan
            })
            .ToList();

        return View(danismanlar);
    }

    [HttpPost]
    public JsonResult CheckUserExists(string? email, string? phone)
    {
        bool emailExists = false;
        bool phoneExists = false;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.ToUpper();
            emailExists = _context.Users.AsNoTracking().Any(u => u.NormalizedEmail == normalizedEmail);
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            phoneExists = _context.Users.AsNoTracking().Any(u => u.PhoneNumber == phone);
        }

        return Json(new { emailExists, phoneExists });
    }

    [HttpPost]
    public async Task<IActionResult> Sil(string id)
    {
        // Veritabanında kullanıcıyı bul
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Kullanıcıya ait ilanları getir
        var userIlanlar = _context.IlanBilgileri.Where(i => i.Email == user.Email).ToList();

        // İlan durumunu "archive" olarak ayarla
        foreach (var ilan in userIlanlar)
        {
            ilan.Status = "archive";
        }

        // Kullanıcının IsActive durumunu "Inactive" olarak ayarla
        user.IsActive = "Inactive"; // User tablosunda IsActive sütunu olmalı

        // Değişiklikleri kaydet
        await _context.SaveChangesAsync();

        // Danışmanlar sayfasına yönlendir
        return RedirectToAction("Danismanlar");
    }

    public ActionResult Sozlesmeler()
    {
        return View(); // sozlesmeler.cshtml sayfasını döndürür
    }

    // Akademi sayfasına yönlendir
    public IActionResult Akademi()
    {
        return View(); // Akademi.cshtml sayfasını render eder
    }

    public ActionResult ŞevvalHat()
    {
        // ŞevvalHat.cshtml sayfasına yönlendir
        return View();
    }

    public ActionResult Hizmetler()
    {
        return View(); // Hizmetler.cshtml sayfasını döndürür
    }

    // İlan listesini alma metodu
    public IActionResult Ilanlar(string filter = "active", string userEmail = null, string searchTerm = "")
    {
        var userEmailFromSession = User.Identity.Name;
        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == userEmailFromSession);

        if (user == null)
        {
            return NotFound();
        }

        // Şirket sahibi tarafından davet edilen danışmanların e-postalarını al
        var allowedEmails = _context.ConsultantInvitations.AsNoTracking()
            .Where(ci => ci.CompanyName == user.CompanyName && ci.Status == "Completed")
            .Select(ci => ci.Email)
            .Distinct()
            .ToList();

        IQueryable<IlanModel> ilanlarQuery;

        if (userEmail == "Tum")
        {
            // "Tüm Danışmanlar" seçeneği
            ilanlarQuery = _context.IlanBilgileri.AsNoTracking().Where(i => allowedEmails.Contains(i.Email));
        }
        else if (userEmail != null) // Eğer userEmail null değilse
        {
            // Belirli bir danışmanın ilanları
            ilanlarQuery = _context.IlanBilgileri.AsNoTracking().Where(i => i.Email == userEmail);
        }
        else
        {
            // Eğer userEmail null ise, oturumdaki kullanıcıya göre filtrele
            ilanlarQuery = _context.IlanBilgileri.AsNoTracking().Where(i => i.Email == userEmailFromSession);
        }

        // Duruma göre filtrele
        if (filter != "all")
        {
            ilanlarQuery = ilanlarQuery.Where(i => i.Status == filter);
        }

        // Arama terimine göre filtrele
        if (!string.IsNullOrEmpty(searchTerm))
        {
            ilanlarQuery = ilanlarQuery.Where(i => i.Title.Contains(searchTerm));
        }

        var ilanlar = ilanlarQuery.OrderByDescending(i => i.Id).ToList();
        var ilanIds = ilanlar.Select(i => i.Id).ToList();
        var fotos = _context.Photos.AsNoTracking().Where(p => ilanIds.Contains(p.IlanId)).ToList();

        // Şirket sahibi tarafından davet edilen danışmanları getir (sadece aktif olanlar)
        var users = _context.ConsultantInvitations.AsNoTracking()
                .Where(ci => ci.CompanyName == user.CompanyName && ci.Status == "Completed")
                .Select(ci => new
                {
                    ci.Email,
                    ci.FirstName,
                    ci.LastName
                })
                .Distinct()
                .ToList()
                .Select(ci => new ApplicationUser
                {
                    Email = ci.Email,
                    FirstName = ci.FirstName,
                    LastName = ci.LastName,
                    PhoneNumber = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == ci.Email)?.PhoneNumber,
                    IsActive = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == ci.Email)?.IsActive
                })
                .Where(u => u.IsActive == "active")
                .ToList();

        // İlk yüklemede, şirket sahibini de danışmanlar listesine ekle
        if (userEmail == null && !users.Any(u => u.Email == user.Email))
        {
            users.Insert(0, new ApplicationUser
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber
            });
        }

        var model = new TumIlanlarDTO
        {
            _Ilanlar = ilanlar,
            _Fotograflar = fotos,
            Filter = filter,
            Users = users,
            SelectedUserEmail = userEmail ?? userEmailFromSession, // Seçili danışmanı koru
            SearchTerm = searchTerm,
            TotalIlanCount = ilanlar.Count // Zaten filtrelendiği için sayım doğrudan
        };
        return View(model);
    }

    // AJAX tarafından çağrılan metot, ilanı seçilen danışmana ata
    [HttpPost]
    public IActionResult AssignToConsultant(int ilanId, string consultantEmail)
    {
        var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == ilanId);
        if (ilan == null)
        {
            return Json(new { success = false, message = "İlan bulunamadı." });
        }

        var consultant = _context.Users.FirstOrDefault(u => u.Email == consultantEmail);
        if (consultant == null)
        {
            return Json(new { success = false, message = "Danışman bulunamadı." });
        }

        // İlan bilgilerini seçilen danışmanın bilgileriyle güncelle
        ilan.Email = consultant.Email;
        ilan.FirstName = consultant.FirstName;
        ilan.LastName = consultant.LastName;
        ilan.PhoneNumber = consultant.PhoneNumber;
        _context.SaveChanges();

        return Json(new { success = true, message = "İlan başarıyla danışmana atandı." });
    }

    public async Task<IActionResult> Archive()
    {
        var userEmail = User.Identity.Name;
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            return NotFound();
        }

        // Kullanıcının arşivlenmiş ilanlarını getir
        var ilanlar = await _context.IlanBilgileri.AsNoTracking()
            .Where(i => i.Email == userEmail && i.Status == "archive")
            .OrderBy(i => i.LastActionDate) // Son işlem tarihine göre sırala
            .ToListAsync();

        if (!ilanlar.Any())
        {
            return View(new TumIlanlarDTO
            {
                GroupedIlanlar = new Dictionary<int, Dictionary<int, Dictionary<int, List<IlanModel>>>>(),
                TotalArchiveCount = 0 // Boş model için toplam 0
            });
        }

        // Arşivlenmiş ilan sayısını hesapla
        int totalArchiveCount = ilanlar.Count;

        // DTO oluştur
        var groupedIlanlar = ilanlar
            .Where(i => i.LastActionDate.HasValue) // Nullable kontrolü
            .GroupBy(i => i.LastActionDate.Value.Year)
            .ToDictionary(
                yearGroup => yearGroup.Key,
                yearGroup => yearGroup
                    .GroupBy(i => i.LastActionDate.Value.Month)
                    .ToDictionary(
                        monthGroup => monthGroup.Key,
                        monthGroup => monthGroup
                            .GroupBy(i => i.LastActionDate.Value.Day)
                            .ToDictionary(
                                dayGroup => dayGroup.Key,
                                dayGroup => dayGroup.ToList()
                            )
                    )
            );

        var model = new TumIlanlarDTO
        {
            GroupedIlanlar = groupedIlanlar,
            TotalArchiveCount = totalArchiveCount // Toplam ilan sayısını modele dahil et
        };

        return View(model);
    }

    public async Task<IActionResult> UyeKontrol(string filter = "All")
    {
        var users = await _context.Users.AsNoTracking().ToListAsync(); // Tüm kullanıcıları getir
        var ilanlar = await _context.IlanBilgileri.AsNoTracking().ToListAsync(); // Tüm ilanları getir
        var model = new TumIlanlarDTO { Users = users, _Ilanlar = ilanlar };
        ViewBag.Filter = filter; // Seçilen filtreyi ViewBag ile gönder
        return View(model);
    }

    // Firma Sahipleri sayfası
    public IActionResult FirmaSahipleri()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdatePrice([FromBody] UpdatePriceDTO dto)
    {
        try
        {
            if (dto == null || dto.IlanId <= 0 || dto.NewPrice <= 0)
            {
                return BadRequest("Geçersiz veri.");
            }

            var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == dto.IlanId);
            if (ilan == null)
            {
                return NotFound("İlan bulunamadı.");
            }

            ilan.Price = dto.NewPrice;
            ilan.LastActionDate = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, newPrice = ilan.Price });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Sunucu hatası: {ex.Message}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete([FromBody] DeleteRequest request)
    {
        try
        {
            var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == request.IlanId);
            if (ilan == null)
            {
                return NotFound("İlan bulunamadı.");
            }

            var fotograflar = _context.Photos.Where(p => p.IlanId == ilan.Id).ToList();
            foreach (var foto in fotograflar)
            {
                // Fotoğraf dosya sistemi işlemi
                // File.Delete(foto.FilePath);
            }

            ilan.Status = "deleted"; // İlanı "deleted" olarak işaretle
            ilan.LastActionDate = DateTime.Now;
            _context.IlanBilgileri.Update(ilan);
            _context.SaveChanges();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"İlan silinirken bir hata oluştu: {ex.Message}");
        }
    }

    // DeleteRequest modelini tanımla
    public class DeleteRequest
    {
        public int IlanId { get; set; }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Archive([FromBody] DeleteRequest request)
    {
        try
        {
            var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == request.IlanId);
            if (ilan == null)
            {
                return NotFound("İlan bulunamadı.");
            }

            ilan.Status = "archive"; // İlanı "arşivlenmiş" olarak işaretle
            ilan.LastActionDate = DateTime.Now;
            _context.IlanBilgileri.Update(ilan);
            _context.SaveChanges();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"İlan arşivlenirken bir hata oluştu: {ex.Message}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFromPublished([FromBody] DeleteRequest request)
    {
        try
        {
            // Gelen ilanId'ye göre ilanı bul
            var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == request.IlanId);
            if (ilan == null)
            {
                return NotFound("İlan bulunamadı.");
            }

            // İlan durumunu "inactive" olarak ayarla
            ilan.Status = "inactive";
            ilan.LastActionDate = DateTime.Now;
            _context.IlanBilgileri.Update(ilan);
            _context.SaveChanges();

            return Ok();
        }
        catch (Exception ex)
        {
            // Daha anlamlı bir hata mesajı döndür
            return StatusCode(500, $"İlan yayından kaldırılırken bir hata oluştu: {ex.Message}. Detaylar: {ex.StackTrace}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PromoteToActive([FromBody] DeleteRequest request)
    {
        try
        {
            // Gelen ilanId'ye göre ilanı bul
            var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == request.IlanId);
            if (ilan == null)
            {
                return NotFound("İlan bulunamadı.");
            }

            // İlan durumunu "active" olarak ayarla
            ilan.Status = "active";
            ilan.LastActionDate = DateTime.Now;
            _context.IlanBilgileri.Update(ilan);
            _context.SaveChanges();

            return Ok();
        }
        catch (Exception ex)
        {
            // Daha anlamlı bir hata mesajı döndür
            return StatusCode(500, $"İlan yayınlanırken bir hata oluştu: {ex.Message}. Detaylar: {ex.StackTrace}");
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        // İlgili ilanı getir
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
        {
            return NotFound("İlan bulunamadı.");
        }

        // Fotoğrafları ve diğer bilgileri ekle
        var photos = _context.Photos.AsNoTracking().Where(p => p.IlanId == id).ToList();
        var videos = _context.Videos.AsNoTracking().Where(p => p.IlanId == id).ToList();

        // Bilgileri TumIlanlarDTO ile model olarak aktar
        var model = new TumIlanlarDTO
        {
            _Ilanlar = new List<IlanModel> { ilan },
            _Fotograflar = photos,
            SelectedCategory = ilan.Category,
            SelectedDurum = ilan.KonutDurumu,
            SelectedSubcategory = ilan.MulkTipi,
        };

        // Kategoriye göre düzenleme sayfasına yönlendir
        var redirectAction = ilan.Category switch
        {
            "Konut (Yaşam Alanı)" => nameof(KonutIlanEdit),
            "Arsa" => nameof(AraziIlanEdit),
            "Bağ" => nameof(BagIlanEdit),
            "Bahçe" => nameof(BahceIlanEdit),
            "Tarla" => nameof(TarlaIlanEdit),
            "İş Yeri" => nameof(IsYeriIlanEdit),
            "Turistik Tesis" => nameof(TuristikTesisIlanEdit),
            _ => null,
        };

        IlanDuzenleDTO ılanDuzenleDTO = new IlanDuzenleDTO
        {
            Ilan = ilan,
            _Fotograflar = photos,
            _Videolar = videos,
        };

        if (redirectAction != null)
        {
            ViewBag.Model = ılanDuzenleDTO; // Modeli ViewBag ile taşı
            return View(ılanDuzenleDTO);
        }

        return NotFound();
    }




    public string GetRedirectUrl(ApplicationUser user)
    {
        if (user.IsConsultant)
        {
            // Danışmansa, ConsultantInvitations tablosundaki InvitedBy değerine göre yönlendir
            var consultantInvitation = _context.ConsultantInvitations.AsNoTracking()
                                               .FirstOrDefault(ci => ci.Email == user.Email);
            if (consultantInvitation != null && !string.IsNullOrEmpty(consultantInvitation.InvitedBy))
            {
                return Url.Action("Details", "Ilan", new { id = consultantInvitation.InvitedBy });
            }
        }
        else
        {
            // Şirket sahibiyse, kendi id'sine göre yönlendir
            return Url.Action("Details", "Ilan", new { id = user.Id });
        }
        // Varsayılan yönlendirme, hata oluşursa ana sayfaya dön
        return Url.Action("Index", "Home");
    }

    public IActionResult RedirectToStore(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return NotFound(); // E-posta boşsa hata döndür
        }

        // E-posta ile kullanıcıyı bul
        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return NotFound(); // Kullanıcı bulunamazsa hata döndür
        }

        if (user.IsConsultant == false) // Şirket sahibi ise
        {
            // Şirket sahibinin Detaylar sayfasına yönlendir
            return RedirectToAction("Details", "Ilan", new { id = user.Id });
        }
        else // Danışman ise
        {
            // Danışmanı ConsultantInvitations tablosundaki InvitedBy'a göre yönlendir
            var consultantInvitation = _context.ConsultantInvitations.AsNoTracking()
                .FirstOrDefault(ci => ci.Email == user.Email);

            if (consultantInvitation != null)
            {
                // Danışman için InvitedBy'a göre yönlendirme
                return RedirectToAction("Details", "Ilan", new { id = consultantInvitation.InvitedBy });
            }
            else
            {
                // Danışman kaydı bulunamazsa hata mesajı
                return NotFound();
            }
        }
    }

    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; } // Kullanıcı bilgileri
        public List<IlanModel> Ilanlar { get; set; } // Kullanıcının ilanları
    }

    // İlan oluşturma sayfası için GET metodu
    [HttpGet]
    public IActionResult IlanVer()
    {
        var model = new IlanModel(); // Yeni bir model oluştur
        return View(model); // Modeli view'e geçir
    }

    // İlan vitrin sayfası için GET metodu
    public IActionResult Vitrin()
    {
        return View();
    }

    // Önizleme Aksiyon Metodu
    public IActionResult Onizleme(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
        {
            return NotFound();
        }
        var fotograf = _context.Photos.AsNoTracking().Where(f => f.IlanId == id).ToList();
        var model = new TumIlanlarDTO
        {
            _Ilanlar = new List<IlanModel> { ilan },
            _Fotograflar = fotograf,
        };
        return View("Onizleme", ilan);
    }

    // Slug oluşturma yardımcı metodu
    private string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title)) return "";
        // Küçük harfe çevir
        string slug = title.ToLowerInvariant();
        // Türkçe karakterleri İngilizce karşılıklarına çevir
        slug = slug.Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");
        // Geçersiz karakterleri kaldır ve boşlukları tire ile değiştir
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // Sadece harf, rakam, boşluk ve tire bırak
        slug = Regex.Replace(slug, @"\s+", "-").Trim(); // Birden fazla boşluğu tek tireye çevir
        slug = Regex.Replace(slug, @"-+", "-"); // Birden fazla tireyi tek tireye çevir
        return slug;
    }

    // Mevcut ilansayfasi metodu (eski URL'ler için hala çalışır - geriye dönük uyumluluk)
    [HttpGet("/Ilan/ilansayfasi/{id:int}")]
    // Yeni SEO dostu rota (ana rota bu olacak)
    // Örn: /ilan/remax-dogudan-hazar-golunde-satilik-villa-19269
    [HttpGet("ilan/{slug}-{id:int}", Name = "IlanDetaySeo")]
    public async Task<IActionResult> ilansayfasi(int id, string slug)
    {
        var userEmail = User?.Identity?.Name;

        var allowedEmails = new List<string>
            {
                "sftumen41@gmail.com", "exdel.txt@gmail.com",
                "burak.tumen@hotmail.com", "fahritumen01@gmail.com",
                "ilkaykoyun4167@gmail.com", "eren.acar.08@gmail.com"
            };

        // İlan bilgilerini, fotoğraflarını ve kullanıcısını tek bir sorguda getir
        var ilanData = await _context.IlanBilgileri.AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new
            {
                Ilan = i,
                Photos = _context.Photos.AsNoTracking().Where(f => f.IlanId == i.Id).ToList(),
                Videos = _context.Videos.AsNoTracking().Where(v => v.IlanId == i.Id).ToList(),
                User = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == i.Email),
                Company = _context.ConsultantInvitations.AsNoTracking().FirstOrDefault(ci => ci.Email == i.Email)
            })
            .FirstOrDefaultAsync();

        if (ilanData == null || (ilanData.Ilan.Status != "active" &&
            (!allowedEmails.Contains(userEmail) && userEmail != ilanData.Ilan.Email)))
        {
            var semt = ilanData?.Ilan?.semt;
            var semtIlanlari = await _context.IlanBilgileri.AsNoTracking()
                .Where(i => i.Status == "active" && i.semt == semt)
                .Select(i => new IlanModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    semt = i.semt,
                    sehir = i.sehir,
                    Price = i.Price,
                    Area = i.Area
                }).ToListAsync();
            return View("Hata", new TumIlanlarDTO { _Ilanlar = semtIlanlari });
        }

        // Eğer gelen slug boşsa veya ilan başlığı ile eşleşmiyorsa, doğru slug'a kalıcı olarak yönlendir
        string expectedSlug = GenerateSlug(ilanData.Ilan.Title);

        if (string.IsNullOrEmpty(slug) || slug != expectedSlug)
        {
            // Yeni SEO dostu URL'ye kalıcı olarak yönlendir (301 Moved Permanently)
            // Bu, arama motorlarına URL'nin kalıcı olarak taşındığını bildirir.
            return RedirectToRoutePermanent("IlanDetaySeo", new { id = id, slug = expectedSlug });
        }

        // Görüntülenme sayısı kontrolü
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var sessionKey = $"{ipAddress}_{id}_{DateTime.Today:yyyy-MM-dd}";

       // if (!string.IsNullOrEmpty(ipAddress) && HttpContext.Session.GetString(sessionKey) == null)
        {
            var ilanToUpdate = await _context.IlanBilgileri.FindAsync(id);
            if (ilanToUpdate != null)
            {
                ilanToUpdate.GoruntulenmeSayisi++;
                ilanToUpdate.GoruntulenmeTarihi = DateTime.Now;
                _context.IlanBilgileri.Update(ilanToUpdate);
                await _context.SaveChangesAsync();
            }

            // Detay sayfasında gösterilecek toplam görüntülenmeyi (ilan + günün ilanı) hesapla
            try
            {
                var today = DateTime.Today;
                // Önce bugüne ait featured kayıt var mı?
                var featured = await _context.GununIlanlari
                    .AsNoTracking()
                    .Where(g => g.Id == id && g.YayinlanmaTarihi.Date == today)
                    .OrderByDescending(g => g.YayinlanmaTarihi)
                    .FirstOrDefaultAsync();

                // Yoksa bu ilana ait en son featured kaydına düş
                if (featured == null)
                {
                    featured = await _context.GununIlanlari
                        .AsNoTracking()
                        .Where(g => g.Id == id)
                        .OrderByDescending(g => g.YayinlanmaTarihi)
                        .FirstOrDefaultAsync();
                }

                if (featured != null)
                {
                    var baseViews = ilanData.Ilan.GoruntulenmeSayisi;
                    ilanData.Ilan.GoruntulenmeSayisi = (baseViews) + (featured.GoruntulenmeSayisi);
                }
            }
            catch { }

           // HttpContext.Session.SetString(sessionKey, "visited");
        }

        var otherIlanlar = await _context.IlanBilgileri.AsNoTracking()
            .Where(i => i.Email == ilanData.Ilan.Email && i.Status == "active" && i.Id != id)
            .Select(i => new IlanModel
            {
                Id = i.Id,
                Title = i.Title,
                Price = i.Price,
                Area = i.Area,
                semt = i.semt,
                sehir = i.sehir,
                
            })
            .ToListAsync();

        var randomIlanlar = otherIlanlar
            .OrderBy(x => Guid.NewGuid())
            .Take(5)
            .ToList();

        string companyName = null;
        int companyTotalIlanCount = 0;
        string companyOwnerId = null;
        string companyOwnerProfilePicturePath = null;

        if (ilanData.User != null && !ilanData.User.IsConsultant) // Şirket sahibi
        {
            companyName = ilanData.User.CompanyName;

            companyOwnerId = ilanData.User.Id;

            var companyOwner = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == companyOwnerId);

            if (companyOwner != null)
            {
                companyOwnerProfilePicturePath = companyOwner.ProfilePicturePath;
            }

            var consultantEmails = await _context.ConsultantInvitations.AsNoTracking()
                .Where(ci => ci.CompanyName == companyName)
                .Select(ci => ci.Email)
                .ToListAsync();

            consultantEmails.Add(ilanData.User.Email);
            companyTotalIlanCount = await _context.IlanBilgileri.AsNoTracking()
                .CountAsync(i => consultantEmails.Contains(i.Email) && i.Status == "active");
        }
        else if (ilanData.Company != null) // Danışman
        {
            var companyOwner = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == ilanData.Company.InvitedBy);

            if (companyOwner != null)
            {
                companyName = companyOwner.CompanyName;
                companyOwnerId = companyOwner.Id;
                companyOwnerProfilePicturePath = companyOwner.ProfilePicturePath;

                var consultantEmails = await _context.ConsultantInvitations.AsNoTracking()
                    .Where(ci => ci.CompanyName == companyName)
                    .Select(ci => ci.Email)
                    .ToListAsync();

                consultantEmails.Add(companyOwner.Email);

                companyTotalIlanCount = await _context.IlanBilgileri.AsNoTracking()
                    .CountAsync(i => consultantEmails.Contains(i.Email) && i.Status == "active");
            }
        }

        // Oturuma bu ilanın bugün ziyaret edildiğini işaretliyoruz
        var response = await _recentlyVisitedAnnouncementService.AddRecentlyVisitedAnnouncementAsync(new AddRecentlyVisitedAnnouncementCommandRequest
        {
            AnnouncementId = id,
            UserId = _userManager.GetUserId(User),
            Province = ilanData.Ilan.sehir,
            Property = ilanData.Ilan.Category

        }, CancellationToken.None);

        // DTO oluştur
        var model = new TumIlanlarDTO
        {
            _Ilanlar = new List<IlanModel> { ilanData.Ilan },
            _Fotograflar = ilanData.Photos,
            _Videolar = ilanData.Videos,
            User = ilanData.User,
            ProfilePicturePath = ilanData.User?.ProfilePicturePath,
            Yorumlar = await _context.Yorumlar.AsNoTracking()
                .Where(y => y.IlanId == id)
                .OrderByDescending(y => y.YorumTarihi)
                .ToListAsync(),
            RandomIlanlar = randomIlanlar,
            CompanyName = companyName,
            CompanyTotalIlanCount = companyTotalIlanCount,
            CompanyOwnerProfilePicturePath = companyOwnerProfilePicturePath,
            RegistrationDate = ilanData.User?.RegistrationDate
        };

        // TKGM linkini DTO'ya ekleyin
        if (!string.IsNullOrEmpty(ilanData.Ilan.TKGMParselLink))
        {
            model.TKGMParselLink = ilanData.Ilan.TKGMParselLink;
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTelefonAramaSayisi(int id)
    {
        // İlanı veritabanından al
        var ilan = await _context.IlanBilgileri.FirstOrDefaultAsync(i => i.Id == id);

        // İlan bulunamazsa 404 döndür
        if (ilan == null)
        {
            return NotFound();
        }

        // Telefon arama sayısını artır
        ilan.TelefonAramaSayisi += 1;

        // HaftalıkAramalar tablosunu kontrol et
        var userEmail = ilan.Email; // İlanın e-posta adresini al
        var haftalikArama = await _context.HaftalikAramalar.FirstOrDefaultAsync(h => h.UserEmail == userEmail);

        if (haftalikArama != null)
        {
            // Haftanın başlangıcını (Pazartesi) bul
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)firstDayOfWeek);

            // Eğer bugün haftanın ilk günü ise ve son sıfırlama bu haftadan önceyse, haftalık aramaları sıfırla
            if (DateTime.Today.DayOfWeek == firstDayOfWeek && haftalikArama.LastResetDate.Date < startOfWeek.Date)
            {
                haftalikArama.Pazar = 0;
                haftalikArama.Pazartesi = 0;
                haftalikArama.Sali = 0;
                haftalikArama.Carsamba = 0;
                haftalikArama.Persembe = 0;
                haftalikArama.Cuma = 0;
                haftalikArama.Cumartesi = 0;
                haftalikArama.Toplam = 0;
                haftalikArama.LastResetDate = DateTime.Now; // Sıfırlama tarihini güncelle
            }

            // Mevcut günü al ve ilgili günün arama sayısını artır
            var gun = DateTime.Now.DayOfWeek;
            switch (gun)
            {
                case DayOfWeek.Sunday:
                    haftalikArama.Pazar += 1;
                    break;
                case DayOfWeek.Monday:
                    haftalikArama.Pazartesi += 1;
                    break;
                case DayOfWeek.Tuesday:
                    haftalikArama.Sali += 1;
                    break;
                case DayOfWeek.Wednesday:
                    haftalikArama.Carsamba += 1;
                    break;
                case DayOfWeek.Thursday:
                    haftalikArama.Persembe += 1;
                    break;
                case DayOfWeek.Friday:
                    haftalikArama.Cuma += 1;
                    break;
                case DayOfWeek.Saturday:
                    haftalikArama.Cumartesi += 1;
                    break;
            }
            // Haftalık toplamı güncelle
            haftalikArama.Toplam += 1;

            // Haftalık arama kaydını güncelle
            _context.HaftalikAramalar.Update(haftalikArama);
        }
        else
        {
            // Haftalık arama kaydı yoksa yeni bir tane oluştur ve ekle
            haftalikArama = new HaftalikArama
            {
                UserEmail = userEmail,
                Pazar = (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) ? 1 : 0,
                Pazartesi = (DateTime.Now.DayOfWeek == DayOfWeek.Monday) ? 1 : 0,
                Sali = (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) ? 1 : 0,
                Carsamba = (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) ? 1 : 0,
                Persembe = (DateTime.Now.DayOfWeek == DayOfWeek.Thursday) ? 1 : 0,
                Cuma = (DateTime.Now.DayOfWeek == DayOfWeek.Friday) ? 1 : 0,
                Cumartesi = (DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ? 1 : 0,
                Toplam = 1,
                LastResetDate = DateTime.Now // Yeni oluşturulduğunda sıfırlama tarihini ayarla
            };
            _context.HaftalikAramalar.Add(haftalikArama);
        }

        // Telefon arama sayısı ve haftalık arama tablosu güncellendikten sonra değişiklikleri kaydet
        await _context.SaveChangesAsync();

        // Başarılı mesajı döndür
        return Json(new { success = true });
    }


    [HttpPost]
    public IActionResult YorumEkle(int ilanId, string yorum, string ad = null, string soyad = null)
    {
        // Kullanıcı bilgisini al
        ApplicationUser kullanici = null;
        if (User.Identity.IsAuthenticated)
        {
            // Kullanıcı giriş yapmışsa ilgili ApplicationUser'ı al
            kullanici = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
        }
        else
        {
            // Üye değilse, ad ve soyad bilgisini kullan
            kullanici = new ApplicationUser
            {
                FirstName = ad,
                LastName = soyad
            };
        }

        // Yorum modelini oluştur
        var yeniYorum = new YorumModel
        {
            Yorum = yorum,
            YorumTarihi = DateTime.Now,
            IlanId = ilanId,
            Kullanici = kullanici  // Kullanıcı bilgisi
        };

        // Yorumu veritabanına kaydet
        _context.Yorumlar.Add(yeniYorum);
        _context.SaveChanges();

        // Yorumlar eklendikten sonra ilan detay sayfasına geri dön
        return RedirectToAction("ilansayfasi", new { id = ilanId });
    }

    [HttpPost]
    public IActionResult addFavorite(int id)
    {
        var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == id);

        if (ilan == null)
        {
            return Json(new { success = false, message = "İlan bulunamadı" });
        }

        // Kullanıcının e-posta adresini al (örn. oturum açmış kullanıcıdan)
        var userEmail = User.Identity.Name; // Bu, oturum açmış kullanıcıya göredir, alternatif bir yöntem kullanılabilir

        // HaftalikBegeniler tablosundan kullanıcının mevcut haftalık favori verisini al
        var haftalikBegeniler = _context.HaftalikBegeniler
                                        .FirstOrDefault(h => h.UserEmail == userEmail);

        if (haftalikBegeniler == null)
        {
            // Kullanıcının haftalık favori kaydı yoksa yeni bir tane oluştur
            haftalikBegeniler = new HaftalikBegeniler
            {
                UserEmail = userEmail,
                Pazar = 0,
                Pazartesi = 0,
                Sali = 0,
                Carsamba = 0,
                Persembe = 0,
                Cuma = 0,
                Cumartesi = 0,
                Toplam = 0
            };
            _context.HaftalikBegeniler.Add(haftalikBegeniler);
        }

        // Haftanın mevcut gününü al
        var today = DateTime.Now.DayOfWeek;

        // Bugün Salı ise Salı'ya +1 ekle
        switch (today)
        {
            case DayOfWeek.Monday:
                haftalikBegeniler.Pazartesi += 1;
                break;
            case DayOfWeek.Tuesday:
                haftalikBegeniler.Sali += 1;
                break;
            case DayOfWeek.Wednesday:
                haftalikBegeniler.Carsamba += 1;
                break;
            case DayOfWeek.Thursday:
                haftalikBegeniler.Persembe += 1;
                break;
            case DayOfWeek.Friday:
                haftalikBegeniler.Cuma += 1;
                break;
            case DayOfWeek.Saturday:
                haftalikBegeniler.Cumartesi += 1;
                break;
            case DayOfWeek.Sunday:
                haftalikBegeniler.Pazar += 1;
                break;
        }

        // Toplam favoriyi güncelle
        haftalikBegeniler.Toplam += 1;

        // Favori sayısını artır
        ilan.FavoriSayisi += 1;

        // Değişiklikleri kaydet
        _context.SaveChanges();

        // Kullanıcının oturumda favori yaptığını kaydet
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        HttpContext.Session.SetString(ipAddress + id, "favori");

        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult Index()
    {
        // Kullanıcının IP adresini al
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Kullanıcının favorilerini al
        var favoritedIds = _context.IlanBilgileri.AsNoTracking()
            .Where(i => HttpContext.Session.GetString(ipAddress + i.Id) != null)
            .Select(i => i.Id)
            .ToList();

        ViewBag.FavoritedIds = favoritedIds;

        // İlan listesini döndür
        var ilanList = _context.IlanBilgileri.AsNoTracking().ToList();
        return View(ilanList);
    }

    // -----------------------------
    //  KATEGORİ SEÇİMİ
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> KategoriSecim(string yetkiKodu, string durum)
    {
        // Eğer yetkiKodu ve durum "Başarılı" ise, işlem başarılıdır.
        if (!string.IsNullOrEmpty(yetkiKodu) && durum == "Basarili")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Kullanıcı kodunu E-Devlet API'den çek (örnek)
                var eidsUserData = await GetUserCode2Async(yetkiKodu, user.PhoneNumber, user.VergiNo);
                if (eidsUserData != null && !string.IsNullOrEmpty(eidsUserData.KullaniciKodu))
                {
                    // Kullanıcının IP adresini al
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                    // Kullanıcının daha önce doğrulanıp doğrulanmadığını kontrol et
                    var existingVerification = await _context.UserVerifications.AsNoTracking()
                        .FirstOrDefaultAsync(v => v.UserId == user.Id);

                    if (existingVerification == null)
                    {
                        // Yeni doğrulama kaydı oluştur
                        var verification = new UserVerification
                        {
                            UserId = user.Id,
                            FirstName = eidsUserData.FirstName ?? user.FirstName,
                            LastName = eidsUserData.LastName ?? user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            VerificationCode = yetkiKodu,
                            UserCode = eidsUserData.KullaniciKodu,
                            VerificationDate = DateTime.UtcNow,
                            VerificationExpiryDate = DateTime.UtcNow.AddMonths(6), // 6 ay geçerli
                            IPAddress = ipAddress
                        };
                        _context.UserVerifications.Add(verification);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Mevcut doğrulama kaydını güncelle
                        existingVerification.VerificationCode = yetkiKodu;
                        existingVerification.VerificationDate = DateTime.UtcNow;
                        existingVerification.VerificationExpiryDate = DateTime.UtcNow.AddMonths(6);
                        if (!string.IsNullOrEmpty(eidsUserData.FirstName))
                            existingVerification.FirstName = eidsUserData.FirstName;
                        if (!string.IsNullOrEmpty(eidsUserData.LastName))
                            existingVerification.LastName = eidsUserData.LastName;
                        _context.UserVerifications.Update(existingVerification);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        // Kullanıcıyı ilan seçme sayfasına yönlendir
        return View();
    }

    // -----------------------------
    //  KULLANICI DOĞRULAMA KONTROLÜ
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> CheckUserVerification()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Json(new { isVerified = false });

        var verification = await _context.UserVerifications.AsNoTracking()
            .FirstOrDefaultAsync(uv => uv.UserId == user.Id);

        if (verification == null)
            return Json(new { isVerified = false });

        return Json(new
        {
            isVerified = true,
            name = verification.FirstName,
            surname = verification.LastName,
            verificationCode = verification.VerificationCode
        });
    }

    // -----------------------------
    //  EIDS SERVİS ÇAĞRISI
    // -----------------------------
    // Örneğin, EIDS servis çağırma metodunda _configuration kullanımı:
    private async Task<EidsUserResponse> GetKullaniciKoduFromEids(string yetkiKodu, string gsmNo, string vergiNo = null)
    {
        // appsettings.json'dan _configuration aracılığıyla değerleri al.
        var username = _configuration["EIDS:Username"];   // Örnek: EidsSvvlEmlkUser
        var password = _configuration["EIDS:Password"];     // Örnek: Xn9!4NycSt8HW
        var firmaKodu = _configuration["EIDS:FirmaKodu"];    // Örnek: 4208673F-31D1-420B-9B23-60C9BF3897A2

        // EIDS endpoint
        var endpoint = "https://ws.gtb.gov.tr:8443/EidsApi/Kullanici/GetKullaniciKodu";

        using (var client = new HttpClient())
        {
            // Basic Auth başlığı oluştur
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authToken);

            // İstek gövdesi
            var requestBody = new
            {
                yetkiKodu,
                gsmNo,
                vergiNo
            };
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            // POST isteği gönder
            var response = await client.PostAsync(endpoint, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                // Hatayı yakalayabilir veya loglayabilirsin
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            // Gelen JSON'ı modeline deserialize et.
            var eidsResult = JsonConvert.DeserializeObject<EidsUserResponse>(responseString);
            return eidsResult;
        }
    }

    public async Task<EidsUserResponse> GetUserCode2Async(string yetkiKodu, string gsmNo, string vergiNo = null)
    {
        var endpoint = "https://ws.gtb.gov.tr:8443/EidsApi/Kullanici/GetKullaniciKodu";
        var username = _configuration["EIDS:Username"];   // Örnek: EidsSvvlEmlkUser
        var password = _configuration["EIDS:Password"];     // Örnek: Xn9!4NycSt8HW
        var firmaKodu = _configuration["EIDS:FirmaKodu"];

        try
        {
            // Basic Auth başlığı ekle
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                // İstek gövdesini oluştur - tüm gerekli alanları dahil etmek için
                var requestBody = new
                {
                    yetkiKodu,
                    gsmNo,
                    vergiNo
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // API isteğini gönder
                var response = await client.PostAsync(endpoint, jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"EIDS API yanıtı: {responseContent}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Null kontrolü
                if (string.IsNullOrWhiteSpace(responseContent))
                    return null;

                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<EidsUserResponse>(responseContent, options);
                }
                catch (System.Text.Json.JsonException jex)
                {
                    Console.Error.WriteLine($"JSON ayrıştırma hatası: {jex.Message}, Yanıt: {responseContent}");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"GetUserCodeAsync hatası: {ex.Message}");
            return null;
        }
    }

    // -----------------------------
    //  YARDIMCI METOT: RedirectToLogin
    // -----------------------------
    private IActionResult RedirectToLogin()
    {
        // Kullanıcı yoksa, giriş sayfasına veya istediğiniz yere yönlendirebilirsiniz.
        return RedirectToAction("Login", "Account");
    }

    // -----------------------------
    //  EIDS YANIT MODELİ
    // -----------------------------
    public class EidsUserResponse
    {
        [JsonProperty("kullaniciKodu")]
        public string KullaniciKodu { get; set; }

        [JsonProperty("ad")]
        public string FirstName { get; set; }

        [JsonProperty("soyad")]
        public string LastName { get; set; }

        [JsonProperty("tckn")]
        public string TCKN { get; set; }

        [JsonProperty("vkn")]
        public string VKN { get; set; }
    }

    // Controllers/IlanController.cs
    [HttpGet]
    public async Task<IActionResult> CheckTasinmazAuthorization(string tasinmazNo)
    {
        // Kullanıcı kontrolü
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { isAuthorized = false, message = "Kullanıcı bulunamadı" });

        // Kullanıcı doğrulama bilgilerini kontrol et
        var verification = await _context.UserVerifications.AsNoTracking()
            .FirstOrDefaultAsync(uv => uv.UserId == user.Id);
        if (verification == null)
            return Json(new { isAuthorized = false, message = "Kullanıcı doğrulama bilgisi bulunamadı. Lütfen E-Devlet doğrulaması yapın." });

        // Taşınmaz numarasının ondalık sayıya dönüştürülüp dönüştürülemeyeceğini kontrol et
        if (!decimal.TryParse(tasinmazNo, out var tasinmazId))
            return Json(new { isAuthorized = false, message = "Lütfen geçerli bir taşınmaz numarası girin." });

        try
        {
            // Taşınmazın daha önce yetkilendirilip yetkilendirilmediğini kontrol et
            if (verification.AuthorizedPropertyNumbers != null &&
                verification.AuthorizedPropertyNumbers.Contains(tasinmazNo))
                return Json(new { isAuthorized = true, message = "Taşınmaz yetkilendirmesi onaylandı." });

            // EidsTasinmazAPI servisini çağır ve taşınmaz yetkilendirme kontrolünü yap
            var apiResponse = await VerifyTasinmazAuthorizationAsync(
                tasinmazId,
                verification,
                user
            );

            if (apiResponse != null && apiResponse.StatusCode == 200 && apiResponse.Data != null)
            {
                // Yetkilendirme başarılıysa, kullanıcının yetkili taşınmazlar listesine ekle
                if (verification.AuthorizedPropertyNumbers == null)
                    verification.AuthorizedPropertyNumbers = new List<string>();
                verification.AuthorizedPropertyNumbers.Add(tasinmazNo);
                _context.UserVerifications.Update(verification);
                await _context.SaveChangesAsync();

                // Başarılı sonucu döndür
                return Json(new
                {
                    isAuthorized = true,
                    message = "Taşınmaz yetkilendirmesi onaylandı.",
                    data = apiResponse.Data
                });
            }

            // Hata mesajını hazırla
            var errorMsg = apiResponse?.Errors != null && apiResponse.Errors.Any() ? string.Join(",", apiResponse.Errors)
                : "Bu taşınmaz için yetkilendirilmediniz.";
            return Json(new { isAuthorized = false, message = errorMsg });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"CheckTasinmazAuthorization hatası: {ex.Message}");
            return Json(new { isAuthorized = false, message = "Sorgulama sırasında bir hata oluştu: " + ex.Message });
        }
    }


    public async Task<EidsResponse> VerifyTasinmazAuthorizationAsync(decimal tasinmazId, UserVerification verification,
        ApplicationUser user)
    {
        var apiBaseUrl = "https://ws.gtb.gov.tr:8443";
        var endpoint = $"{apiBaseUrl}/EidsTasinmazAPI";

        try
        {
            using var client = new HttpClient();
            var username = _configuration["EIDS:Username"];
            var password = _configuration["EIDS:Password"];
            var firmaKodu = _configuration["EIDS:FirmaKodu"];

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!Guid.TryParse(firmaKodu, out var firmaKodGuid))
            {
                return new EidsResponse
                {
                    StatusCode = 400,
                    Errors = new[] { "FirmaKod Guid formatında değil" }
                };
            }

            var requestModel = new
            {
                FirmaKod = firmaKodGuid,
                KullaniciKodu = verification.UserCode,
                TasinmazId = tasinmazId > 0 ? tasinmazId : (decimal?)null,
                VergiNo = !string.IsNullOrEmpty(user.VergiNo) ? user.VergiNo : null
            };

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = null
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestModel, options);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new EidsResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Errors = new[] { $"API isteği başarısız: {response.StatusCode}, Yanıt: {responseContent}" }
                };

            var deserializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (string.IsNullOrWhiteSpace(responseContent))
                return new EidsResponse
                {
                    StatusCode = 500,
                    Errors = new[] { "API boş yanıt döndürdü" }
                };

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<EidsResponse>(responseContent, deserializeOptions);
            }
            catch (System.Text.Json.JsonException jex)
            {
                return new EidsResponse
                {
                    StatusCode = 500,
                    Errors = new[] { "API yanıtı geçersiz format" }
                };
            }
        }
        catch (Exception ex)
        {
            return new EidsResponse
            {
                StatusCode = 500,
                Errors = new[] { $"İstek sırasında hata oluştu: {ex.Message}" }
            };
        }
    }

    // -----------------------------
    //  E-DEVLET CALLBACK
    // -----------------------------
    [HttpGet("EDevletCallback")]
    public async Task<IActionResult> EDevletCallback(string yetkiKodu, string tasinmazListesi)
    {
        // 1) Oturum açmış kullanıcıyı al
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToLogin(); // Projenizde tanımlı bir metot olduğunu varsayıyoruz.

        // 2) EIDS servisinden kullanıcı bilgilerini çek
        //    (örn. TCKN, Ad, Soyad verilerini döndüren bir metot)
        var eidsUserData = await GetKullaniciKoduFromEids(yetkiKodu, user.PhoneNumber);

        // 3) Bu kullanıcı için veritabanında bir doğrulama kaydı var mı?
        var verification = await _context.UserVerifications
            .FirstOrDefaultAsync(uv => uv.UserId == user.Id);

        if (verification == null)
        {
            // Yeni doğrulama kaydı oluştur
            verification = new UserVerification
            {
                UserId = user.Id,
                VerificationCode = yetkiKodu,
                UserCode = eidsUserData.KullaniciKodu,
                AuthorizedPropertyNumbers = tasinmazListesi?.Split(',').ToList(),
                FirstName = eidsUserData?.FirstName,   // EIDS'den gelen Ad
                LastName = eidsUserData?.LastName,     // EIDS'den gelen Soyad
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                VerificationDate = DateTime.UtcNow,
                VerificationExpiryDate = DateTime.UtcNow.AddMonths(6) // 6 ay geçerli
            };
            _context.UserVerifications.Add(verification);
        }
        else
        {
            // Mevcut kaydı güncelle
            verification.VerificationCode = yetkiKodu;
            verification.AuthorizedPropertyNumbers = tasinmazListesi?.Split(',').ToList();
            // EIDS'den gelen veriler boş değilse güncelle
            if (!string.IsNullOrEmpty(eidsUserData?.FirstName))
                verification.FirstName = eidsUserData.FirstName;
            if (!string.IsNullOrEmpty(eidsUserData?.LastName))
                verification.LastName = eidsUserData.LastName;
        }

        // Değişiklikleri kaydet
        await _context.SaveChangesAsync();

        // E-Devlet doğrulaması başarıyla tamamlandıktan sonra KategoriSecim sayfasına yönlendir
        return RedirectToAction("KategoriSecim", new { yetkiKodu, durum = "Basarili" });
    }

    [HttpPost("Kaydet")]
    public async Task<IActionResult> Kaydet([FromForm] IlanModel model)
    {
        if (model == null)
        {
            ModelState.AddModelError(string.Empty, "Model verisi alınamadı.");
            return BadRequest("Gönderilen ilan bilgileri eksik.");
        }
        // Fiyatı doğru şekilde al
        decimal? price = model.Price;
        // Yeni ilan nesnesi oluştur ve model bilgilerini aktar
        IlanModel ilan = new IlanModel()
        {
            Title = model?.Title,
            Description = model?.Description,
            Price = (decimal)price, // Fiyatı olduğu gibi al
            AdaNo = model?.AdaNo,
            ParselNo = model?.ParselNo,
            GirisTarihi = DateTime.Now,
            Category = model?.Category,
            KonutDurumu = model?.KonutDurumu,
            Aidat = model.Aidat,
            AraziNiteliği = model?.AraziNiteliği,
            Area = model.Area,
            ArsaDurumu = model?.ArsaDurumu,
            Asansor = model?.Asansor,
            Balkon = model?.Balkon,
            BanyoSayisi = model?.BanyoSayisi,
            BinaYasi = model?.BinaYasi,
            BrutMetrekare = model?.BrutMetrekare,
            BulunduguKat = model?.BulunduguKat,
            Esyali = model?.Esyali,
            Gabari = model?.Gabari,
            GayrimenkulSahibi = model?.GayrimenkulSahibi,
            IlanNo = model?.IlanNo,
            ImarDurumu = model?.ImarDurumu,
            Isitma = model?.Isitma,
            Kaks = model?.Kaks,
            KatSayisi = model?.KatSayisi,
            Kimden = model?.Kimden,
            Konum = model?.Konum,
            KrediyeUygunluk = model?.KrediyeUygunluk,
            KullanimDurumu = model?.KullanimDurumu,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            MulkTipi = model?.MulkTipi,
            MulkTipiArsa = model?.MulkTipiArsa,
            NetMetrekare = model?.NetMetrekare,
            OdaSayisi = model?.OdaSayisi,
            YatakSayisi = model?.YatakSayisi,
            Otopark = model?.Otopark,
            PaftaNo = model?.PaftaNo,
            SelectedCategories = model?.SelectedCategories,
            Takas = model?.Takas,
            TakasaUygunluk = model?.TakasaUygunluk,
            TapuDurumu = model?.TapuDurumu,
            TasınmazNumarasi = model.TasınmazNumarasi,
            TKGMParselLink = model?.TKGMParselLink,
            UploadedFiles = model?.UploadedFiles,
            VideoLink = model?.VideoLink,
            SerhDurumu = model?.SerhDurumu,
            PricePerSquareMeter = model.PricePerSquareMeter,
            MeyveninCinsi = model?.MeyveninCinsi,
            AcikAlan = model?.AcikAlan,
            KapaliAlan = model?.KapaliAlan,
            GunlukMusteriSayisi = model?.GunlukMusteriSayisi,
            sehir = model?.sehir,
            semt = model?.semt,
            mahalleKoy = model?.mahalleKoy,
            FirstName = model?.FirstName,
            LastName = model?.LastName,
            PhoneNumber = model?.PhoneNumber,
            Email = model?.Email,
            ProfilePicture = model?.ProfilePicture,
            PatronunNotu = model?.PatronunNotu,
            Status = "taslak" // Başlangıçta "taslak" olarak ayarlandı
        };
        Console.WriteLine("Veritabanına Eklendi");
        // İlanı veritabanına ekle
        await _context.IlanBilgileri.AddAsync(ilan);
        await _context.SaveChangesAsync(); // Veritabanına kaydet

        // Fotoğraf yükleme işlemi için KaydetFotograflarAsync metodunu çağır
        if (model.UploadedFiles != null && model.UploadedFiles.Any())
        {
            var fotograflar = await KaydetFotograflarAsync(model.UploadedFiles, ilan.Id);
            await _context.Photos.AddRangeAsync(fotograflar);
        }
        else
        {
            ModelState.AddModelError("UploadedFiles", "Hiç fotoğraf yüklenmedi.");
        }

        // Video yükleme işlemi için KaydetVideolarAsync metodunu çağır
        if (model.UploadedVideos != null && model.UploadedVideos.Any())
        {
            var videolar = await KaydetVideolarAsync(model.UploadedVideos, ilan.Id);
            await _context.Videos.AddRangeAsync(videolar);
        }
        else
        {
            ModelState.AddModelError("UploadedVideos", "Hiç video yüklenmedi.");
        }

        await _context.SaveChangesAsync(); // Fotoğraf ve video verilerini veritabanına kaydet

        // Başarılı kayıttan sonra yönlendirme
        return RedirectToAction("IlanOnizlemesi", new { id = ilan.Id });
    }

    [HttpPost("duzenle")]
    public async Task<IActionResult> Duzenle([FromForm] IlanModel model)
    {
        if (model == null)
        {
            ModelState.AddModelError(string.Empty, "Model verisi alınamadı.");
            return BadRequest("Gönderilen ilan bilgileri eksik.");
        }

        var duzenlenecekUlan = await _context.IlanBilgileri.FirstOrDefaultAsync(_ =>
            _.Id == model.Id
        );

        if (duzenlenecekUlan is null)
        {
            return BadRequest("Düzenlenecek ilan bulunamadı");
        }

        // Fotoğraf yükleme işlemi için KaydetFotograflarAsync metodunu çağır
        if (model.UploadedFiles != null && model.UploadedFiles.Any())
        {
            // Mevcut fotoğrafları silme veya güncelleme mantığı buraya eklenebilir
            // Örneğin: var existingPhotos = _context.Photos.Where(p => p.IlanId == duzenlenecekUlan.Id).ToList();
            // _context.Photos.RemoveRange(existingPhotos);
            // await _context.SaveChangesAsync();
            var newPhotos = await KaydetFotograflarAsync(model.UploadedFiles, duzenlenecekUlan.Id);
            await _context.Photos.AddRangeAsync(newPhotos);
        }

        // Video yükleme işlemi için KaydetVideolarAsync metodunu çağır
        if (model.UploadedVideos != null && model.UploadedVideos.Any())
        {
            // Mevcut videoları silme veya güncelleme mantığı buraya eklenebilir
            var newVideos = await KaydetVideolarAsync(model.UploadedVideos, duzenlenecekUlan.Id);
            await _context.Videos.AddRangeAsync(newVideos);
        }

        // İlan bilgilerini güncelle
        // İLAN SAHİBİ BİLGİLERİ HARİÇ TUTULDU - KİM DÜZENLERSE DÜZENLESİN İLAN SAHİBİ DEĞİŞMEZ
        duzenlenecekUlan.Title = model.Title;
        duzenlenecekUlan.Description = model.Description;
        duzenlenecekUlan.TapuDurumu = model.TapuDurumu;
        duzenlenecekUlan.ArsaDurumu = model.ArsaDurumu;
        duzenlenecekUlan.KonutDurumu = model.KonutDurumu;
        duzenlenecekUlan.ImarDurumu = model.ImarDurumu;
        duzenlenecekUlan.SerhDurumu = model.SerhDurumu;
        duzenlenecekUlan.Price = model.Price;
        duzenlenecekUlan.sehir = model.sehir;
        duzenlenecekUlan.semt = model.semt;
        duzenlenecekUlan.mahalleKoy = model.mahalleKoy;
        duzenlenecekUlan.AcikAlan = model.AcikAlan;
        duzenlenecekUlan.AdaNo = model.AdaNo;
        duzenlenecekUlan.ParselNo = model.ParselNo;
        duzenlenecekUlan.Kaks = model.Kaks;
        duzenlenecekUlan.Gabari = model.Gabari;
        duzenlenecekUlan.Area = model.Area;
        duzenlenecekUlan.BrutMetrekare = model.BrutMetrekare;
        duzenlenecekUlan.NetMetrekare = model.NetMetrekare;
        duzenlenecekUlan.Aidat = model.Aidat;
        duzenlenecekUlan.AraziNiteliği = model.AraziNiteliği;
        duzenlenecekUlan.Asansor = model.Asansor;
        duzenlenecekUlan.Balkon = model.Balkon;
        duzenlenecekUlan.BanyoSayisi = model.BanyoSayisi;
        duzenlenecekUlan.BinaYasi = model.BinaYasi;
        duzenlenecekUlan.BulunduguKat = model.BulunduguKat;
        duzenlenecekUlan.Category = model.Category;
        duzenlenecekUlan.Esyali = model.Esyali;
        duzenlenecekUlan.GirisTarihi = model.GirisTarihi;
        duzenlenecekUlan.GunlukMusteriSayisi = model.GunlukMusteriSayisi;
        duzenlenecekUlan.IlanNo = model.IlanNo;
        duzenlenecekUlan.Isitma = model.Isitma;
        duzenlenecekUlan.KapaliAlan = model.KapaliAlan;
        duzenlenecekUlan.KatSayisi = model.KatSayisi;
        duzenlenecekUlan.Kimden = model.Kimden;
        duzenlenecekUlan.Konum = model.Konum;
        duzenlenecekUlan.KrediyeUygunluk = model.KrediyeUygunluk;
        duzenlenecekUlan.KullanimDurumu = model.KullanimDurumu;
        duzenlenecekUlan.Latitude = model.Latitude;
        duzenlenecekUlan.Longitude = model.Longitude;
        duzenlenecekUlan.MeyveninCinsi = model.MeyveninCinsi;
        duzenlenecekUlan.MulkTipi = model.MulkTipi;
        duzenlenecekUlan.MulkTipiArsa = model.MulkTipiArsa;
        duzenlenecekUlan.NetMetrekare = model.NetMetrekare;
        duzenlenecekUlan.OdaSayisi = model.OdaSayisi;
        duzenlenecekUlan.Otopark = model.Otopark;
        duzenlenecekUlan.PaftaNo = model.PaftaNo;
        duzenlenecekUlan.PatronunNotu = model.PatronunNotu;
        duzenlenecekUlan.Price = model.Price;
        duzenlenecekUlan.PricePerSquareMeter = model.PricePerSquareMeter;
        duzenlenecekUlan.SelectedCategories = model.SelectedCategories;
        duzenlenecekUlan.Status = model.Status;
        duzenlenecekUlan.Takas = model.Takas;
        duzenlenecekUlan.TakasaUygunluk = model.TakasaUygunluk;
        duzenlenecekUlan.TasınmazNumarasi = model.TasınmazNumarasi;
        duzenlenecekUlan.TKGMParselLink = model.TKGMParselLink;
        duzenlenecekUlan.VideoLink = model.VideoLink;
        duzenlenecekUlan.YatakSayisi = model.YatakSayisi;
        duzenlenecekUlan.LastActionDate = DateTime.Now;
        duzenlenecekUlan.Status = "active"; // İlan durumunu "active" olarak ayarla

        await _context.SaveChangesAsync(); // İlan bilgilerini, fotoğraf ve video referanslarını veritabanına kaydet

        return RedirectToAction("IlanOnizlemesi", new { id = model.Id });
    }


    private async Task<List<PhotoModel>> KaydetFotograflarAsync(
        IEnumerable<IFormFile> files,
        int ilanId
    )
    {
        var fotograflar = new List<PhotoModel>();
        if (files != null)
        {
            var savePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images/uploads"
            );
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            foreach (var item in files)
            {
                var uniqName = Guid.NewGuid();
                var uniqFileName = Path.Combine(savePath, $"{uniqName}_{item.FileName}");
                using (var stream = new FileStream(uniqFileName, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }
                fotograflar.Add(
                    new PhotoModel
                    {
                        Url = $"/images/uploads/{uniqName}_{item.FileName}",
                        IlanId = ilanId,
                    }
                );
            }
        }
        return fotograflar;
    }

    private async Task<List<VideoModel>> KaydetVideolarAsync(
        IEnumerable<IFormFile> videos,
        int ilanId
    )
    {
        var videolar = new List<VideoModel>();
        if (videos != null)
        {
            var videoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "videos/uploads"
            );
            if (!Directory.Exists(videoPath))
                Directory.CreateDirectory(videoPath);

            foreach (var video in videos)
            {
                var uniqName = Guid.NewGuid();
                var uniqVideoFileName = Path.Combine(videoPath, $"{uniqName}_{video.FileName}");
                using (var stream = new FileStream(uniqVideoFileName, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                videolar.Add(
                    new VideoModel
                    {
                        Url = $"/videos/uploads/{uniqName}_{video.FileName}",
                        IlanId = ilanId,
                    }
                );
            }
        }
        return videolar;
    }

    [HttpPost]
    public async Task<IActionResult> Edit(IlanDuzenleDTO model)
    {
        if (!ModelState.IsValid) return View(model);

        // Video Yükleme
        if (model.UploadedVideos != null && model.UploadedVideos.Count > 0)
        {
            foreach (var video in model.UploadedVideos)
            {
                var uniqName = Guid.NewGuid();
                var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos/uploads");
                var uniqVideoFileName = Path.Combine(videoPath, $"{uniqName}_{video.FileName}");

                if (!Directory.Exists(videoPath))
                    Directory.CreateDirectory(videoPath);

                using (var stream = new FileStream(uniqVideoFileName, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                string videoUrl = $"/videos/uploads/{uniqName}_{video.FileName}";
                _context.Videos.Add(new VideoModel { Url = videoUrl, IlanId = model.Ilan.Id });
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Edit", new { id = model.Ilan.Id });
    }

    [HttpPost]
    public IActionResult DeletePhoto(int id)
    {
        var photo = _context.Photos.FirstOrDefault(p => p.Id == id);
        if (photo != null)
        {
            _context.Photos.Remove(photo);
            _context.SaveChanges();
            return Ok();
        }
        return NotFound("Fotoğraf bulunamadı.");
    }

    [HttpPost("UploadPhotos")]
    public async Task<IActionResult> UploadPhotos([FromForm] IlanModel model)
    {
        if (model == null)
        {
            return BadRequest("Model verisi alınamadı.");
        }

        List<PhotoModel> fotograflar = new List<PhotoModel>();

        // Fotoğraf yükleme işlemi
        if (model.UploadedFiles != null && model.UploadedFiles.Any()) // .Count yerine .Any()
        {
            foreach (var item in model.UploadedFiles)
            {
                if (item.Length > 0)
                {
                    var uniqName = Guid.NewGuid(); // Dosya adı için benzersiz GUID
                    var savePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "uploads"
                    );

                    // Eğer yükleme klasörü yoksa oluştur
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }

                    var filePath = Path.Combine(savePath, $"{uniqName}_{item.FileName}");

                    // Dosya fiziksel olarak kaydedilir
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    string photoUrl = $"/images/uploads/{uniqName}_{item.FileName}";
                    fotograflar.Add(new PhotoModel
                    {
                        Url = photoUrl,
                        IlanId = model.Id // İlan ID'si ile ilişkilendir
                    });
                }
            }

            if (fotograflar.Count > 0)
            {
                await _context.Photos.AddRangeAsync(fotograflar);
                await _context.SaveChangesAsync(); // Fotoğrafları veritabanına kaydet
                return Ok(new { message = "Fotoğraflar başarıyla yüklendi.", photos = fotograflar });
            }
            else
            {
                return BadRequest("Hiç fotoğraf yüklenmedi.");
            }
        }
        return BadRequest("Fotoğraf yüklenirken bir hata oluştu.");
    }

    [HttpDelete("Video/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var video = await _context.Videos.FirstOrDefaultAsync(v => v.Id == id);
        if (video == null)
        {
            return NotFound("Video bulunamadı");
        }

        // Videonun fiziksel dosyasını sil
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", video.Url.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        // Video kaydını veritabanından sil
        _context.Videos.Remove(video);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UploadVideo(IlanDuzenleDTO model)
    {
        // Model.Ilan üzerinden ilan nesnesine erişim
        var ilan = model.Ilan;
        if (ilan == null)
        {
            return NotFound();
        }

        // Yeni video yükleme işlemi
        if (model.UploadedVideos != null && model.UploadedVideos.Count > 0)
        {
            var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos/uploads");
            if (!Directory.Exists(videoPath))
            {
                Directory.CreateDirectory(videoPath);
            }

            foreach (var video in model.UploadedVideos)
            {
                var uniqName = Guid.NewGuid();
                var filePath = Path.Combine(videoPath, $"{uniqName}_{video.FileName}");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                string videoUrl = $"/videos/uploads/{uniqName}_{video.FileName}";
                // Yeni video ekle
                model._Videolar.Add(new VideoModel { Url = videoUrl, IlanId = ilan.Id });
            }
            // Videolar başarıyla eklendiyse, ilanı tekrar yönlendir
            await _context.SaveChangesAsync();  // Veritabanına kaydetmelisiniz.
        }
        return RedirectToAction("Edit", new { id = ilan.Id });
    }


    [HttpGet]
    public IActionResult IlanOnizlemesi(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
        {
            return NotFound();
        }

        // İlanla ilişkili kullanıcıyı al
        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == ilan.Email);

        // Fotoğrafları ve videoları al
        var fotograf = _context.Photos.AsNoTracking().Where(f => f.IlanId == id).ToList();
        var videolar = _context.Videos.AsNoTracking().Where(v => v.IlanId == id).ToList();

        var model = new TumIlanlarDTO
        {
            _Ilanlar = new List<IlanModel> { ilan },
            _Fotograflar = fotograf,
            _Videolar = videolar,
            User = user // Kullanıcıyı modele ekle
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> IlanOnizlemesi(int id, bool isConfirmed)
    {
        var ilan = _context.IlanBilgileri.FirstOrDefault(i => i.Id == id);
        if (ilan == null)
        {
            return NotFound();
        }

        // "Devam Et" butonuna basılırsa ilanı aktifleştir
        if (isConfirmed)
        {
            ilan.Status = "active";
            _context.Update(ilan);
            await _context.SaveChangesAsync();
        }

        // Yönlendirme işlemi
        return RedirectToAction("Yayinlandi", "Ilan", new { id = ilan.Id }); // İlan aktifleşti ve Yayinlandi sayfasına yönlendirildi
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveKonutIlanEdit(TumIlanlarDTO ilan)
    {
        // Konut için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveIsYeriIlanEdit(TumIlanlarDTO ilan)
    {
        // İş Yeri için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveAraziIlanEdit(TumIlanlarDTO ilan)
    {
        // Arazi için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveBagIlanEdit(TumIlanlarDTO ilan)
    {
        // Bağ için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveBahceIlanEdit(TumIlanlarDTO ilan)
    {
        // Bahçe için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveTarlaIlanEdit(TumIlanlarDTO ilan)
    {
        // Tarla için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveTuristikTesisIlanEdit(TumIlanlarDTO ilan)
    {
        // Turistik Tesis için kaydetme mantığı
        return RedirectToAction("Index");
    }

    [HttpPost("DosyaYukle")]
    public async Task<IActionResult> DosyaYukle(IFormFile[] dosyalar)
    {
        if (dosyalar == null || dosyalar.Length < 1)
            return BadRequest();

        foreach (var dosya in dosyalar)
        {
            var dosyaAdi = Path.GetFileName(dosya.FileName);
            var dosyalarFile = Path.Combine(Directory.GetCurrentDirectory(), "Dosyalar");
            var path = Path.Combine(dosyalarFile, dosyaAdi);

            if (!Directory.Exists(dosyalarFile))
                Directory.CreateDirectory(dosyalarFile);

            using (Stream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                await dosya.CopyToAsync(fs);
            }
            _context.Photos.Add(new PhotoModel { Url = path });
        }
        _context.SaveChanges();
        return Ok();
    }

    // Kategoriye göre yönlendirme
    [HttpGet("/ilan/ver")]
    public IActionResult KategoriYonetimi(string category, string durum, string subcategory)
    {
        if (string.IsNullOrEmpty(category))
        {
            return BadRequest("Kategori seçilmedi.");
        }

        // Seçilen değerleri ViewBag'e aktar
        ViewBag.SelectedCategory = category;
        ViewBag.SelectedDurum = durum;
        ViewBag.SelectedSubcategory = subcategory;

        // Kategoriye göre yönlendirme
        var redirectAction = category switch
        {
            "Konut (Yaşam Alanı)" => nameof(KonutIlan),
            "Arsa" => nameof(AraziIlan),
            "Bağ" => nameof(BagIlan),
            "Bahçe" => nameof(BahceIlan),
            "Tarla" => nameof(TarlaIlan),
            "İş Yeri" => nameof(IsYeriIlan),
            "Turistik Tesis" => nameof(TuristikTesisIlan),
            _ => null,
        };

        return redirectAction != null ? RedirectToAction(redirectAction) : NotFound();
    }

    public IActionResult KonutIlan() => View(new IlanModel { Category = "Konut" });
    public IActionResult IsYeriIlan() => View(new IlanModel { Category = "İş Yeri" });
    public IActionResult AraziIlan() => View(new IlanModel { Category = "Arsa" });
    public IActionResult BagIlan() => View(new IlanModel { Category = "Bağ" });
    public IActionResult BahceIlan() => View(new IlanModel { Category = "Bahçe" });
    public IActionResult TarlaIlan() => View(new IlanModel { Category = "Tarla" });
    public IActionResult TuristikTesisIlan() => View(new IlanModel { Category = "Turistik Tesis" });

    [HttpGet]
    public IActionResult KonutIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    [HttpGet]
    public IActionResult IsYeriIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    [HttpGet]
    public IActionResult AraziIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    [HttpGet]
    public IActionResult BagIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    [HttpGet]
    public IActionResult BahceIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    [HttpGet]
    public IActionResult TarlaIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    [HttpGet]
    public IActionResult TuristikTesisIlanEdit(int id)
    {
        var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id);
        if (ilan == null)
            return NotFound();
        return View(ilan);
    }

    // Başarı sayfası için GET metodu
    public IActionResult Success() => View();

    // İlanın başarıyla yayınlandığı sayfa için GET metodu
    public IActionResult Yayinlandi(int id)
    {
        ViewBag.Id = id; // ID'yi ViewBag ile geçir
        return View(); // Yayinlandi.cshtml sayfasını döndürür
    }

    [HttpPost]
    public async Task<IActionResult> Konutilan(IlanModel ilan)
    {
        if (ModelState.IsValid)
        {
            _context.IlanBilgileri.Add(ilan); // İlan nesnesini burada kullan
            await _context.SaveChangesAsync(); // Değişiklikleri kaydet
            return RedirectToAction("TumIlanlar");
        }
        return View(ilan);
    }

    public IActionResult Step1()
    {
        return View();
    }

    // POST: AraziIlan
    [HttpPost]
    public IActionResult KonutIlan(IlanModel model)
    {
        if (ModelState.IsValid)
        {
            // Yeni ilanı listeye ekle
            model.Id = ilanlar.Count + 1;
            ilanlar.Add(model);
            // Tüm İlanlar sayfasına yönlendir
            return RedirectToAction("TumIlanlar");
        }
        // Form geçerli değilse aynı sayfada kal
        return View(model);
    }

    [HttpGet]
    public IActionResult KonutIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "Konut (Yaşam Alanı)",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult IsYeriIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "İş Yeri",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult AraziIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "Arsa",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult BagIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "Bağ",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult BahceIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "Bahçe",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult TarlaIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "Tarla",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult TuristikTesisIlan(string durum, string subcategory)
    {
        var model = new IlanModel
        {
            Category = "Turistik Tesis",
            KonutDurumu = durum,
            MulkTipi = subcategory,
        };
        return View(model);
    }

    // POST: AraziIlan
    [HttpPost]
    public IActionResult AraziIlan(IlanModel model)
    {
        if (ModelState.IsValid)
        {
            // Yeni ilanı listeye ekle
            model.Id = ilanlar.Count + 1;
            ilanlar.Add(model);
            // Tüm İlanlar sayfasına yönlendir
            return RedirectToAction("TumIlanlar");
        }
        // Form geçerli değilse aynı sayfada kal
        return View(model);
    }

    //public async Task<IActionResult> TumIlanlar()
    //{
    //    var ilanlar = await _context.IlanBilgileri.AsNoTracking().ToListAsync(); // başarılı ilanlar gelir
    //    var fotograflar = await _context.Photos.AsNoTracking().ToListAsync(); // başarısız fotoğraflar gelmez ????
    //    TumIlanlarDTO tumIlanlarDTO = new TumIlanlarDTO
    //    {
    //        _Fotograflar = fotograflar,
    //        _Ilanlar = ilanlar,
    //    };
    //    return View(tumIlanlarDTO);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BagIlan(IlanModel model)
    {
        var ilanlar = _context.IlanBilgileri.AsNoTracking().ToList(); // Veritabanından ilanları getir
        var geciciIlanlar = ilanlar
            .Select(ilan => new IlanModel
            {
                Id = ilan.Id,
                Area = ilan.Area,
                AdaNo = ilan.AdaNo,
                ParselNo = ilan.ParselNo,
                Price = ilan.Price, // Doğru fiyat alanını kullan
                Title =
                    ilan.Title // Başlık mevcut değilse ekle
                ,
            })
            .ToList();

        return View(geciciIlanlar); // Model olarak görüntülemek için gönder
    }

    [HttpDelete]
    public async Task<IActionResult> Sil(int id)
    {
        // Veritabanında ilgili ilanı bul ve silme işlemini yap.
        var ilan = await _context.IlanBilgileri.FirstOrDefaultAsync(i => i.Id == id);
        if (ilan == null)
        {
            return NotFound();
        }

        _context.IlanBilgileri.Remove(ilan);
        var photos = await _context.Photos.Where(x => x.IlanId == ilan.Id).ToListAsync();
        _context.Photos.RemoveRange(photos);
        await _context.SaveChangesAsync(); // SaveChangesAsync kullan, işlem asenkrondur.

        return Ok();
    }


    [HttpPost]
    public async Task<IActionResult> DetayliIlanAra(DetayliAramaRequestDTO detayliAramaRequestDTO) // Bu action, alacağı model parametresiyle bir post isteği yapacak.
    {
        decimal? EnDusukFiyat = 0;
        decimal? EnYuksekFiyat = 0;

        if (detayliAramaRequestDTO.EnDusukFiyat is not null)
            EnDusukFiyat = detayliAramaRequestDTO.EnDusukFiyat;

        if (detayliAramaRequestDTO.EnYuksekFiyat is not null)
            EnYuksekFiyat = detayliAramaRequestDTO.EnYuksekFiyat;

        double? enAzMetrekare = 0;
        double? enCokMetrekare = 0;

        if (detayliAramaRequestDTO.EnAzMetrekare is not null)
            enAzMetrekare = detayliAramaRequestDTO.EnAzMetrekare;

        if (detayliAramaRequestDTO.EnCokMetrekare is not null)
            enCokMetrekare = detayliAramaRequestDTO.EnCokMetrekare;

        var resimliIlanlar = await _context
            .Photos.AsNoTracking()
            .Select(x => x.IlanId)
            .ToListAsync();
        var videoluIlanlar = await _context
            .Videos.AsNoTracking()
            .Select(x => x.IlanId)
            .ToListAsync();

        var arananIlanlar = await _context
            .IlanBilgileri.AsNoTracking().Where(x =>
                (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.Kategori)
                    || x.Category == detayliAramaRequestDTO.Kategori
                )
                && (EnDusukFiyat == 0 || x.Price >= EnDusukFiyat)
                && (EnYuksekFiyat == 0 || x.Price <= EnYuksekFiyat)
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.Il)
                    || x.sehir == detayliAramaRequestDTO.Il
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.Ilce)
                    || x.semt == detayliAramaRequestDTO.Ilce
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.Mahalle)
                    || x.mahalleKoy == detayliAramaRequestDTO.Mahalle
                )
                && (enCokMetrekare == 0 || x.Area <= detayliAramaRequestDTO.EnCokMetrekare)
                && (enAzMetrekare == 0 || x.Area >= detayliAramaRequestDTO.EnAzMetrekare)
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.OdaSayisi)
                    || x.OdaSayisi == detayliAramaRequestDTO.OdaSayisi
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.BinaYasi)
                    || x.BinaYasi == detayliAramaRequestDTO.BinaYasi
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.IsitmaTipi)
                    || x.Isitma == detayliAramaRequestDTO.IsitmaTipi
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.BalkonSayisi)
                    || x.Balkon == detayliAramaRequestDTO.BalkonSayisi
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.AsansorDurumu)
                    || x.Asansor == detayliAramaRequestDTO.AsansorDurumu
                )
                && (
                    string.IsNullOrEmpty(detayliAramaRequestDTO.OtoparkDurumu)
                    || x.Otopark == detayliAramaRequestDTO.OtoparkDurumu
                )
                && (
                    detayliAramaRequestDTO.FotograDurumu == null
                    || (
                        detayliAramaRequestDTO.FotograDurumu == true
                        && resimliIlanlar.Contains(x.Id)
                    )
                    || (
                        detayliAramaRequestDTO.FotograDurumu == false
                        && !resimliIlanlar.Contains(x.Id)
                    )
                )
                && (
                    detayliAramaRequestDTO.VideoDurumu == null
                    || (detayliAramaRequestDTO.VideoDurumu == true && videoluIlanlar.Contains(x.Id))
                    || (
                        detayliAramaRequestDTO.VideoDurumu == false
                        && !videoluIlanlar.Contains(x.Id)
                    )
                )
            )
            .Select(x => new IlanHizliAraResponseDTO
            {
                sehir = x.sehir,
                semt = x.semt,
                mahalleKoy = x.mahalleKoy,
                AdaNo = x.AdaNo,
                ParselNo = x.ParselNo,
                Area = x.Area,
                IlanAciklamasi = x.Description,
                IlanBasligi = x.Title,
                IlanFiyati = x.Price,
                GirisTarihi = x.GirisTarihi,
                Id = x.Id,
            })
            .ToListAsync();

        return View(arananIlanlar);
    }


    [HttpPost]
    public async Task<IActionResult> HizliAra(string anahtarKelime, DetayliAramaRequestDTO detayliAramaRequestDTO)
    {
        // Anahtar kelime bir sayı ise, ilan ID'si olarak ayrıştır.
        int? ilanId = null;
        if (!string.IsNullOrEmpty(anahtarKelime) && int.TryParse(anahtarKelime, out int parsedId))
        {
            ilanId = parsedId;
        }

        // Fiyat değerlerini kontrol et ve varsayılan değerleri ata.
        decimal? EnDusukFiyat = detayliAramaRequestDTO.EnDusukFiyat ?? 0;
        decimal? EnYuksekFiyat = detayliAramaRequestDTO.EnYuksekFiyat ?? 0;

        double? enAzMetrekare = detayliAramaRequestDTO.EnAzMetrekare;
        double? enCokMetrekare = detayliAramaRequestDTO.EnCokMetrekare;

        // Resimli ve videolu ilanların ID'lerini al.
        var resimliIlanlar = await _context.Photos.AsNoTracking()
            .Select(x => x.IlanId)
            .ToListAsync();

        var videoluIlanlar = await _context.Videos.AsNoTracking()
            .Select(x => x.IlanId)
            .ToListAsync();

        // Ana Sorgu
        var query = _context.IlanBilgileri.AsNoTracking().Where(x =>
            x.Status == "active" &&
            (
                string.IsNullOrEmpty(anahtarKelime) ||
                (
                    (x.sehir ?? "").Contains(anahtarKelime) ||
                    (x.semt ?? "").Contains(anahtarKelime) ||
                    (x.mahalleKoy ?? "").Contains(anahtarKelime) ||
                    (x.Title ?? "").Contains(anahtarKelime) ||
                    (x.Description ?? "").Contains(anahtarKelime) ||
                    (ilanId != null && x.Id == ilanId)
                )
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.Kategori) ||
                (x.Category ?? "") == detayliAramaRequestDTO.Kategori
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.MülkTipi) ||
                (x.MulkTipi ?? "") == detayliAramaRequestDTO.MülkTipi
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.KonutDurumu) ||
                (x.KonutDurumu ?? "") == detayliAramaRequestDTO.KonutDurumu
            ) &&
            (EnDusukFiyat == 0 || x.Price >= EnDusukFiyat) &&
            (EnYuksekFiyat == 0 || x.Price <= EnYuksekFiyat) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.Il) ||
                (x.sehir ?? "") == detayliAramaRequestDTO.Il
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.Ilce) ||
                (x.semt ?? "") == detayliAramaRequestDTO.Ilce
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.Mahalle) ||
                (x.mahalleKoy ?? "") == detayliAramaRequestDTO.Mahalle
            ) &&
            (enCokMetrekare == null || x.Area <= enCokMetrekare) &&
            (enAzMetrekare == null || x.Area >= enAzMetrekare) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.OdaSayisi) ||
                (x.OdaSayisi ?? "") == detayliAramaRequestDTO.OdaSayisi
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.BinaYasi) ||
                (x.BinaYasi ?? "") == detayliAramaRequestDTO.BinaYasi
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.IsitmaTipi) ||
                (x.Isitma ?? "") == detayliAramaRequestDTO.IsitmaTipi
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.BalkonSayisi) ||
                (x.Balkon ?? "") == detayliAramaRequestDTO.BalkonSayisi
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.AsansorDurumu) ||
                (x.Asansor ?? "") == detayliAramaRequestDTO.AsansorDurumu
            ) &&
            (
                string.IsNullOrEmpty(detayliAramaRequestDTO.OtoparkDurumu) ||
                (x.Otopark ?? "") == detayliAramaRequestDTO.OtoparkDurumu
            ) &&
            (
                detayliAramaRequestDTO.FotograDurumu == null ||
                (detayliAramaRequestDTO.FotograDurumu == true && resimliIlanlar.Contains(x.Id)) ||
                (detayliAramaRequestDTO.FotograDurumu == false && !resimliIlanlar.Contains(x.Id))
            ) &&
            (
                detayliAramaRequestDTO.VideoDurumu == null ||
                (detayliAramaRequestDTO.VideoDurumu == true && videoluIlanlar.Contains(x.Id)) ||
                (detayliAramaRequestDTO.VideoDurumu == false && !videoluIlanlar.Contains(x.Id))
            )
        );

        // Verileri çek (E-posta adresini de çekiyoruz ki kullanıcıyı bulabilelim)
        var arananIlanlar = await query.Select(x => new
        {
            x.Id,
            Description = x.Description ?? "",
            Title = x.Title ?? "",
            x.Price,
            sehir = x.sehir ?? "",
            semt = x.semt ?? "",
            mahalleKoy = x.mahalleKoy ?? "",
            x.Area,
            x.AdaNo,
            x.ParselNo,
            x.GirisTarihi,
            x.YatakSayisi,
            BinaYasi = x.BinaYasi ?? "",
            x.NetMetrekare,
            x.BrutMetrekare, // Brüt metrekare eklendi
            Category = x.Category ?? "",
            MulkTipi = x.MulkTipi ?? "",
            KonutDurumu = x.KonutDurumu ?? "",
            x.OdaSayisi,
            x.Email // Kullanıcıyı bulmak için
        }).ToListAsync();

        var ilanIdler = arananIlanlar.Select(x => x.Id).ToList();
        var emails = arananIlanlar.Select(x => x.Email).Distinct().ToList();

        // Fotoğrafları al
        var photos = await _context.Photos.AsNoTracking()
            .Where(x => ilanIdler.Contains(x.IlanId))
            .ToListAsync();

        // Kullanıcıları al (Firma adı için)
        var users = await _context.Users.AsNoTracking()
            .Where(u => emails.Contains(u.Email))
            .Select(u => new { u.Email, u.CompanyName, u.FirstName, u.LastName })
            .ToListAsync();

        var listedModel = new List<IlanHizliAraResponseDTO>();

        foreach (var ilan in arananIlanlar)
        {
            // Fotoğraf URL
            string url = photos.FirstOrDefault(p => p.IlanId == ilan.Id)?.Url ?? "";

            // Firma Adı Bulma
            var user = users.FirstOrDefault(u => u.Email == ilan.Email);
            string firmaAdi = user != null && !string.IsNullOrEmpty(user.CompanyName)
                ? user.CompanyName
                : (user != null ? $"{user.FirstName} {user.LastName}" : "Şevval Emlak");

            listedModel.Add(new IlanHizliAraResponseDTO
            {
                IlanAciklamasi = ilan.Description,
                IlanBasligi = ilan.Title,
                IlanFiyati = ilan.Price,
                IlanVitrinImageUrl = url,
                Id = ilan.Id,
                sehir = ilan.sehir,
                semt = ilan.semt,
                mahalleKoy = ilan.mahalleKoy,
                Area = ilan.Area,
                AdaNo = ilan.AdaNo,
                ParselNo = ilan.ParselNo,
                GirisTarihi = ilan.GirisTarihi,
                YatakSayisi = ilan.YatakSayisi,
                BinaYasi = ilan.BinaYasi,
                NetMetrekare = ilan.NetMetrekare,
                BrutMetrekare = ilan.BrutMetrekare,
                OdaSayisi = ilan.OdaSayisi,
                Category = ilan.Category,
                MulkTipi = ilan.MulkTipi,
                KonutDurumu = ilan.KonutDurumu,
                FirmaAdi = firmaAdi
            });
        }

        var kategoriIlanSayilari = new
        {
            KonutIlanlariCount = arananIlanlar.Count(i => i.Category == "Konut (Yaşam Alanı)"),
            IsYeriIlanlariCount = arananIlanlar.Count(i => i.Category == "İş Yeri"),
            TuristikTesisIlanlariCount = arananIlanlar.Count(i => i.Category == "Turistik Tesis"),
            ArsaIlanlariCount = arananIlanlar.Count(i => i.Category == "Arsa"),
            BahceIlanlariCount = arananIlanlar.Count(i => i.Category == "Bahçe"),
            TarlaIlanlariCount = arananIlanlar.Count(i => i.Category == "Tarla")
        };

        ViewBag.KategoriIlanSayilari = kategoriIlanSayilari;
        ViewBag.AnahtarKelime = anahtarKelime;

        return View(listedModel);
    }
}