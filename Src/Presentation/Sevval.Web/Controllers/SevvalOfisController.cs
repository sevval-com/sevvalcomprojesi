using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.AddUser;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using sevvalemlak.csproj.ClientServices.UserServices;
using sevvalemlak.csproj.Dto.RealEstates;
using sevvalemlak.Dto;
using sevvalemlak.Models;
using sevvalemlak.Services;
using Sevval.Web.Models;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using OfficeOpenXml;

namespace YourProjectNamespace.Controllers
{
    public class SevvalOfisController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly INetGsmService _netGsmService;
        private readonly IUserClientService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SevvalOfisController> _logger;

        public SevvalOfisController(IConfiguration configuration, ApplicationDbContext context, INetGsmService netGsmService, IUserClientService userService, UserManager<ApplicationUser> userManager, ILogger<SevvalOfisController> logger)
        {
            _configuration = configuration;
            _context = context;
            _netGsmService = netGsmService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<IActionResult> CheckUserAuthorization()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Unauthorized();
                }
                return RedirectToAction("Login", "Account");
            }

            string companyName = string.Empty;
            if (user.IsConsultant)
            {
                var invitation = await _context.ConsultantInvitations.AsNoTracking().FirstOrDefaultAsync(c => c.Email == user.Email);
                if (invitation != null)
                {
                    var owner = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == invitation.InvitedBy);
                    companyName = owner?.CompanyName;
                }
            }
            else
            {
                companyName = user.CompanyName;
            }

            if (!string.Equals(companyName, "ŞEVVAL EMLAK", StringComparison.OrdinalIgnoreCase))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return new JsonResult(new { success = false, message = "Bu işlem için yetkiniz yok." }) { StatusCode = 403 };
                }

                return new ContentResult
                {
                    ContentType = "text/html; charset=utf-8", // Karakter kodlaması eklendi
                    StatusCode = 403,
                    Content = @"
                        <!DOCTYPE html>
                        <html lang='tr'>
                            <head>
                                <meta charset='UTF-8'>
                                <title>Yetkisiz Erişim</title>
                                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css' rel='stylesheet'>
                            </head>
                            <body class='d-flex align-items-center justify-content-center vh-100 bg-light'>
                                <div class='text-center'>
                                    <h1 class='display-1 fw-bold text-primary'>403</h1>
                                    <p class='fs-3'> <span class='text-danger'>Erişim Reddedildi!</span></p>
                                    <p class='lead'>
                                        Bu sayfayı görüntüleme yetkiniz yok.
                                    </p>
                                    <a href='/' class='btn btn-primary'>Ana Sayfaya Dön</a>
                                </div>
                            </body>
                        </html>"
                };
            }

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return authorizationResult;

            // Üye türlerini ve ilan durumlarını tek bir sorguda saymak için daha verimli bir yaklaşım
            var users = _context.Users;

            var ilanlar = _context.IlanBilgileri;

            // Tüm sorguları paralel olarak çalıştır


            // "Emlakçı" (ve eski "Kurumsal") ve "Bireysel" için sayıları al
            var corporateCount = users.Where(x => x.UserTypes == "Emlakçı" || x.UserTypes == "Kurumsal")?.Count() ?? 0;
            var individualCount = users.Where(x => x.UserTypes == "Bireysel")?.Count() ?? 0;
            var bankCount = users.Where(x => x.UserTypes == "Banka")?.Count() ?? 0;
            var buildingCount = users.Where(x => x.UserTypes == "İnşaat")?.Count() ?? 0;
            var foundationCount = users.Where(x => x.UserTypes == "Vakıf")?.Count() ?? 0;

            // Ücretli & Ücretsiz üyeleri al
            var paidCount = users.Where(x => x.IsSubscribed == "ücretli")?.Count() ?? 0;
            var freeCount = users.Where(x => x.IsSubscribed == "ücretsiz")?.Count() ?? 0;

            // KonutDurumu'na göre ilan sayısını hesapla
            var ilanStatusCounts = new[]
            {
                new { Name = "Satılık", Value = ilanlar.Where(x => x.KonutDurumu == "Satılık")?.Count() ?? 0 },
                new { Name = "Kiralık", Value = ilanlar.Where(x => x.KonutDurumu == "Kiralık")?.Count() ?? 0 },
                new { Name = "Devren Satılık", Value = ilanlar.Where(x => x.KonutDurumu == "Devren Satılık")?.Count() ?? 0 },
                new { Name = "Devren Kiralık", Value = ilanlar.Where(x => x.KonutDurumu == "Devren Kiralık")?.Count() ?? 0 }
            };

            // ViewBag ile verileri gönder
            ViewBag.CorporateCount = corporateCount;
            ViewBag.IndividualCount = individualCount;
            ViewBag.BankCount = bankCount;
            ViewBag.BuildingCount = buildingCount;
            ViewBag.FoundationCount = foundationCount;
            ViewBag.PaidCount = paidCount;
            ViewBag.FreeCount = freeCount;
            ViewBag.PropertyStatusCounts = ilanStatusCounts;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IlanTalepTakip()
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return authorizationResult;

            // Sadece gerekli alanları seçerek ve AsNoTracking kullanarak performansı artır
            var talepler = await _context.SatisTalepleri
                                            .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                                            .OrderByDescending(t => t.CreatedDate)
                                            .ToListAsync();
            return View(talepler);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInterestedPerson(int requestNumber, string interestedPerson)
        {
            var talep = await _context.SatisTalepleri.FirstOrDefaultAsync(t => t.RequestNumber == requestNumber);
            if (talep != null)
            {
                talep.InterestedPerson = interestedPerson;
                _context.Update(talep);
                await _context.SaveChangesAsync();
                return Ok(); // Başarılı yanıt
            }
            return BadRequest(); // Hatalı yanıt
        }

        public async Task<IActionResult> Uyelikler(string filter = "All", int page = 1, int pageSize = 50)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return authorizationResult;

            // Kullanıcıları ve ilanları tek sorguda, sadece gerekli alanları çekerek alıyoruz
            // Tüm kullanıcıları ve ilanları çekmek yerine, sadece ilgili olanları çekmek daha verimli olabilir
            // Bu örnekte tüm kullanıcıları ve ilanları çekmeye devam ediyoruz ancak Select ile iyileştirme yapıyoruz.

            // 🆕 ConsultantInvitations'ı önceden yükle (Danışman-Firma ilişkisi için)
            var consultantInvitations = await _context.ConsultantInvitations
                .AsNoTracking()
                .Where(ci => ci.Status == "Accepted")
                .ToDictionaryAsync(ci => ci.Email, ci => ci.InvitedBy);

            // Tüm kullanıcıları al (filtreleme için)
            var allUsers = await _context.Users.AsNoTracking().ToListAsync();

            // 🆕 Her kullanıcı için firma bilgisini belirle (Danışman ise bağlı olduğu firma)
            foreach (var user in allUsers)
            {
                if (user.IsConsultant && consultantInvitations.ContainsKey(user.Email))
                {
                    var invitedById = consultantInvitations[user.Email];
                    var companyOwner = allUsers.FirstOrDefault(u => u.Id == invitedById);
                    if (companyOwner != null)
                    {
                        // Geçici olarak CompanyName'e firma sahibinin şirket adını yazıyoruz
                        user.CompanyName = $"{companyOwner.CompanyName} (Danışman)";
                    }
                }
            }

            // Sadece 'active' ilanları veritabanı seviyesinde filtrele
            var activeIlanlar = await _context.IlanBilgileri
                .AsNoTracking()
                .Where(i => i.Status == "active") // Veritabanı seviyesinde filtrele
                .Select(i => new IlanModel
                {
                    Id = i.Id,
                    Title = i.Title ?? "", // NULL ise boş string yap
                    Description = i.Description ?? "",
                    Price = i.Price,
                    Area = i.Area,
                    Status = i.Status ?? "inactive",
                    Email = i.Email // Kullanıcının ilanlarını gruplamak için Email'e ihtiyacımız var
                })
                .ToListAsync();

            // Onay bekleyen kullanıcı sayısını hesapla - Case-insensitive karşılaştırma
            var pendingCount = allUsers.Count(u => 
                !string.IsNullOrWhiteSpace(u.IsActive) && 
                u.IsActive.Trim().Equals("passive", StringComparison.OrdinalIgnoreCase));
            
            // Debug için tüm IsActive değerlerini ve sayılarını logla
            var isActiveStats = allUsers
                .GroupBy(u => u.IsActive ?? "(null)")
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();
            
            _logger.LogInformation("IsActive değerleri ve dağılımı: {Stats}", 
                string.Join(", ", isActiveStats.Select(s => $"'{s.Value}': {s.Count}")));
            _logger.LogInformation("Onay bekleyen (passive) kullanıcı sayısı: {Count}", pendingCount);
            
            ViewBag.PendingCount = pendingCount;

            // Filtreleme uygula - Case-insensitive karşılaştırma
            var filteredUsers = allUsers.Where(user =>
                filter == "All" ||
                (filter == "OnayBekleyen" && !string.IsNullOrWhiteSpace(user.IsActive) && user.IsActive.Trim().Equals("passive", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "Bireysel" && !string.IsNullOrWhiteSpace(user.UserTypes) && user.UserTypes.Trim().Equals("Bireysel", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "Emlakçı" && !string.IsNullOrWhiteSpace(user.UserTypes) && user.UserTypes.Trim().Equals("Emlakçı", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "Kurumsal" && !string.IsNullOrWhiteSpace(user.UserTypes) && user.UserTypes.Trim().Equals("Kurumsal", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "Vakıf" && !string.IsNullOrWhiteSpace(user.UserTypes) && user.UserTypes.Trim().Equals("Vakıf", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "İnşaat" && !string.IsNullOrWhiteSpace(user.UserTypes) && user.UserTypes.Trim().Equals("İnşaat", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "Banka" && !string.IsNullOrWhiteSpace(user.UserTypes) && user.UserTypes.Trim().Equals("Banka", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "ücretsiz" && !string.IsNullOrWhiteSpace(user.IsSubscribed) && user.IsSubscribed.Trim().Equals("ücretsiz", StringComparison.OrdinalIgnoreCase)) ||
                (filter == "ücretli" && !string.IsNullOrWhiteSpace(user.IsSubscribed) && user.IsSubscribed.Trim().Equals("ücretli", StringComparison.OrdinalIgnoreCase))
            ).ToList();
            
            // Debug için filtrelenmiş kullanıcı sayısını logla
            _logger.LogInformation("Filtre: {Filter}, Bulunan kullanıcı sayısı: {Count}", filter, filteredUsers.Count);

            // Toplam kayıt sayısı
            var totalRecords = filteredUsers.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Sayfalama uygula
            var pagedUsers = filteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Modeli oluşturuyoruz
            var model = new TumIlanlarDTO
            {
                Users = pagedUsers,
                _Ilanlar = activeIlanlar
            };

            // ViewBag ile sayfalama bilgilerini gönder
            ViewBag.Filter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            model.PageSize = pageSize;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return Json(new { success = false, message = "Yetkiniz yok" });

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                user.IsActive = "active";
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Kullanıcıya onay maili gönder
                    try
                    {
                        await SendApprovalEmail(user.Email, user.FirstName, user.LastName);
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Onay maili gönderilemedi: {Email}", user.Email);
                    }

                    return Json(new { success = true, message = "Kullanıcı başarıyla onaylandı" });
                }

                return Json(new { success = false, message = "Kullanıcı onaylanırken hata oluştu" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApproveUser hatası: {UserId}", userId);
                return Json(new { success = false, message = "Bir hata oluştu" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkApprove(List<string> selectedIds)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return Json(new { success = false, message = "Yetkiniz yok" });

            if (selectedIds == null || !selectedIds.Any())
            {
                return Json(new { success = false, message = "Hiçbir kullanıcı seçilmedi" });
            }

            try
            {
                // Case-insensitive karşılaştırma için ToLower() kullan
                var users = await _context.Users
                    .Where(u => selectedIds.Contains(u.Id) && 
                                u.IsActive != null && 
                                u.IsActive.ToLower().Trim() == "passive")
                    .ToListAsync();

                if (!users.Any())
                {
                    return Json(new { success = false, message = "Onaylanacak bekleyen kullanıcı bulunamadı" });
                }

                int approvedCount = 0;
                foreach (var user in users)
                {
                    user.IsActive = "active";
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        approvedCount++;
                        // Email gönderimi opsiyonel
                        try
                        {
                            await SendApprovalEmail(user.Email, user.FirstName, user.LastName);
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Toplu onay maili gönderilemedi: {Email}", user.Email);
                        }
                    }
                }

                return Json(new { success = true, message = $"{approvedCount} kullanıcı başarıyla onaylandı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BulkApprove hatası");
                return Json(new { success = false, message = "Bir hata oluştu" });
            }
        }

        private async Task SendApprovalEmail(string toEmail, string firstName, string lastName)
        {
            var smtpConfig = _configuration.GetSection("EmailSettings");
            var smtpServer = smtpConfig["SmtpServer"];
            var smtpPort = int.Parse(smtpConfig["SmtpPort"]);
            var smtpUser = smtpConfig["Username"];
            var smtpPassword = smtpConfig["Password"];
            var fromAddress = smtpConfig["FromAddress"];

            var subject = "Üyeliğiniz Onaylandı - Şevval Emlak";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #0d6efd;'>Tebrikler {firstName} {lastName}!</h2>
                    <p>Şevval Emlak üyeliğiniz başarıyla onaylanmıştır.</p>
                    <p>Artık platformumuzdaki tüm özellikleri kullanabilirsiniz.</p>
                    <p><a href='https://www.sevvalemlak.com' style='background-color: #0d6efd; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Siteye Git</a></p>
                    <br>
                    <p>İyi günler dileriz,</p>
                    <p><strong>Şevval Emlak Ekibi</strong></p>
                </body>
                </html>";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromAddress, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }

        // 🆕 Üye İstatistikleri API (Modal için)
        [HttpGet]
        public async Task<IActionResult> GetMemberStats(string userId)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return Json(new { success = false, message = "Yetkiniz yok" });

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                // İlan istatistiklerini hesapla
                var userIlanlar = await _context.IlanBilgileri
                    .AsNoTracking()
                    .Where(i => i.Email == user.Email)
                    .ToListAsync();

                var stats = new
                {
                    userId = userId,
                    fullName = $"{user.FirstName} {user.LastName}",
                    email = user.Email,
                    toplamIlanSayisi = userIlanlar.Count,
                    fotografliIlanSayisi = userIlanlar.Count(i => !string.IsNullOrEmpty(i.ProfilePicture)),
                    fotografsizIlanSayisi = userIlanlar.Count(i => string.IsNullOrEmpty(i.ProfilePicture)),
                    videoluIlanSayisi = userIlanlar.Count(i => !string.IsNullOrEmpty(i.VideoLink)),
                    videosuzIlanSayisi = userIlanlar.Count(i => string.IsNullOrEmpty(i.VideoLink)),
                    sonIlanTarihi = userIlanlar.Any() ? userIlanlar.Max(i => i.GirisTarihi).ToString("dd.MM.yyyy HH:mm") : "Henüz ilan yok"
                };

                return Json(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMemberStats hatası: {UserId}", userId);
                return Json(new { success = false, message = "İstatistikler yüklenirken hata oluştu" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkDelete(List<string> selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
            {
                try
                {
                    // Seçilen kullanıcıları bul
                    var usersToDelete = await _context.Users
                        .Where(u => selectedIds.Contains(u.Id))
                        .ToListAsync();

                    if (usersToDelete.Any())
                    {
                        // Kullanıcıları sil
                        _context.Users.RemoveRange(usersToDelete);
                        await _context.SaveChangesAsync();

                        // Başarı mesajı
                        TempData["SuccessMessage"] = $"{usersToDelete.Count} kullanıcı başarıyla silindi.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Silinecek kullanıcı bulunamadı.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Silme işlemi sırasında bir hata oluştu: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Silinecek kullanıcı seçilmedi.";
            }

            return RedirectToAction("Uyelikler");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string Id, string FirstName, string LastName, string Email, string UserTypes, string CompanyName, string IsSubscribed)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Uyelikler");
                }

                // Kullanıcı bilgilerini güncelle
                user.FirstName = FirstName?.Trim();
                user.LastName = LastName?.Trim();
                user.Email = Email?.Trim();
                user.UserTypes = UserTypes;
                user.IsSubscribed = IsSubscribed;

                // Bireysel kullanıcılar için firma adını temizle
                if (UserTypes == "Bireysel")
                {
                    user.CompanyName = null;
                }
                else
                {
                    user.CompanyName = CompanyName?.Trim();
                }

                // Değişiklikleri kaydet
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kullanıcı bilgileri başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Güncelleme işlemi sırasında bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Uyelikler");
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDTO model)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doğru şekilde doldurunuz.";
                    return RedirectToAction("Uyelikler");
                }

                if (model.UserTypes == "Bireysel")
                {
                    var individual = await _userService.IndividualRegister(new IndividualRegisterCommandRequest
                    {
                        FirstName = model.FirstName?.Trim(),
                        LastName = model.LastName?.Trim(),
                        Email = model.Email?.Trim(),
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        PhoneNumber = model.PhoneNumber?.Trim(),
                     
                        ProfilePicture = model.ProfilePicture,
                        Source = "SevvalOfis"
                    }, CancellationToken.None);

                    if (individual.IsSuccessfull)
                    {
                        TempData["SuccessMessage"] = individual.Message ?? "Yeni kullanıcı başarıyla eklendi.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = individual.Message ?? "Kullanıcı ekleme işlemi başarısız oldu.";
                    }
                }
                else
                {
                    var corporate = await _userService.CorporateRegister(new CorporateRegisterCommandRequest
                    {
                        FirstName = model.FirstName?.Trim(),
                        LastName = model.LastName?.Trim(),
                        Email = model.Email?.Trim(),
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        PhoneNumber = model.PhoneNumber?.Trim(),
                        CompanyName = model.CompanyName?.Trim(),
                        City = model.City?.Trim(),
                        District = model.District?.Trim(),
                        Address = model.Address?.Trim(),
                        Reference = model.Reference?.Trim(),
                        ProfilePicture = model.ProfilePicture,
                        Level5Certificate = model.Level5Certificate,
                        TaxPlate = model.TaxPlate
                    }, CancellationToken.None);

                    if (corporate.IsSuccessfull)
                    {
                        TempData["SuccessMessage"] = corporate.Message ?? "Yeni kullanıcı başarıyla eklendi.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = corporate.Message ?? "Kullanıcı ekleme işlemi başarısız oldu.";
                    }
                }
                // UserService kullanarak kullanıcı ekleme işlemini gerçekleştir

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcı ekleme işlemi sırasında bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Uyelikler");
        }

        public IActionResult BireyselKullanicilar(string referansFilter, DateTime? registrationFrom, DateTime? registrationTo)
        {
            // "Bireysel" kullanıcılarını temel alıyoruz.
            var usersQuery = _context.Users.AsNoTracking().Where(u => u.UserTypes == "Bireysel");

            // Referans filtreleme (boş olmayan referans değerlerinden seçilen)
            if (!string.IsNullOrEmpty(referansFilter))
            {
                usersQuery = usersQuery.Where(u => u.Referans == referansFilter);
            }

            // Kayıt Tarihi filtreleme: başlangıç ve/veya bitiş tarihine göre
            if (registrationFrom.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.RegistrationDate >= registrationFrom.Value);
            }
            if (registrationTo.HasValue)
            {
                // Bitiş tarihini gün sonuna kadar al
                usersQuery = usersQuery.Where(u => u.RegistrationDate <= registrationTo.Value.AddDays(1).AddSeconds(-1));
            }

            var users = usersQuery.OrderBy(u => u.RegistrationDate).ToList();

            // Kullanıcıların ilan sayılarını bellek üzerinde gruplayarak bul
            // Tüm ilanları çekip gruplamak, her kullanıcı için ayrı sorgu yapmaktan daha verimli olabilir
            var allIlanlar = _context.IlanBilgileri.AsNoTracking().Select(i => new { i.Email, i.Id }).ToList();
            var userIlanSayilari = users.ToDictionary(
                user => user.Id,
                user => allIlanlar.Count(ilan => ilan.Email == user.Email)
            );

            int toplamIlan = userIlanSayilari.Values.Sum();

            // Referans filtreleme için: "Bireysel" kullanıcılarının, referans alanında dolu olan veriler (distinct)
            var distinctReferans = _context.Users
                .AsNoTracking()
                .Where(u => u.UserTypes == "Bireysel" && !string.IsNullOrEmpty(u.Referans))
                .Select(u => u.Referans)
                .Distinct()
                .ToList();

            // ConsultantInvitations sorgusu optimize edildi
            var consultantInvitations = _context.ConsultantInvitations
               .AsNoTracking()
               .Where(ci => (ci.CompanyName == "ŞEVVAL EMLAK" || ci.CompanyName == "ACR EMLAK")
                            && _context.Users.Any(u => u.Email == ci.Email)) // Veritabanı seviyesinde email kontrolü
               .ToList(); // ToList() ile veriyi belleğe çek

            var model = new BireyselKullaniciViewModel
            {
                Users = users,
                ConsultantInvitations = consultantInvitations,
                DistinctReferansValues = distinctReferans,
                FilterReferans = referansFilter,
                FilterRegistrationFrom = registrationFrom,
                FilterRegistrationTo = registrationTo,
                UserIlanSayilari = userIlanSayilari,
                ToplamKullaniciSayisi = users.Count, // Filtrelenmiş kullanıcı sayısı
                ToplamIlanSayisi = toplamIlan     // Filtrelenmiş kullanıcılara ait toplam ilan sayısı
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ReferansSec(string userId, int invitationId)
        {
            var kullanici = _context.Users.Find(userId);
            if (kullanici != null)
            {
                if (invitationId == -1) // TANER TÜMEN seçildiyse
                {
                    kullanici.Referans = "TANER TÜMEN";
                    _context.SaveChanges();
                    return RedirectToAction(nameof(BireyselKullanicilar));
                }
                else
                {
                    var invitation = _context.ConsultantInvitations.FirstOrDefault(
                        ci => ci.Id == invitationId &&
                        (ci.CompanyName == "ŞEVVAL EMLAK" || ci.CompanyName == "ACR EMLAK")
                    );
                    if (invitation != null)
                    {
                        kullanici.Referans = $"{invitation.FirstName} {invitation.LastName}";
                        _context.SaveChanges();
                        return RedirectToAction(nameof(BireyselKullanicilar)); // Sayfayı yenile
                    }
                }
            }
            return NotFound(); // Kullanıcı bulunamazsa
        }

        public IActionResult ExportBireyselKullanicilarToExcel(string referansFilter, DateTime? registrationFrom, DateTime? registrationTo)
        {
            // "Bireysel" kullanıcılarını temel alıyoruz.
            var usersQuery = _context.Users.AsNoTracking().Where(u => u.UserTypes == "Bireysel");

            // Filtreleme işlemleri
            if (!string.IsNullOrEmpty(referansFilter))
            {
                usersQuery = usersQuery.Where(u => u.Referans == referansFilter);
            }
            if (registrationFrom.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.RegistrationDate >= registrationFrom.Value);
            }
            if (registrationTo.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.RegistrationDate <= registrationTo.Value.AddDays(1).AddSeconds(-1));
            }

            var users = usersQuery.OrderBy(u => u.RegistrationDate).ToList();

            // Kullanıcıların ilan sayılarını bul
            var allIlanlar = _context.IlanBilgileri.AsNoTracking().Select(i => new { i.Email, i.Id }).ToList();
            var userIlanSayilari = users.ToDictionary(
                user => user.Id,
                user => allIlanlar.Count(ilan => ilan.Email == user.Email)
            );

            // Excel oluşturma
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Bireysel Kullanıcılar");

                // Başlık satırını oluştur
                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 12;
                worksheet.Row(1).Style.Font.FontColor = XLColor.White;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#003366");

                // Birleştirilmiş hücre oluştur ve başlığı yaz
                worksheet.Range(1, 1, 1, 7).Merge().Value = "Bireysel Kullanıcılar";
                worksheet.Range(1, 1, 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(1, 1, 1, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                // Sütun başlıklarını ekle
                worksheet.Cell(2, 1).Value = "No";
                worksheet.Cell(2, 2).Value = "Ad Soyad";
                worksheet.Cell(2, 3).Value = "Mail Adresi";
                worksheet.Cell(2, 4).Value = "Telefon";
                worksheet.Cell(2, 5).Value = "Üyelik Tarihi";
                worksheet.Cell(2, 6).Value = "İlan Sayısı";
                worksheet.Cell(2, 7).Value = "Referans";

                // Verileri ekle
                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    worksheet.Cell(i + 3, 1).Value = i + 1;
                    worksheet.Cell(i + 3, 2).Value = $"{user.FirstName} {user.LastName}";
                    worksheet.Cell(i + 3, 3).Value = user.Email;
                    worksheet.Cell(i + 3, 4).Value = user.PhoneNumber;
                    worksheet.Cell(i + 3, 5).Value = user.RegistrationDate.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(i + 3, 6).Value = userIlanSayilari.TryGetValue(user.Id, out var ilanCount) ? ilanCount : 0;
                    worksheet.Cell(i + 3, 7).Value = user.Referans;
                }

                // Tüm hücrelere kenarlık ekle
                worksheet.Cells().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Cells().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                // Sütunları otomatik boyutlandır
                worksheet.Columns().AdjustToContents();
                worksheet.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                // Excel dosyasını belleğe yaz
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Dosyayı indirme için gerekli başlıkları ayarla
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "BireyselKullanicilar.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult GetUserIlans(string email)
        {
            // Sadece id ve title alanlarını çekerek daha hafif bir yanıt döndür
            var ilans = _context.IlanBilgileri
                .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                .Where(i => i.Email == email)
                // JSON’da camelCase olarak dönüyoruz:
                .Select(i => new
                {
                    id = i.Id,
                    title = i.Title
                })
                .ToList();

            return Json(ilans);
        }

        public async Task<IActionResult> SehirlereGoreEmlakcilar(RealEstateSearchDto request)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return authorizationResult;

            var query = _context.Users
                .AsNoTracking()
                .Where(u => (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal") && u.IsConsultant == false);

            if (!string.IsNullOrEmpty(request.City))
            {
                query = query.Where(u => u.City == request.City);
            }

            if (!string.IsNullOrEmpty(request.District))
            {
                query = query.Where(u => u.District == request.District);
            }

            if (!string.IsNullOrEmpty(request.CompanySearch))
            {
                query = query.Where(u => u.CompanyName.ToLower().Contains(request.CompanySearch.ToLower()));
            }

            if (request.AddressFilter == "WithAddress")
            {
                query = query.Where(u => !string.IsNullOrEmpty(u.AcikAdres));
            }
            else if (request.AddressFilter == "WithoutAddress")
            {
                query = query.Where(u => string.IsNullOrEmpty(u.AcikAdres));
            }

            var emlakcilar = await query.ToListAsync();

            var allIlanlar = await _context.IlanBilgileri
                .AsNoTracking()
                .Where(i => i.Status == "active")
                .Select(i => new { i.Email, i.Id })
                .ToListAsync();

            var emlakcilarWithCounts = emlakcilar.Select(emlakci => new
            {
                Emlakci = emlakci,
                IlanSayisi = allIlanlar.Count(ilan => ilan.Email == emlakci.Email)

            }).ToList();

            if (request.AnnouncementFilter == "WithAnnouncements")
            {
                emlakcilarWithCounts = emlakcilarWithCounts.Where(e => e.IlanSayisi > 0).ToList();
            }
            else if (request.AnnouncementFilter == "WithoutAnnouncements")
            {
                emlakcilarWithCounts = emlakcilarWithCounts.Where(e => e.IlanSayisi == 0).ToList();
            }

            // Sıralama
            switch (request.SortBy?.ToLower())
            {
                case "companyname":
                    emlakcilarWithCounts = request.SortOrder == "DESC" 
                        ? emlakcilarWithCounts.OrderByDescending(e => e.Emlakci.CompanyName).ToList()
                        : emlakcilarWithCounts.OrderBy(e => e.Emlakci.CompanyName).ToList();
                    break;
                case "firstname":
                    emlakcilarWithCounts = request.SortOrder == "DESC" 
                        ? emlakcilarWithCounts.OrderByDescending(e => e.Emlakci.FirstName).ToList()
                        : emlakcilarWithCounts.OrderBy(e => e.Emlakci.FirstName).ToList();
                    break;
                case "city":
                    emlakcilarWithCounts = request.SortOrder == "DESC" 
                        ? emlakcilarWithCounts.OrderByDescending(e => e.Emlakci.City).ToList()
                        : emlakcilarWithCounts.OrderBy(e => e.Emlakci.City).ToList();
                    break;
                case "registrationdate":
                    emlakcilarWithCounts = request.SortOrder == "DESC" 
                        ? emlakcilarWithCounts.OrderByDescending(e => e.Emlakci.RegistrationDate).ToList()
                        : emlakcilarWithCounts.OrderBy(e => e.Emlakci.RegistrationDate).ToList();
                    break;
                case "announcementcount":
                    emlakcilarWithCounts = request.SortOrder == "DESC" 
                        ? emlakcilarWithCounts.OrderByDescending(e => e.IlanSayisi).ToList()
                        : emlakcilarWithCounts.OrderBy(e => e.IlanSayisi).ToList();
                    break;
                default:
                    emlakcilarWithCounts = emlakcilarWithCounts.OrderBy(e => e.Emlakci.CompanyName).ToList();
                    break;
            }

            var totalRecords = emlakcilarWithCounts.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var pagedEmlakcilar = emlakcilarWithCounts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var sehirler = await _context.Users
                .AsNoTracking()
                .Where(u => (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal") && u.IsConsultant == false)
                .Select(u => u.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var ilceler = await _context.Users
                .AsNoTracking()
                .Where(u => (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal") && u.IsConsultant == false)
                .Where(u => string.IsNullOrEmpty(request.City) || u.City == request.City)
                .Select(u => u.District)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            var model = new TumIlanlarDTO
            {
                Users = pagedEmlakcilar.Select(e => e.Emlakci).ToList(),
                AvailableCities = sehirler,
                AvailableDistricts = ilceler
            };

            ViewBag.IlanSayilari = pagedEmlakcilar.ToDictionary(e => e.Emlakci.Id, e => e.IlanSayisi);
            ViewBag.SelectedCity = request.City;
            ViewBag.SelectedDistrict = request.District;
            ViewBag.CompanySearch = request.CompanySearch;
            ViewBag.SortBy = request.SortBy;
            ViewBag.SortOrder = request.SortOrder;
            ViewBag.AddressFilter = request.AddressFilter;
            ViewBag.AnnouncementFilter = request.AnnouncementFilter;
            
            ViewBag.CurrentPage = request.Page;
            ViewBag.PageSize = request.PageSize;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = request.Page > 1;
            ViewBag.HasNextPage = request.Page < totalPages;

            request.Announcements = model;

            return View(request);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateOpenAddress([FromBody] UpdateAddressModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.userId))
            {
                return Json(new { success = false });
            }
            // FindAsync yerine FirstOrDefaultAsync kullanılarak daha esnek sorgu imkanı sağlanabilir
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.userId);
            if (user != null)
            {
                user.AcikAdres = model.openAddress;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public class UpdateAddressModel
        {
            public string userId { get; set; }
            public string openAddress { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return Json(new List<string>());
            }

            var districts = await _context.Users
                .AsNoTracking()
                .Where(u => (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal") && u.IsConsultant == false && u.City == city)
                .Select(u => u.District)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Json(districts);
        }

     

       
        

        public async Task<IActionResult> DownloadExcel(string? city)
        {
            var query = _context.Users
                .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                .Where(u => (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal") && u.IsConsultant == false);

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(u => u.City == city);
            }

            var emlakcilar = await query.OrderBy(u => u.City).ToListAsync();

            // EPPlus için lisans ayarı: Sınıfları tam nitelikli olarak kullanıyoruz.
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Emlakcilar");

                // Başlık satırı
                worksheet.Cells[1, 1].Value = "Şehir";
                worksheet.Cells[1, 2].Value = "İsim";
                worksheet.Cells[1, 3].Value = "Firma Adı";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Telefon";

                int row = 2;
                foreach (var emlakci in emlakcilar)
                {
                    // Şehir ve ilçe bilgisini birleştiriyoruz
                    worksheet.Cells[row, 1].Value = $"{emlakci.City} / {emlakci.District}";
                    worksheet.Cells[row, 2].Value = $"{emlakci.FirstName} {emlakci.LastName}";
                    worksheet.Cells[row, 3].Value = emlakci.CompanyName;
                    worksheet.Cells[row, 4].Value = emlakci.Email;
                    worksheet.Cells[row, 5].Value = emlakci.PhoneNumber;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Emlakcilar-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Sil(string id)
        {
            // Kullanıcıyı veritabanından bul
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcıya ait ilanları bul
            var userIlanlar = _context.IlanBilgileri.Where(i => i.Email == user.Email).ToList();

            // İlanların status değerini "archive" yapıyoruz
            foreach (var ilan in userIlanlar)
            {
                ilan.Status = "archive"; // Status'u "archive" yapıyoruz
            }

            // Kullanıcıyı veritabanından sil
            _context.Users.Remove(user);

            // Değişiklikleri kaydediyoruz
            await _context.SaveChangesAsync();

            // Listeleme sayfasına yönlendir
            return RedirectToAction("Uyelikler");
        }


        public IActionResult EidsOnayliKullanicilar()
        {

            // Sadece gerekli alanları çekerek ve AsNoTracking kullanarak performansı artır
            var dto = new TumIlanlarDTO
            {
                UserVerifications = _context.UserVerifications.AsNoTracking().ToList(),
                _Ilanlar = _context.IlanBilgileri.AsNoTracking().ToList() // veya filtrelemelerle birlikte
            };
            return View(dto);
        }


        public async Task<IActionResult> TumIlanlar(string companyName, string status, string selectedUser, string category)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return authorizationResult;

            // İlanları filtreleme
            var ilanlarQuery = _context.IlanBilgileri.AsNoTracking(); // Takip etmeyi kapat

            // Firma ismi ile filtreleme
            if (!string.IsNullOrEmpty(companyName))
            {
                var consultantEmails = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.CompanyName == companyName)
                    .Select(u => u.Email)
                    .ToListAsync();

                ilanlarQuery = ilanlarQuery.Where(i => consultantEmails.Contains(i.Email));
            }

            // Durum ile filtreleme
            if (!string.IsNullOrEmpty(status))
            {
                ilanlarQuery = ilanlarQuery.Where(i => i.Status == status);
            }

            // Üye ile filtreleme
            if (!string.IsNullOrEmpty(selectedUser))
            {
                ilanlarQuery = ilanlarQuery.Where(i => i.Email == selectedUser);
            }

            // Kategori ile filtreleme
            if (!string.IsNullOrEmpty(category))
            {
                ilanlarQuery = ilanlarQuery.Where(i => i.Category == category);
            }

            var ilanlar = await ilanlarQuery
                .Select(i => new IlanModel
                {
                    Id = i.Id,
                    Title = i.Title ?? "", // NULL ise boş string yap
                    Description = i.Description ?? "",
                    Price = i.Price,
                    Area = i.Area,
                    Status = i.Status ?? "inactive", // NULL ise "inactive" yap
                                                     // Diğer alanlar da burada NULL kontrolü ile alınabilir
                })
                .ToListAsync();

            var usersTask = _context.Users.AsNoTracking().ToListAsync();
            var consultantInvitationsTask = _context.ConsultantInvitations.AsNoTracking().ToListAsync();

            await Task.WhenAll(usersTask, consultantInvitationsTask);

            var users = usersTask.Result;
            var consultantInvitations = consultantInvitationsTask.Result;

            var userWithCompany = users.Select(user => new
            {
                User = user,
                CompanyName = user.IsConsultant
                    ? consultantInvitations.FirstOrDefault(ci => ci.Email == user.Email)?.CompanyName
                    : user.CompanyName
            }).Cast<dynamic>().ToList(); // Hata düzeltildi: anonim tip dynamic'e dönüştürüldü.


            var model = new TumIlanlarDTO
            {
                _Ilanlar = ilanlar,
                UsersWithCompany = userWithCompany, // Dynamic liste ile eşleştir
            };

            return View(model);
        }

        public IActionResult GununIlaniTakip()
        {
            // Aktif ilanlar
            var ilanlar = _context.IlanBilgileri
                .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                .Where(i => i.Status == "active")
                .Select(i => new IlanModel
                {
                    Id = i.Id,
                    Category = i.Category ?? "",
                    KonutDurumu = i.KonutDurumu ?? "",
                    MulkTipi = i.MulkTipi ?? "",
                    Title = i.Title ?? "",
                    Description = i.Description ?? "",
                    Price = i.Price,
                    Area = i.Area,
                    Email = i.Email ?? "",
                    Status = i.Status ?? "",
                    PhoneNumber = i.PhoneNumber ?? "",
                    FirstName = i.FirstName ?? "",
                    LastName = i.LastName ?? "",
                    LastActionDate = i.LastActionDate ?? DateTime.Now // NULL ise bugünün tarihini ata
                })
                .ToList();


            // Gunun ilanlarını veritabanından getirin
            var gununIlanlari = _context.GununIlanlari
                                        .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                                        .OrderBy(ilan => ilan.YayinlanmaTarihi)
                                        .ToList();

            var model = new GununIlanViewModel
            {
                Ilanlar = ilanlar,
                GununIlanlari = gununIlanlari
            };

            return View(model);
        }

        // Diğer action'lar...

        public IActionResult AfisTalepler()
        {

            var afisTalepler = _context.AfisTalepler.AsNoTracking().ToList(); // Takip etmeyi kapat
            return View(afisTalepler);
        }

        [HttpGet]
        public IActionResult GetUserAddress(string email)
        {
            var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == email); // Takip etmeyi kapat
            if (user != null)
            {
                return Json(new { address = user.AcikAdres });
            }
            else
            {
                return Json(new { address = "Adres bulunamadı." });
            }
        }

        // Yeni action: Durum güncelleme için
        [HttpPost]
        public async Task<IActionResult> UpdateDurum(int id, string durum)
        {
            var afisTalep = await _context.AfisTalepler.FindAsync(id);
            if (afisTalep == null)
            {
                return Json(new { success = false, message = "Afiş talebi bulunamadı." });
            }

            afisTalep.Durum = durum;
            _context.Entry(afisTalep).State = EntityState.Modified; //bu satırı ekledim
            try
            {
                await _context.SaveChangesAsync();

                // Kullanıcının adını ve soyadını al
                var kullanici = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == afisTalep.Email);
                string kullaniciAdi = kullanici?.FirstName;
                string kullaniciSoyadi = kullanici?.LastName;

                // E-posta gönderme işlemini burada gerçekleştirin
                await SendDurumUpdateEmail(afisTalep.Email, durum, afisTalep.Id, kullaniciAdi, kullaniciSoyadi);

                return Json(new { success = true, message = "Durum güncellendi." });
            }
            catch (DbUpdateException ex)
            {
                // Hata loglaması yapılmalı (Önemli)
                // Log.Error("Veritabanı hatası oluştu: {ErrorMessage}", ex.Message); // Örneğin Serilog ile loglama
                return Json(new { success = false, message = "Veritabanı hatası: " + ex.Message });
            }
        }

        private async Task SendDurumUpdateEmail(string toEmail, string durum, int afisTalepId, string kullaniciAdi, string kullaniciSoyadi)
        {
            // Ayarları çek
            var smtp = _configuration.GetSection("Email");
            string smtpServer = smtp["SmtpServer"];
            int smtpPort = int.Parse(smtp["SmtpPort"]);
            string smtpUser = smtp["Username"];
            string smtpPass = smtp["Password"];
            string fromAddress = smtp["FromAddress"];
            string siteName = "sevvalemlak.com.tr";

            // Logo dosya yolu
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "favlogo.webp");

            var mail = new MailMessage();
            mail.From = new MailAddress(fromAddress, siteName);
            mail.To.Add(toEmail);
            mail.IsBodyHtml = true;

            // Logo’yu embed et
            var inlineLogo = new LinkedResource(logoPath, "image/webp")
            {
                ContentId = "logoImage"
            };
            string subject = "";
            string body = "";

            // Duruma göre konu ve mesaj içeriğini ayarla
            switch (durum)
            {
                case "SIRAYA ALINDI":
                    subject = "Siparişiniz Sıraya Alındı";
                    body = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Sipariş Sıraya Alındı</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f0f0f0; /* Daha açık bir arka plan */
            margin: 0;
            padding: 0;
        }}
        .container {{
            background-color: #ffffff;
            max-width: 600px;
            margin: 30px auto; /* Daha fazla üst/alt boşluk */
            padding: 30px;
            border-radius: 12px; /* Daha yuvarlak kenarlar */
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1); /* Daha belirgin gölge */
            border: 1px solid #e0e0e0; /* Hafif kenar çizgisi */
        }}
        .header {{
            text-align: center;
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .header img {{
            max-width: 180px; /* Daha büyük logo */
            height: auto;
        }}
        .header h2 {{
            color: #007bff; /* Daha canlı mavi */
            margin-top: 15px; /* Üst başlık boşluğu */
            margin-bottom: 25px;
            font-size: 24px; /* Daha büyük başlık */
        }}
        .message {{
            font-size: 1.1em;
            color: #555555; /* Daha koyu metin */
            line-height: 1.7; /* Daha fazla satır aralığı */
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .order-id {{
            font-weight: bold;
            color: #333333; /* Daha koyu sipariş no */
        }}
        .footer {{
            text-align: center;
            margin-top: 30px; /* Artırılmış boşluk */
            font-size: 0.9em;
            color: #888888; /* Daha açık gri */
            border-top: 1px solid #d0d0d0; /* Daha açık çizgi */
            padding-top: 15px; /* Üst boşluk */
        }}
        .blue-bg {{ /* Yeni sınıf */
            background-color: #e3f2fd; /* Açık mavi */
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header blue-bg'>
            <img src='cid:logoImage' alt='Şevval Emlak Logo'>
            <h2>Siparişiniz Sıraya Alındı</h2>
        </div>
        <div class='message'>
            <p>Sayın <span class='order-id'>{kullaniciAdi} {kullaniciSoyadi}</span>,</p>
            <p><span class='order-id'>#{afisTalepId}</span> numaralı afiş siparişiniz için işleminiz sıraya alınmıştır. En kısa sürede hazırlanmaya başlanacaktır.</p>
        </div>
        <div class='footer'>
            <p>Bu e-posta Şevval Emlak tarafından otomatik olarak gönderilmiştir.</p>
        </div>
    </div>
</body>
</html>
";
                    break;
                case "SİPARİŞ HAZIRLANIYOR":
                    subject = "Siparişiniz Hazırlanıyor";
                    body = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Siparişiniz Hazırlanıyor</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f0f0f0; /* Daha açık bir arka plan */
            margin: 0;
            padding: 0;
        }}
        .container {{
            background-color: #ffffff;
            max-width: 600px;
            margin: 30px auto; /* Daha fazla üst/alt boşluk */
            padding: 30px;
            border-radius: 12px; /* Daha yuvarlak kenarlar */
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1); /* Daha belirgin gölge */
             border: 1px solid #e0e0e0; /* Hafif kenar çizgisi */
        }}
        .header {{
            text-align: center;
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .header img {{
            max-width: 180px; /* Daha büyük logo */
            height: auto;
        }}
        .header h2 {{
            color: #007bff; /* Daha canlı mavi */
            margin-top: 15px; /* Üst başlık boşluğu */
            margin-bottom: 25px;
             font-size: 24px; /* Daha büyük başlık */
        }}
        .message {{
            font-size: 1.1em;
            color: #555555; /* Daha koyu metin */
            line-height: 1.7; /* Daha fazla satır aralığı */
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .order-id {{
            font-weight: bold;
            color: #333333; /* Daha koyu sipariş no */
        }}
        .footer {{
            text-align: center;
            margin-top: 30px; /* Artırılmış boşluk */
            font-size: 0.9em;
            color: #888888; /* Daha açık gri */
            border-top: 1px solid #d0d0d0; /* Daha açık çizgi */
            padding-top: 15px; /* Üst boşluk */
        }}
        .blue-bg {{ /* Yeni sınıf */
            background-color: #e3f2fd; /* Açık mavi */
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header blue-bg'>
            <img src='cid:logoImage' alt='Şevval Emlak Logo'>
            <h2>Siparişiniz Hazırlanıyor</h2>
        </div>
        <div class='message'>
             <p>Sayın <span class='order-id'>{kullaniciAdi} {kullaniciSoyadi}</span>,</p>
            <p><span class='order-id'>#{afisTalepId}</span> numaralı siparişiniz hazırlanıyor. En kısa sürede kargoya verilecektir.</p>
        </div>
        <div class='footer'>
            <p>Bu e-posta Şevval Emlak tarafından otomatik olarak gönderilmiştir.</p>
        </div>
    </div>
</body>
</html>
";
                    break;
                case "KARGOYA VERİLDİ":
                    subject = "Siparişiniz Kargoya Verildi";
                    body = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Siparişiniz Kargoya Verildi</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f0f0f0; /* Daha açık bir arka plan */
            margin: 0;
            padding: 0;
        }}
        .container {{
            background-color: #ffffff;
            max-width: 600px;
            margin: 30px auto; /* Daha fazla üst/alt boşluk */
            padding: 30px;
            border-radius: 12px; /* Daha yuvarlak kenarlar */
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1); /* Daha belirgin gölge */
             border: 1px solid #e0e0e0; /* Hafif kenar çizgisi */
        }}
        .header {{
            text-align: center;
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .header img {{
            max-width: 180px; /* Daha büyük logo */
            height: auto;
        }}
        .header h2 {{
            color: #007bff; /* Daha canlı mavi */
            margin-top: 15px; /* Üst başlık boşluğu */
            margin-bottom: 25px;
             font-size: 24px; /* Daha büyük başlık */
        }}
        .message {{
            font-size: 1.1em;
            color: #555555; /* Daha koyu metin */
            line-height: 1.7; /* Daha fazla satır aralığı */
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .order-id {{
            font-weight: bold;
            color: #333333; /* Daha koyu sipariş no */
        }}
        .footer {{
            text-align: center;
            margin-top: 30px; /* Artırılmış boşluk */
            font-size: 0.9em;
            color: #888888; /* Daha açık gri */
            border-top: 1px solid #d0d0d0; /* Daha açık çizgi */
            padding-top: 15px; /* Üst boşluk */
        }}
        .blue-bg {{ /* Yeni sınıf */
            background-color: #e3f2fd; /* Açık mavi */
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header blue-bg'>
            <img src='cid:logoImage' alt='Şevval Emlak Logo'>
            <h2>Siparişiniz Kargoya Verildi</h2>
        </div>
        <div class='message'>
            <p>Sayın <span class='order-id'>{kullaniciAdi} {kullaniciSoyadi}</span>,</p>
            <p><span class='order-id'>#{afisTalepId}</span> numaralı siparişiniz kargoya verilmiştir. En kısa sürede tarafınıza ulaşacaktır.</p>
        </div>
        <div class='footer'>
            <p>Bu e-posta Şevval Emlak tarafından otomatik olarak gönderilmiştir.</p>
        </div>
    </div>
</body>
</html>
";
                    break;
                case "TESLİM EDİLDİ":
                    subject = "Siparişiniz Teslim Edildi";
                    body = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Siparişiniz Teslim Edildi</title>
    <style>
       body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f0f0f0; /* Daha açık bir arka plan */
            margin: 0;
            padding: 0;
        }}
        .container {{
            background-color: #ffffff;
            max-width: 600px;
            margin: 30px auto; /* Daha fazla üst/alt boşluk */
            padding: 30px;
            border-radius: 12px; /* Daha yuvarlak kenarlar */
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1); /* Daha belirgin gölge */
            border: 1px solid #e0e0e0; /* Hafif kenar çizgisi */
        }}
        .header {{
            text-align: center;
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .header img {{
            max-width: 180px; /* Daha büyük logo */
            height: auto;
        }}
        .header h2 {{
            color: #007bff; /* Daha canlı mavi */
            margin-top: 15px; /* Üst başlık boşluğu */
            margin-bottom: 25px;
            font-size: 24px; /* Daha büyük başlık */
        }}
        .message {{
            font-size: 1.1em;
            color: #555555; /* Daha koyu metin */
            line-height: 1.7; /* Daha fazla satır aralığı */
            margin-bottom: 25px; /* Artırılmış boşluk */
        }}
        .order-id {{
            font-weight: bold;
            color: #333333; /* Daha koyu sipariş no */
        }}
        .contact-phone {{
            color: #007bff; /* Daha canlı mavi */
            text-decoration: none;
            font-weight: bold; /* Telefon numarası vurgulu */
        }}
        .footer {{
            text-align: center;
            margin-top: 30px; /* Artırılmış boşluk */
            font-size: 0.9em;
            color: #888888; /* Daha açık gri */
            border-top: 1px solid #d0d0d0; /* Daha açık çizgi */
            padding-top: 15px; /* Üst boşluk */
        }}
        .blue-bg {{ /* Yeni sınıf */
            background-color: #e3f2fd; /* Açık mavi */
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header blue-bg'>
            <img src='cid:logoImage' alt='Şevval Emlak Logo'>
            <h2>Siparişiniz Teslim Edildi</h2>
        </div>
        <div class='message'>
            <p>Sayın <span class='order-id'>{kullaniciAdi} {kullaniciSoyadi}</span>,</p>
            <p><span class='order-id'>#{afisTalepId}</span> numaralı siparişiniz teslim edilmiştir. Umarız memnun kalmışsınızdır. Görüşleriniz için lütfen iletişime geçin: <a href='tel:02129555541' class='contact-phone'>0212 955 55 41</a></p>
        </div>
        <div class='footer'>
            <p>Bu e-posta Şevval Emlak tarafından otomatik olarak gönderilmiştir.</p>
        </div>
    </div>
</body>
</html>
";
                    break;
                default:
                    subject = "Sipariş Durum Güncellemesi";
                    body = $"Sayın {kullaniciAdi} {kullaniciSoyadi}, siparişinizle ilgili bir güncelleme yapıldı. Yeni durum: {durum}";
                    break;
            }
            mail.Subject = subject;
            var alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(inlineLogo);
            mail.AlternateViews.Add(alternateView);

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                await client.SendMailAsync(mail);
            }
        }

        [HttpGet]
        public IActionResult MailGonder(string? email)
        {

            // Kullanıcıları ve şehirleri tek sorguda AsNoTracking ile çek
            var users = _context.Users.AsNoTracking().ToList();
            var availableCities = users
                .Where(u => !string.IsNullOrEmpty(u.City))
                .Select(u => u.City)
                .Distinct()
                .ToList();

            var model = new TumIlanlarDTO
            {
                Users = users,
                AvailableCities = availableCities,
                SelectedUserEmail=email
            };

           
            return View(model);
        }

        [HttpPost]
        public IActionResult MailGonder(string aliciEmails, string konu, string mesaj)
        {
            // Tekrarlayan kod blokları için yardımcı metot kullanılabilir.
            // Ayrıca, kullanıcı listelerini ve şehir listelerini her seferinde tekrar çekmek yerine,
            // view'e dönerken modelin bir kopyası üzerinden işlem yapılabilir.
            Action<string, bool> setViewBagAndReturnView = (message, success) =>
            {
                ViewBag.Message = message;
                ViewBag.Success = success;
                var users = _context.Users.AsNoTracking().ToList(); // Yeniden çekmek gerekebilir
                var availableCities = users
                    .Where(u => !string.IsNullOrEmpty(u.City))
                    .Select(u => u.City)
                    .Distinct()
                    .ToList();
                var model = new TumIlanlarDTO
                {
                    Users = users,
                    AvailableCities = availableCities
                };
                View(model);
            };


            if (string.IsNullOrWhiteSpace(aliciEmails))
            {
                setViewBagAndReturnView("Lütfen en az bir alıcı seçin.", false);
                return View();
            }

            // Virgülle ayrılmış e-posta adreslerini geçerli olanları filtrele
            var emailList = aliciEmails
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(email => email.Trim())
                .Where(email => IsValidEmail(email))
                .Distinct()
                .ToList();

            if (emailList.Count == 0)
            {
                setViewBagAndReturnView("Geçerli e-posta adresi bulunamadı.", false);
                return View();
            }

            try
            {
                var smtpSettings = _configuration.GetSection("Email").Get<EmailSettings>();

                if (smtpSettings == null || string.IsNullOrEmpty(smtpSettings.FromAddress) || string.IsNullOrEmpty(smtpSettings.Password))
                {
                    setViewBagAndReturnView("Mail yapılandırması eksik!", false);
                    return View();
                }

                var fromAddress = new MailAddress(smtpSettings.FromAddress, "Sevval Emlak");

                using var smtp = new SmtpClient
                {
                    Host = smtpSettings.SmtpServer,
                    Port = smtpSettings.SmtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpSettings.FromAddress, smtpSettings.Password)
                };

                foreach (var email in emailList)
                {
                    try
                    {
                        var toAddress = new MailAddress(email);
                        using var messageObj = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = konu,
                            Body = GetHtmlEmailBody(mesaj),
                            IsBodyHtml = true
                        };

                        smtp.Send(messageObj);
                    }
                    catch (Exception ex)
                    {
                        // Belirli bir kullanıcıya mail gönderilemezse diğerlerini etkilemesin
                        Console.WriteLine($"E-posta gönderme hatası ({email}): {ex.Message}");
                    }
                }

                setViewBagAndReturnView("E-postalar başarıyla gönderildi!", true);
                return View();
            }
            catch (Exception ex)
            {
                setViewBagAndReturnView($"Bir hata oluştu: {ex.Message}", false);
                return View();
            }
        }

        // E-posta adresinin geçerliliğini kontrol eden metot
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // HTML e-posta gövdesi oluşturan metot
        private string GetHtmlEmailBody(string mesaj)
        {
            return $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.4;
            color: #495057;
            background-color: #f8f9fa;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 680px;
            margin: 20px auto;
            background: #ffffff;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
            padding: 30px;
        }}
        .logo {{
            max-width: 150px;
            height: auto;
            margin-bottom: 10px;
        }}
        .header {{
            background: linear-gradient(135deg, #0558ca, #5f9dcf);
            padding: 15px;
            text-align: center;
            color: white;
            font-weight: bold;
            border-bottom: 3px solid #2980b9;
            font-size: 18px;
        }}
        .content {{
            font-size: 15px;
            margin-top: 10px;
            margin-bottom: 10px;
        }}
        .footer {{
            text-align: center;
            padding: 15px;
            font-size: 13px;
            color: #6c757d;
            border-top: 1px solid #e9ecef;
            margin-top: 10px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <img src='https://i.hizliresim.com/sw39o6d.webp' alt='Sevval Emlak Logo' class='logo' />
            <h1 style='color: #fff; margin:0; font-size: 22px;'>
                VARSA BİZDEN İYİSİ, O DA BİZDEN BİRİSİ
            </h1>
        </div>
        <div class='content'>
            {mesaj.Trim().Replace("\n", "<br>").Replace("<br><br>", "<br>")}
        </div>
        <div class='footer'>
            <p style='margin:0;'>
                © 2024 Sevval Emlak. Tüm hakları saklıdır.<br>
                Telefon: 0 (212) 955 55 41 | E-Posta: sevvaldestek@gmail.com
            </p>
        </div>
    </div>
</body>
</html>";
        }



        public class EmailSettings
        {
            public string SmtpServer { get; set; }
            public int SmtpPort { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FromAddress { get; set; }
            public string AdminAddress { get; set; }
        }


        [HttpGet]
        public IActionResult SirayaAl(int id)
        {
            var ilan = _context.IlanBilgileri.AsNoTracking().FirstOrDefault(i => i.Id == id); // Takip etmeyi kapat

            if (ilan != null)
            {
                // Son eklenen ilanı bul
                var sonIlani = _context.GununIlanlari
                    .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                    .OrderByDescending(i => i.YayinlanmaTarihi)  // Yayınlanma tarihine göre sıralama
                    .FirstOrDefault();

                DateTime yayinlanmaTarihi;

                // Eğer daha önce ilan eklenmişse, bir gün sonrasını kullan
                if (sonIlani != null)
                {
                    yayinlanmaTarihi = sonIlani.YayinlanmaTarihi.AddDays(1);
                }
                else
                {
                    // İlk ilan olduğu için, bugün tarihini kullan
                    yayinlanmaTarihi = DateTime.Now;
                }

                var gununIlan = new GununIlanModel
                {
                    Id = id,
                    Category = ilan.Category,
                    KonutDurumu = ilan.KonutDurumu,
                    MulkTipi = ilan.MulkTipi,
                    SelectedCategories = ilan.SelectedCategories,
                    Title = ilan.Title,
                    MeyveninCinsi = ilan.MeyveninCinsi,
                    Description = ilan.Description,
                    Price = ilan.Price,
                    PricePerSquareMeter = ilan.PricePerSquareMeter,
                    Aidat = ilan.Aidat,
                    TasınmazNumarasi = ilan.TasınmazNumarasi,
                    Area = ilan.Area,
                    AdaNo = ilan.AdaNo,
                    ParselNo = ilan.ParselNo,
                    PaftaNo = ilan.PaftaNo,
                    AcikAlan = ilan.AcikAlan,
                    KapaliAlan = ilan.KapaliAlan,
                    GunlukMusteriSayisi = ilan.GunlukMusteriSayisi,
                    BrutMetrekare = ilan.BrutMetrekare,
                    NetMetrekare = ilan.NetMetrekare,
                    OdaSayisi = ilan.OdaSayisi,
                    sehir = ilan.sehir,
                    semt = ilan.semt,
                    mahalleKoy = ilan.mahalleKoy,
                    YatakSayisi = ilan.YatakSayisi,
                    BinaYasi = ilan.BinaYasi,
                    KatSayisi = ilan.KatSayisi,
                    BulunduguKat = ilan.BulunduguKat,
                    Isitma = ilan.Isitma,
                    BanyoSayisi = ilan.BanyoSayisi,
                    AraziNiteliği = ilan.AraziNiteliği,
                    Balkon = ilan.Balkon,
                    Asansor = ilan.Asansor,
                    Otopark = ilan.Otopark,
                    Esyali = ilan.Esyali,
                    Takas = ilan.Takas,
                    KullanimDurumu = ilan.KullanimDurumu,
                    TapuDurumu = ilan.TapuDurumu,
                    GayrimenkulSahibi = ilan.GayrimenkulSahibi,
                    Konum = ilan.Konum,
                    VideoLink = ilan.VideoLink,
                    TKGMParselLink = ilan.TKGMParselLink,
                    IlanNo = ilan.IlanNo,
                    GirisTarihi = DateTime.Now,
                    ImarDurumu = ilan.ImarDurumu,
                    Gabari = ilan.Gabari,
                    Kaks = ilan.Kaks,
                    SerhDurumu = ilan.SerhDurumu,
                    KrediyeUygunluk = ilan.KrediyeUygunluk,
                    TakasaUygunluk = ilan.TakasaUygunluk,
                    Kimden = ilan.Kimden,
                    Latitude = ilan.Latitude,
                    Longitude = ilan.Longitude,
                    FirstName = ilan.FirstName,
                    LastName = ilan.LastName,
                    PhoneNumber = ilan.PhoneNumber,
                    Email = ilan.Email,
                    GoruntulenmeSayisi = 0, // Varsayılan olarak 0
                    GoruntulenmeTarihi = DateTime.MinValue,
                    Status = ilan.Status,
                    ProfilePicture = ilan.ProfilePicture,
                    ProfilePicturePath = ilan.ProfilePicturePath,
                    UploadedVideos = ilan.UploadedVideos,
                    UploadedFiles = ilan.UploadedFiles,
                    MulkTipiArsa = ilan.MulkTipiArsa,
                    ArsaDurumu = ilan.ArsaDurumu,
                    PatronunNotu = ilan.PatronunNotu,
                    MesajSayisi = ilan.MesajSayisi,
                    TelefonAramaSayisi = ilan.TelefonAramaSayisi,
                    FavoriSayisi = ilan.FavoriSayisi,
                    AramaTarihi = ilan.AramaTarihi,
                    LastActionDate = ilan.LastActionDate,
                    YayinlanmaTarihi = yayinlanmaTarihi  // Yayınlanma tarihini ekle
                };

                // Veritabanına ekle
                _context.GununIlanlari.Add(gununIlan);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public IActionResult UpdateYayinlanmaTarihi([FromBody] YayinlanmaTarihiUpdateModel model)
        {
            var ilan = _context.GununIlanlari.FirstOrDefault(i => i.Id == model.Id);

            if (ilan != null)
            {
                ilan.YayinlanmaTarihi = DateTime.Parse(model.YeniTarih);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public class YayinlanmaTarihiUpdateModel
        {
            public int Id { get; set; }
            public string YeniTarih { get; set; }
        }

        [HttpPost]
        public IActionResult RemoveFromList([FromBody] RemoveFromListModel model)
        {
            var ilan = _context.GununIlanlari.FirstOrDefault(i => i.Id == model.Id);

            if (ilan != null)
            {
                _context.GununIlanlari.Remove(ilan);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public class RemoveFromListModel
        {
            public int Id { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> Guncelle(string id, string userType) // id string olarak alınmalı
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserTypes = userType; // Kullanıcı tipini güncelliyoruz.
            _context.Users.Update(user);
            await _context.SaveChangesAsync(); // Veritabanını güncelliyoruz.

            return RedirectToAction("Uyelikler");
        }

        public IActionResult Dopingler()
        {
            return View();
        }

        public IActionResult SmsMesaj(string? phone)
        {
            // Kullanıcıları ve şehirleri tek sorguda AsNoTracking ile çek
            var users = _context.Users.AsNoTracking().ToList();

            var userDtos = users.Select(u => new UserSmsDTO
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.LastName}",
                Phone = u.PhoneNumber,
                City = u.City
            }).ToList();

            var availableCities = users
                .Where(u => !string.IsNullOrEmpty(u.City))
                .Select(u => u.City)
                .Distinct()
                .ToList();

            var model = new SmsDTO
            {
                Users = userDtos,
                AvailableCities = availableCities,
                SelectedPhone=phone
            };

            return View(model);
        }

        public IActionResult SmsRaporlama()
        {
            // Gönderim gruplarını JobId ve Message'a göre grupla
            var smsGroups = _context.SmsSendHistory
                .AsNoTracking() // Sadece okunacağı için takip etmeyi kapat
                .GroupBy(h => new { h.JobId, h.Message })
                .Select(g => new SmsReportDTO
                {
                    JobId = g.Key.JobId,
                    SendDate = g.Max(x => x.SendDate), // En son gönderim tarihini al
                    Message = g.Key.Message,
                    TotalCount = g.Count(),
                    SuccessCount = g.Count(s => s.Status == "success"),
                    FailedCount = g.Count(s => s.Status == "failed"),
                    Records = g.Select(r => new SmsSendHistoryDTO
                    {
                        Id = r.Id,
                        PhoneNumber = r.PhoneNumber,
                        Status = r.Status,
                        ResponseCode = r.ResponseCode,
                        ResponseDescription = r.ResponseDescription
                    }).ToList()
                })
                .OrderByDescending(g => g.SendDate) // En son gönderilenleri üstte göster
                .ToList();

            return View(smsGroups);
        }

        [HttpPost]
        public async Task<IActionResult> SmsMesaj(string aliciPhones, string mesaj)
        {
            // Tekrarlayan kod blokları için yardımcı metot
            Func<string, bool, Task<IActionResult>> createAndReturnView = async (msg, success) =>
            {
                ViewBag.Message = msg;
                ViewBag.Success = success;

                var users = await _context.Users.AsNoTracking().ToListAsync(); // AsNoTracking ekledim
                var userDtos = users.Select(u => new UserSmsDTO
                {
                    Id = u.Id,
                    Name = $"{u.FirstName} {u.LastName}",
                    Phone = u.PhoneNumber,
                    City = u.City
                }).ToList();

                var availableCities = users
                    .Where(u => !string.IsNullOrEmpty(u.City))
                    .Select(u => u.City)
                    .Distinct()
                    .ToList();

                var model = new SmsDTO
                {
                    Users = userDtos,
                    AvailableCities = availableCities
                };
                return View(model);
            };

            if (string.IsNullOrEmpty(aliciPhones) || string.IsNullOrEmpty(mesaj))
            {
                return await createAndReturnView("Lütfen alıcıları ve mesaj içeriğini doldurun.", false);
            }

            var phoneList = aliciPhones.Split(',').ToList();
            string result = "";
            try
            {
                result = await _netGsmService.SendSmsAsync(phoneList, mesaj);
            }
            catch (Exception ex)
            {
                // SMS gönderme servisinde bir hata oluşursa
                return await createAndReturnView($"SMS gönderme servisi hatası: {ex.Message}", false);
            }


            // API yanıtını kontrol et ve veritabanına kaydet
            var isSuccess = false;
            var displayMessage = "";
            var jobId = "";
            var responseCode = "";
            var responseDescription = "";

            try
            {
                // JSON yanıtını ayrıştır
                var response = JsonConvert.DeserializeObject<NetGsmResponse>(result);

                // "00" ve "17" başlangıçlı kodlar başarılı gönderim
                isSuccess = response.code == "00" || response.code.StartsWith("17");
                jobId = response.jobid;
                responseCode = response.code;
                responseDescription = response.description;

                displayMessage = isSuccess
                    ? $"SMS mesajı başarıyla gönderildi! (JobID: {response.jobid})"
                    : $"SMS gönderimi başarısız oldu: Kod: {response.code}, Açıklama: {response.description}";
            }
            catch
            {
                // JSON ayrıştırılamazsa orijinal yanıtı göster
                responseCode = result;
                responseDescription = "JSON parse error";
                isSuccess = result == "00" || result.StartsWith("17");
                displayMessage = isSuccess
                    ? "SMS mesajı başarıyla gönderildi!"
                    : $"SMS gönderimi başarısız oldu: {result}";
            }

            // Telefon numaralarının her biri için veritabanına kayıt ekle
            foreach (var phone in phoneList)
            {
                var smsHistory = new SmsSendHistory
                {
                    PhoneNumber = phone,
                    Message = mesaj,
                    SendDate = DateTime.Now,
                    JobId = jobId,
                    Status = isSuccess ? "success" : "failed",
                    ResponseCode = responseCode,
                    ResponseDescription = responseDescription
                };

                _context.SmsSendHistory.Add(smsHistory);
            }

            await _context.SaveChangesAsync();

            return await createAndReturnView(displayMessage, isSuccess);
        }

        /// <summary>
        /// 🆕 Numaraları kayıt tarihine göre oluştur
        /// ŞEVVAL EMLAK her zaman K-0001
        /// Kurumsal ve Bireysel ayrı numaralandırma
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNumbersByRegistrationDate()
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return Json(new { success = false, message = "Yetkiniz yok" });

            try
            {
                // Tüm kullanıcıları al
                var allUsers = await _context.Users.ToListAsync();
                
                // 🏆 ŞEVVAL EMLAK'ı bul (sftumen41@gmail.com)
                var sevvalUser = allUsers.FirstOrDefault(u => 
                    u.Email != null && u.Email.Equals("sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase));

                int totalProcessed = 0;

                // 📋 BİREYSEL KULLANICILAR - Kayıt tarihine göre B-0001, B-0002...
                var bireyselUsers = allUsers
                    .Where(u => u.UserTypes == "Bireysel")
                    .OrderBy(u => u.RegistrationDate)
                    .ToList();
                
                int bireyselCounter = 1;
                foreach (var user in bireyselUsers)
                {
                    user.UserOrder = bireyselCounter++;
                    _context.Users.Update(user); // Açıkça güncelleme olarak işaretle
                    totalProcessed++;
                }

                // 🏢 KURUMSAL KULLANICILAR - Kayıt tarihine göre sırala
                var kurumsalUsers = allUsers
                    .Where(u => u.UserTypes != "Bireysel")
                    .OrderBy(u => u.RegistrationDate)
                    .ToList();

                // 🔥 ÖNCE ŞEVVAL EMLAK'I İŞLE
                if (sevvalUser != null && kurumsalUsers.Contains(sevvalUser))
                {
                    // 1. ŞEVVAL EMLAK → K-0001
                    sevvalUser.UserOrder = 1;
                    _context.Users.Update(sevvalUser); // Açıkça güncelleme olarak işaretle
                    totalProcessed++;
                    _logger.LogInformation("✅ ŞEVVAL EMLAK (ID: {Id}) → K-0001 atandı (Kayıt: {Date})", 
                        sevvalUser.Id, sevvalUser.RegistrationDate.ToString("dd.MM.yyyy HH:mm"));
                    
                    // 2. DİĞER KURUMSAL KULLANICILAR - ŞEVVAL EMLAK HARİÇ
                    int kurumsalCounter = 2;
                    foreach (var user in kurumsalUsers.Where(u => u.Id != sevvalUser.Id))
                    {
                        user.UserOrder = kurumsalCounter;
                        _context.Users.Update(user); // Açıkça güncelleme olarak işaretle
                        _logger.LogInformation("   → {Email} → K-{Number:D4} (Kayıt: {Date})", 
                            user.Email, kurumsalCounter, user.RegistrationDate.ToString("dd.MM.yyyy HH:mm"));
                        kurumsalCounter++;
                        totalProcessed++;
                    }
                }
                else
                {
                    // ŞEVVAL EMLAK YOKSA veya Bireysel ise - Normal sıralama
                    _logger.LogWarning("⚠️ ŞEVVAL EMLAK (sftumen41@gmail.com) kurumsal kullanıcı olarak bulunamadı!");
                    
                    int kurumsalCounter = 1;
                    foreach (var user in kurumsalUsers)
                    {
                        user.UserOrder = kurumsalCounter++;
                        _context.Users.Update(user); // Açıkça güncelleme olarak işaretle
                        totalProcessed++;
                    }
                }

                // 💾 Değişiklikleri veritabanına kaydet
                var savedCount = await _context.SaveChangesAsync();
                _logger.LogInformation("💾 Database'e {SavedCount} değişiklik kaydedildi", savedCount);

                _logger.LogInformation("✅ CreateNumbersByRegistrationDate: {Count} kullanıcı numaralandırıldı", totalProcessed);

                // Mesaj oluştur
                var kurumsalCount = kurumsalUsers.Count;
                var bireyselCount = bireyselUsers.Count;
                
                var message = sevvalUser != null && kurumsalUsers.Contains(sevvalUser)
                    ? $"✅ Numaralandırma tamamlandı!\n\n🏆 ŞEVVAL EMLAK → K-0001\n📊 Kurumsal: {kurumsalCount} kullanıcı (K-0001 - K-{kurumsalCount:D4})\n📊 Bireysel: {bireyselCount} kullanıcı (B-0001 - B-{bireyselCount:D4})\n\n🔄 Toplam: {totalProcessed} kullanıcı"
                    : $"✅ Numaralandırma tamamlandı!\n\n📊 Kurumsal: {kurumsalCount} kullanıcı\n📊 Bireysel: {bireyselCount} kullanıcı\n\n🔄 Toplam: {totalProcessed} kullanıcı";

                return Json(new 
                { 
                    success = true, 
                    message = message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateNumbersByRegistrationDate: Hata oluştu");
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #region Video Onay Yönetimi

        /// <summary>
        /// Bekleyen videoları listeler (Super Admin only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BekleyenVideolar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var pendingVideos = await _context.VideolarSayfasi
                .Include(v => v.YukleyenKullanici)
                .Where(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Pending)
                .OrderByDescending(v => v.YuklenmeTarihi)
                .Select(v => new Sevval.Domain.Entities.PendingVideoViewModel
                {
                    Id = v.Id,
                    VideoAdi = v.VideoAdi,
                    KapakFotografiYolu = v.KapakFotografiYolu,
                    UploaderName = v.YukleyenKullanici != null 
                        ? $"{v.YukleyenKullanici.FirstName} {v.YukleyenKullanici.LastName}" 
                        : "Bilinmiyor",
                    UploaderEmail = v.YukleyenKullanici != null ? v.YukleyenKullanici.Email : "",
                    YuklenmeTarihi = v.YuklenmeTarihi,
                    Kategori = v.Kategori ?? ""
                })
                .ToListAsync();

            ViewBag.PendingCount = pendingVideos.Count;
            return View(pendingVideos);
        }

        /// <summary>
        /// Bekleyen video sayısını JSON olarak döndürür (badge için)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPendingVideoCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { count = 0 });
            }

            var count = await _context.VideolarSayfasi
                .CountAsync(v => v.ApprovalStatus == Sevval.Domain.Enums.VideoApprovalStatus.Pending);

            return Json(new { count });
        }

        /// <summary>
        /// Videoyu onaylar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVideo(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Yetkiniz yok." });
            }

            var video = await _context.VideolarSayfasi.FindAsync(id);
            if (video == null)
            {
                return Json(new { success = false, message = "Video bulunamadı." });
            }

            if (video.ApprovalStatus != Sevval.Domain.Enums.VideoApprovalStatus.Pending)
            {
                return Json(new { success = false, message = "Bu video zaten işlenmiş." });
            }

            video.ApprovalStatus = Sevval.Domain.Enums.VideoApprovalStatus.Approved;
            video.ApprovalDate = DateTime.UtcNow;
            video.ApprovedByUserId = user.Id;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Video onaylandı ve yayına alındı." });
        }

        /// <summary>
        /// Videoyu reddeder
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectVideo(int id, string? reason)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Yetkiniz yok." });
            }

            var video = await _context.VideolarSayfasi.FindAsync(id);
            if (video == null)
            {
                return Json(new { success = false, message = "Video bulunamadı." });
            }

            if (video.ApprovalStatus != Sevval.Domain.Enums.VideoApprovalStatus.Pending)
            {
                return Json(new { success = false, message = "Bu video zaten işlenmiş." });
            }

            video.ApprovalStatus = Sevval.Domain.Enums.VideoApprovalStatus.Rejected;
            video.ApprovalDate = DateTime.UtcNow;
            video.ApprovedByUserId = user.Id;
            video.RejectionReason = reason;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Video reddedildi." });
        }

        #endregion

    }
}