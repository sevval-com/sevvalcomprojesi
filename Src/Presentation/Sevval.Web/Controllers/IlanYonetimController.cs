using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using sevvalemlak.Dto;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace sevvalemlak.Controllers
{
    public class IlanYonetimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IlanYonetimController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Yetki kontrolü (Orijinal koddan korundu)
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
                    ContentType = "text/html; charset=utf-8",
                    StatusCode = 403,
                    Content = @"<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Yetkisiz Erişim</title><link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css' rel='stylesheet'></head><body class='d-flex align-items-center justify-content-center vh-100 bg-light'><div class='text-center'><h1 class='display-1 fw-bold text-primary'>403</h1><p class='fs-3'> <span class='text-danger'>Erişim Reddedildi!</span></p><p class='lead'>Bu sayfayı görüntüleme yetkiniz yok.</p><a href='/' class='btn btn-primary'>Ana Sayfaya Dön</a></div></body></html>"
                };
            }

            return null;
        }

        public async Task<IActionResult> Index(
            string ilanNo,
            string firmaId,
            string danismanEmail,
            string sehir,
            string ilce,
            string mahalle, // Yeni
            string ilanSahibiArama,
            string baslikArama, // Yeni
            decimal? minFiyat, // Yeni
            decimal? maxFiyat, // Yeni
            double? minMetrekare, // Yeni
            double? maxMetrekare, // Yeni
            int page = 1)
        {
            var authorizationResult = await CheckUserAuthorization();
            if (authorizationResult != null) return authorizationResult;

            int pageSize = 20;
            var query = _context.IlanBilgileri.AsNoTracking();

            // --- FİLTRELEME İŞLEMLERİ ---

            // 1. İlan No
            if (!string.IsNullOrEmpty(ilanNo) && int.TryParse(ilanNo, out int id))
            {
                query = query.Where(i => i.Id == id);
            }

            // 2. Başlık Arama (SQLite Case-Insensitive Çözümü)
            // SQLite'da ToLower() kullanarak karşılaştırma yapıyoruz.
            if (!string.IsNullOrEmpty(baslikArama))
            {
                string aranan = baslikArama.ToLower();
                query = query.Where(i => i.Title.ToLower().Contains(aranan));
            }

            // 3. Konum Filtreleri
            if (!string.IsNullOrEmpty(sehir))
            {
                query = query.Where(i => i.sehir == sehir);
            }
            if (!string.IsNullOrEmpty(ilce))
            {
                query = query.Where(i => i.semt == ilce);
            }
            if (!string.IsNullOrEmpty(mahalle))
            {
                query = query.Where(i => i.mahalleKoy == mahalle);
            }

            // 4. Fiyat Aralığı
            if (minFiyat.HasValue)
            {
                query = query.Where(i => i.Price >= minFiyat.Value);
            }
            if (maxFiyat.HasValue)
            {
                query = query.Where(i => i.Price <= maxFiyat.Value);
            }

            // 5. Metrekare Aralığı
            if (minMetrekare.HasValue)
            {
                query = query.Where(i => i.Area >= minMetrekare.Value);
            }
            if (maxMetrekare.HasValue)
            {
                query = query.Where(i => i.Area <= maxMetrekare.Value);
            }

            // 6. Danışman Filtresi
            if (!string.IsNullOrEmpty(danismanEmail))
            {
                query = query.Where(i => i.Email == danismanEmail);
            }

            // 7. Firma ve İlan Sahibi Arama (Orijinal Mantık + Case Insensitive)
            if (!string.IsNullOrEmpty(firmaId) || !string.IsNullOrEmpty(ilanSahibiArama))
            {
                var userQuery = _context.Users.AsNoTracking();

                if (!string.IsNullOrEmpty(ilanSahibiArama))
                {
                    string arananSahip = ilanSahibiArama.ToLower();
                    // İsim Soyisim veya Şirket Adı içinde arama
                    userQuery = userQuery.Where(u =>
                        (u.FirstName + " " + u.LastName).ToLower().Contains(arananSahip) ||
                        u.CompanyName.ToLower().Contains(arananSahip));
                }

                if (!string.IsNullOrEmpty(firmaId))
                {
                    // Seçilen firma ID'si kendisi olabilir veya bu firma tarafından davet edilmiş bir danışman olabilir
                    userQuery = userQuery.Where(u => u.Id == firmaId || _context.ConsultantInvitations.Any(ci => ci.Email == u.Email && ci.InvitedBy == firmaId));
                }

                // Filtrelenen kullanıcıların e-postalarını alıp ilanları buna göre filtreliyoruz
                var filteredUserEmails = await userQuery.Select(u => u.Email).ToListAsync();
                query = query.Where(i => filteredUserEmails.Contains(i.Email));
            }


            // --- VERİ ÇEKME VE SAYFALAMA ---
            var totalCount = await query.CountAsync();
            var ilanlarRaw = await query.OrderByDescending(i => i.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // İlişkili verileri (Fotoğraf, Kullanıcı, Firma Bilgisi) toplu çekme
            var ilanIds = ilanlarRaw.Select(i => i.Id).ToList();
            var ilanEmails = ilanlarRaw.Select(i => i.Email).Distinct().ToList();

            var fotograflar = await _context.Photos.AsNoTracking()
                .Where(p => ilanIds.Contains(p.IlanId))
                .GroupBy(p => p.IlanId)
                .Select(g => g.FirstOrDefault())
                .ToDictionaryAsync(p => p.IlanId);

            var ilanSahipleri = await _context.Users.AsNoTracking()
                .Where(u => ilanEmails.Contains(u.Email))
                .ToDictionaryAsync(u => u.Email);

            // Danışmanların bağlı olduğu firmaları bulma
            var danismanFirmaMap = await _context.ConsultantInvitations.AsNoTracking()
                .Where(ci => ilanEmails.Contains(ci.Email))
                .Join(_context.Users.AsNoTracking(), ci => ci.InvitedBy, owner => owner.Id, (ci, owner) => new { ci.Email, owner.CompanyName })
                .Distinct()
                .ToDictionaryAsync(x => x.Email, x => x.CompanyName);

            var ilanlarList = new List<IlanYonetimItem>();
            foreach (var ilan in ilanlarRaw)
            {
                ilanSahipleri.TryGetValue(ilan.Email, out var sahip);
                string firmaAdi = null;
                if (sahip != null)
                {
                    // Eğer kullanıcı danışmansa, davet eden firmanın adını al, değilse kendi şirket adını kullan
                    firmaAdi = sahip.IsConsultant ? danismanFirmaMap.GetValueOrDefault(sahip.Email) : sahip.CompanyName;
                }

                ilanlarList.Add(new IlanYonetimItem
                {
                    Ilan = ilan,
                    IlanSahibi = sahip,
                    FirmaAdi = firmaAdi ?? "Bireysel / Bilinmiyor",
                    VitrinFotografi = fotograflar.GetValueOrDefault(ilan.Id)
                });
            }

            var model = new IlanYonetimDTO
            {
                Ilanlar = ilanlarList,
                Firmalar = await _context.Users.AsNoTracking().Where(u => !u.IsConsultant).ToListAsync(),
                Danismanlar = await _context.Users.AsNoTracking().ToListAsync(),

                // Form verilerini view'a geri taşıyoruz
                IlanNo = ilanNo,
                FirmaId = firmaId,
                DanismanEmail = danismanEmail,
                Sehir = sehir,
                Ilce = ilce,
                Mahalle = mahalle,
                IlanSahibiArama = ilanSahibiArama,
                BaslikArama = baslikArama,
                MinFiyat = minFiyat,
                MaxFiyat = maxFiyat,
                MinMetrekare = minMetrekare,
                MaxMetrekare = maxMetrekare,

                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }

        // --- YARDIMCI METOTLAR (Orijinal koddan korundu) ---

        [HttpGet]
        public async Task<IActionResult> GetFirmalar()
        {
            var firmalar = await _context.Users.AsNoTracking()
                .Where(u => !u.IsConsultant)
                .Select(u => new { id = u.Id, name = u.CompanyName ?? (u.FirstName + " " + u.LastName) })
                .ToListAsync();
            return Json(firmalar);
        }

        [HttpGet]
        public async Task<IActionResult> GetDanismanlarForFirma(string firmaId)
        {
            var company = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == firmaId);
            if (company == null) return NotFound();

            var consultantEmails = await _context.ConsultantInvitations.AsNoTracking()
                .Where(ci => ci.InvitedBy == company.Id)
                .Select(ci => ci.Email)
                .ToListAsync();

            var danismanlar = await _context.Users.AsNoTracking()
                .Where(u => consultantEmails.Contains(u.Email))
                .Select(u => new { email = u.Email, name = u.FirstName + " " + u.LastName })
                .ToListAsync();

            danismanlar.Insert(0, new { email = company.Email, name = company.FirstName + " " + company.LastName + " (Firma Sahibi)" });

            return Json(danismanlar);
        }

        [HttpPost]
        public async Task<IActionResult> Ata(int ilanId, string danismanEmail)
        {
            var ilan = await _context.IlanBilgileri.FirstOrDefaultAsync(i => i.Id == ilanId);
            if (ilan == null) return Json(new { success = false, message = "İlan bulunamadı." });

            var danisman = await _context.Users.FirstOrDefaultAsync(u => u.Email == danismanEmail);
            if (danisman == null) return Json(new { success = false, message = "Danışman bulunamadı." });

            ilan.Email = danisman.Email;
            ilan.FirstName = danisman.FirstName;
            ilan.LastName = danisman.LastName;
            ilan.PhoneNumber = danisman.PhoneNumber;
            ilan.LastActionDate = System.DateTime.Now;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "İlan başarıyla atandı." });
        }

        [HttpPost]
        public async Task<IActionResult> Sil(int ilanId)
        {
            if (ilanId <= 0) return Json(new { success = false, message = "Geçersiz İlan ID." });
            try
            {
                var ilan = await _context.IlanBilgileri.FirstOrDefaultAsync(i => i.Id == ilanId);
                if (ilan != null)
                {
                    _context.IlanBilgileri.Remove(ilan);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "İlan başarıyla silindi." });
                }
                return Json(new { success = false, message = "Silinecek ilan bulunamadı." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "İlan silinirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TopluSil([FromForm] List<int> ilanIds)
        {
            if (ilanIds == null || !ilanIds.Any()) return Json(new { success = false, message = "Silinecek ilan seçilmedi." });
            try
            {
                var ilanlar = await _context.IlanBilgileri.Where(i => ilanIds.Contains(i.Id)).ToListAsync();
                if (ilanlar.Any())
                {
                    var photos = await _context.Photos.Where(p => ilanIds.Contains(p.IlanId)).ToListAsync();
                    var videos = await _context.Videos.Where(v => ilanIds.Contains(v.IlanId)).ToListAsync();
                    if (photos.Any()) _context.Photos.RemoveRange(photos);
                    if (videos.Any()) _context.Videos.RemoveRange(videos);
                    _context.IlanBilgileri.RemoveRange(ilanlar);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = $"{ilanlar.Count} adet ilan başarıyla silindi." });
                }
                return Json(new { success = false, message = "Silinecek ilan bulunamadı." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "İlanlar silinirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TopluAta([FromForm] List<int> ilanIds, [FromForm] string danismanEmail)
        {
            if (ilanIds == null || !ilanIds.Any() || string.IsNullOrEmpty(danismanEmail)) return Json(new { success = false, message = "Gerekli bilgiler eksik." });
            try
            {
                var danisman = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == danismanEmail);
                if (danisman == null) return Json(new { success = false, message = "Danışman bulunamadı." });
                var ilanlar = await _context.IlanBilgileri.Where(i => ilanIds.Contains(i.Id)).ToListAsync();
                foreach (var ilan in ilanlar)
                {
                    ilan.Email = danisman.Email;
                    ilan.FirstName = danisman.FirstName;
                    ilan.LastName = danisman.LastName;
                    ilan.PhoneNumber = danisman.PhoneNumber;
                    ilan.LastActionDate = DateTime.Now;
                }
                _context.IlanBilgileri.UpdateRange(ilanlar);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"{ilanlar.Count} adet ilan başarıyla '{danisman.FirstName} {danisman.LastName}' adlı danışmana atandı." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "İlanlar atanırken bir hata oluştu." });
            }
        }
    }
}