using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // To use EF Core features
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;


namespace EmlakSitesi.Controllers
{
    public class SatinalController : Controller
    {
        private readonly ApplicationDbContext _context; // Veri bağlamı için alan
        private readonly IConfiguration _config;

        // Bağımlılık enjeksiyonu ile ApplicationDbContext alınır
        public SatinalController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: /Satinal/
        public IActionResult Index()
        {
            // Sayfa için gerekli model verilerini burada işleyebilirsiniz.
            return View();
        }

        public IActionResult Siparislerim()
        {
            // Kullanıcının e-posta adresini al.  Bu kısım kimlik doğrulama sisteminize göre değişebilir.
            // Örneğin, ASP.NET Identity kullanıyorsanız:
            string kullaniciEmail = User.Identity.Name; // veya User.FindFirst(ClaimTypes.Email)?.Value;

            // Veritabanından, kullanıcının e-posta adresiyle eşleşen AfisTalep kayıtlarını al.
            var afisTalepleri = _context.AfisTalepler
                .Where(at => at.Email == kullaniciEmail)
                .ToList();

            // Siparislerim view'ine filtrelenmiş listeyi gönder.
            return View(afisTalepleri);
        }



        // GET: AfisTalepGonder
        public ActionResult AfisTalepGonder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAfisTalep(AfisTalep model)
        {
            // Email alanındaki model binding hatasını kaldırıyoruz
            ModelState.Remove(nameof(model.Email));

            if (!ModelState.IsValid)
            {
                return View("AfisTalepGonder", model);
            }

            // 1) Modeli doldur ve veritabanına kayıt
            model.TalepTarihi = DateTime.Now;
            model.Email = User.Identity.Name; // Giriş yapan kullanıcının e-maili
            _context.AfisTalepler.Add(model);
            await _context.SaveChangesAsync();

            // Afiş Talep ID'sini al
            int afisTalepId = model.Id;

            // 2) Kullanıcının açık adresini al
            var appUser = await _context.Users
                .Where(u => u.Email == model.Email)
                .Select(u => new { u.AcikAdres, u.Email, u.FirstName, u.LastName }) // Kullanıcının emailini, adını ve soyadını da al
                .SingleOrDefaultAsync();

            string gonderimAdres = appUser?.AcikAdres ?? "Adres bilgisi bulunamadı";
            string kullaniciEmail = appUser?.Email; // Kullanıcının emailini değişkene atadık
            string kullaniciAdi = appUser?.FirstName;
            string kullaniciSoyadi = appUser?.LastName;
            // 3) E-posta gönderimi
            try
            {
                // Ayarları çek
                var smtp = _config.GetSection("Email");
                string smtpServer = smtp["SmtpServer"];
                int smtpPort = int.Parse(smtp["SmtpPort"]);
                string smtpUser = smtp["Username"];
                string smtpPass = smtp["Password"];
                string fromAddress = smtp["FromAddress"];
                string adminAddress = smtp["AdminAddress"]; // Admin mail adresi
                string siteName = "sevvalemlak.com.tr";

                // Logo dosya yolu
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "favlogo.png");

                // Mail mesajı oluştur (Admin için olan)
                var adminMail = new MailMessage();
                adminMail.From = new MailAddress(fromAddress, siteName);
                adminMail.To.Add(adminAddress); // Admin adresine gönderilecek
                adminMail.Subject = "Yeni Afiş Talep Formu";
                adminMail.IsBodyHtml = true;

                // Logo’yu embed et (Admin mail için)
                var inlineLogoAdmin = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "logoImage"
                };

                // HTML içeriği (Admin mail için)
                string adminHtmlBody = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>YENİ AFİŞ TALEBİ</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f4f6f9;
            margin: 0; padding: 0;
        }}
        .container {{
            background: #fff;
            max-width: 650px;
            margin: 40px auto;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }}
        .header {{
            flex-direction: column;
            align-items: center;
            margin-bottom: 30px;
             background-color: #1a237e;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }}
        .header .logo img {{
            max-height: 80px;
            width: auto;
            border-radius: 8px;
            margin-bottom: 12px;
        }}
        .header h2 {{
            margin: 0;
            color: #fff;
            font-size: 1.6em;
            font-weight: bold;
            text-transform: uppercase;
             line-height: 1.2;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
            overflow: hidden;
        }}
        th, td {{
            padding: 12px 15px;
            border-bottom: 1px solid #eee;
            text-align: left;
             text-transform: uppercase;
            font-size: 0.9em;
        }}
        th {{
            background-color: #1a237e;
            color: #fff;
            font-weight: bold;
        }}
        tr:nth-child(even) {{
            background-color: #f9f9f9;
        }}
         tr:last-child td {{
            border-bottom: none;
        }}
        .highlight {{
            font-weight: bold;
            color: #0d47a1;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            font-size: 0.9em;
            color: #888;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
        <img src='cid:logoImage' alt='Şevval Emlak Logo' style=""display: block; margin: 0 auto 10px auto; max-height: 80px;"" />
            <h2 style=""margin: 0; color: #fff; font-size: 1.6em; font-weight: bold; text-transform: uppercase;"">YENİ AFİŞ TALEBİ</h2>
        </div>
        <table>
            <tr><th>Alan</th><th>Değer</th></tr>
            <tr><td>Sipariş Kodu</td><td class='highlight'>#{afisTalepId}</td></tr>
            <tr><td>İlan Türü</td><td class='highlight'>{model.IlanTuru}</td></tr>
            <tr><td>Telefon</td><td class='highlight'>{model.Telefon}</td></tr>
            <tr><td>İsim Soyad</td><td class='highlight'>{model.IsimSoyad}</td></tr>
            <tr><td>Firma</td><td class='highlight'>{model.Firma}</td></tr>
            <tr><td>E-posta (Kullanıcı)</td><td class='highlight'>{model.Email}</td></tr>
            <tr><td>Gönderim Adresi</td><td class='highlight'>{gonderimAdres}</td></tr>
            <tr><td>Talep Tarihi</td><td class='highlight'>{model.TalepTarihi:dd.MM.yyyy HH:mm}</td></tr>
        </table>
        <div class='footer'>
            <p>Bu e-posta <strong>Şevval Emlak</strong> sistemi tarafından otomatik olarak gönderilmiştir.</p>
        </div>
    </div>
</body>
</html>
";
                var adminAlternateView = AlternateView.CreateAlternateViewFromString(adminHtmlBody, null, MediaTypeNames.Text.Html);
                adminAlternateView.LinkedResources.Add(inlineLogoAdmin);
                adminMail.AlternateViews.Add(adminAlternateView);


                // Mail mesajı oluştur (Kullanıcı için olan)
                var userMail = new MailMessage();
                userMail.From = new MailAddress(fromAddress, siteName);
                userMail.To.Add(kullaniciEmail); // Kullanıcının kendi mail adresine gönderilecek
                userMail.Subject = "Afiş Talep Onayı"; // Konu başlığı
                userMail.IsBodyHtml = true;

                // Logo’yu embed et (Kullanıcı mail için)
                var inlineLogoUser = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "logoImage"
                };

                // HTML içeriği (Kullanıcı mail için)
                string userHtmlBody = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Sipariş Onayı</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f4f6f9;
            margin: 0; padding: 0;
        }}
        .container {{
            background: #fff;
            max-width: 650px;
            margin: 40px auto;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }}
        .header {{
            flex-direction: column;
            align-items: center;
            margin-bottom: 30px;
            background-color: #1a237e;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }}
        .header .logo img {{
            max-height: 80px;
            width: auto;
            border-radius: 8px;
            margin-bottom: 12px;
        }}
        .header h2 {{
            margin: 0;
            color: #fff;
            font-size: 1.6em;
            font-weight: bold;
            text-transform: uppercase;
            line-height: 1.2;
        }}
        .order-info {{
            margin-top: 20px;
            padding: 15px;
            background-color: #e3f2fd;
            border-radius: 8px;
            border: 1px solid #b0e0e6;
            text-align: center;
        }}
        .order-info p {{
            font-size: 1.1em;
            color: #0d47a1;
            font-weight: bold;
        }}
        .details {{
            margin-top: 30px;
            font-size: 1.1em;
            color: #555;
            line-height: 1.6;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            font-size: 0.9em;
            color: #888;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='cid:logoImage' alt='Şevval Emlak Logo' style=""display: block; margin: 0 auto 10px auto; max-height: 80px;"" />
            <h2 style=""margin: 0; color: #fff; font-size: 1.6em; font-weight: bold; text-transform: uppercase;"">SİPARİŞ ONAYI</h2>
        </div>
        <div class='order-info'>
            <p>Siparişiniz başarıyla oluşturulmuştur. Sipariş Kodunuz: <span style='color:#d32f2f;'>#{afisTalepId}</span></p>
        </div>
        <div class='details'>
            <p>Sayın <span style='font-weight:bold;'>{kullaniciAdi} {kullaniciSoyadi}</span>, afiş talebiniz alınmıştır ve en kısa sürede işleme alınacaktır.  </p>
            <p>Gönderim adresiniz: <span style='font-weight:bold;'>{gonderimAdres}</span> olarak kaydedilmiştir.</p>
            <p>Talebinizle ilgili detaylar aşağıdaki gibidir:</p>
            <ul>
                <li>İlan Türü: <span style='font-weight:bold;'>{model.IlanTuru}</span></li>
                <li>İsim Soyad: <span style='font-weight:bold;'>{model.IsimSoyad}</span></li>
                <li>Firma: <span style='font-weight:bold;'>{model.Firma}</span></li>
                <li>Telefon: <span style='font-weight:bold;'>{model.Telefon}</span></li>
                <li>Talep Tarihi:  <span style='font-weight:bold;'>{model.TalepTarihi:dd.MM.yyyy HH:mm}</span></li>
            </ul>
        </div>
        <div class='footer'>
            <p>Bu e-posta <strong>Şevval Emlak</strong> sistemi tarafından otomatik olarak gönderilmiştir.</p>
        </div>
    </div>
</body>
</html>
";

                var userAlternateView = AlternateView.CreateAlternateViewFromString(userHtmlBody, null, MediaTypeNames.Text.Html);
                userAlternateView.LinkedResources.Add(inlineLogoUser);
                userMail.AlternateViews.Add(userAlternateView);


                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    await client.SendMailAsync(adminMail); // Önce admin'e mail gönder
                    await client.SendMailAsync(userMail); // Sonra kullanıcıya mail gönder
                }
            }
            catch (Exception ex)
            {
                // Hata yönetimi, loglama vb. ekleyebilirsiniz
            }

            return RedirectToAction("TalepBasarili");
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAcikAdres(string newAdres)
        {
            // Oturum açmış kullanıcının email bilgisini kullanarak kaydı çekiyoruz
            var userEmail = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user != null)
            {
                user.AcikAdres = newAdres;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }



        // GET: TalepBasarili
        public ActionResult TalepBasarili()
        {
            return View();
        }


        public async Task<ActionResult> AddToCart([FromBody] CartRequest request)
        {
            if (request == null)
            {
                return Json(new { success = false, message = "Geçersiz veri!" });
            }

            var userEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Json(new { success = false, message = "Kullanıcı oturum açmamış!" });

            int ilanId = request.ilanId;

            // İlanı veritabanından alıyoruz
            var ilan = await _context.IlanBilgileri.FirstOrDefaultAsync(i => i.Id == ilanId);
            if (ilan == null)
            {
                return Json(new { success = false, message = "İlan bulunamadı." });
            }

            // Aynı ürünün sepette olup olmadığını kontrol et
            var mevcutItem = await _context.Sepet.FirstOrDefaultAsync(s => s.UserEmail == userEmail && s.IlanId == ilanId);
            if (mevcutItem != null)
            {
                return Json(new { success = false, message = "Ürün zaten sepette." });
            }

            var sepetItem = new Sepet
            {
                UserEmail = userEmail,
                IlanId = ilanId,
                UrunAdi = request.urunAdi,
                Adet = request.adet,
                Fiyat = request.Fiyat,  // İlanın fiyatını sepete ekliyoruz
                CreatedAt = DateTime.Now
            };

            _context.Sepet.Add(sepetItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Ürün sepete eklendi." });
        }



        public async Task<JsonResult> GetCartItemCount()
        {
            var userEmail = User.Identity.Name; // Oturum açan kullanıcının e-posta adresini alıyoruz
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "Kullanıcı oturum açmamış!" });
            }

            // Sepetteki ürünleri sayıyoruz
            var cartItemCount = await _context.Sepet
                .Where(s => s.UserEmail == userEmail)
                .SumAsync(s => s.Adet); // Adetleri topluyoruz

            return Json(new { success = true, itemCount = cartItemCount });
        }



        // JSON verisinin model binding ile alınabilmesi için DTO sınıfı
        public class CartRequest
        {
            public int ilanId { get; set; }
            public string urunAdi { get; set; }
            public decimal Fiyat { get; set; }
            public int adet { get; set; }
        }

        public async Task<IActionResult> Sepet()
        {
            var userEmail = User.Identity.Name; // Oturum açan kullanıcının e-posta adresini alıyoruz
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account"); // Kullanıcı oturum açmamışsa login sayfasına yönlendir
            }

            // Kullanıcıya ait sepet ürünlerini çekiyoruz
            var sepetListesi = await _context.Sepet
                .Where(s => s.UserEmail == userEmail)
                .ToListAsync();

            // Eğer sepet boşsa, kullanıcıya bilgi verelim
            if (sepetListesi.Count == 0)
            {
                ViewBag.Message = "Sepetiniz şu anda boş.";
            }

            // Sepet verisini View'a gönderiyoruz
            return View(sepetListesi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateCart(int id, int adet)
        {
            var sepetItem = await _context.Sepet.FindAsync(id);
            if (sepetItem == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı" });
            }

            sepetItem.Adet = adet;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoveFromCart(int id)
        {
            var sepetItem = await _context.Sepet.FindAsync(id);
            if (sepetItem == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı" });
            }

            _context.Sepet.Remove(sepetItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ClearCart()
        {
            // Eğer sepet öğeleriniz kullanıcıya özel ise burada kullanıcıya göre filtreleme yapmalısınız.
            var cartItems = _context.Sepet.ToList();
            if (cartItems == null || !cartItems.Any())
            {
                return Json(new { success = false, message = "Sepet zaten boş." });
            }

            _context.Sepet.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // Yeni eklenen aksiyon metodu
        public ActionResult IlanHizmetleri()
        {
            // Gerekli verileri model olarak ekleyebilir veya doğrudan view dönebilirsiniz.
            return View();
        }

        public IActionResult Promosyonlar()
        {
            return View();
        }

        public async Task<IActionResult> GununIlaniTalep()
        {
            var userEmail = User.Identity.Name; // Oturum açan kullanıcının emaili
            if (string.IsNullOrEmpty(userEmail))
                return Json(new { success = false, message = "Kullanıcı oturum açmamış!" });

            var ilanlar = await _context.IlanBilgileri
                .Where(i => i.Email == userEmail && i.Status == "active")
                .Select(i => new { i.Id, i.Title })
                .ToListAsync();

            // View ile ilanları gönderiyoruz
            return View(ilanlar); // Iletilen ilanlar burada View'a aktarılacak
        }

        [HttpPost]
        public async Task<IActionResult> TalepEkle([FromBody] dynamic data)
        {
            try
            {
                string ilanId = data.ilanId;
                if (string.IsNullOrEmpty(ilanId))
                    return Json(new { success = false, message = "İlan seçilmedi!" });

                // Seçilen ilan bilgilerini al
                var selectedIlan = await _context.IlanBilgileri.FirstOrDefaultAsync(i => i.Id.ToString() == ilanId);
                if (selectedIlan == null)
                    return Json(new { success = false, message = "İlan bulunamadı!" });

                // Yeni talep oluştur
                var yeniTalep = new GununIlaniTalep
                {
                    Category = selectedIlan.Category ?? string.Empty,
                    KonutDurumu = selectedIlan.KonutDurumu ?? string.Empty,
                    MulkTipi = selectedIlan.MulkTipi ?? string.Empty,
                    SelectedCategories = selectedIlan.SelectedCategories ?? string.Empty,
                    Title = selectedIlan.Title ?? string.Empty,
                    MeyveninCinsi = selectedIlan.MeyveninCinsi ?? string.Empty,
                    Description = selectedIlan.Description ?? string.Empty,
                    Price = selectedIlan.Price > 0 ? selectedIlan.Price : 0,
                    PricePerSquareMeter = selectedIlan.PricePerSquareMeter > 0 ? selectedIlan.PricePerSquareMeter : 0,
                    Aidat = selectedIlan.Aidat > 0 ? selectedIlan.Aidat : 0,
                    TasınmazNumarasi = selectedIlan.TasınmazNumarasi > 0 ? selectedIlan.TasınmazNumarasi : 0,
                    Area = selectedIlan.Area > 0 ? selectedIlan.Area : 0,
                    AdaNo = selectedIlan.AdaNo ?? string.Empty,
                    ParselNo = selectedIlan.ParselNo ?? string.Empty,
                    PaftaNo = selectedIlan.PaftaNo ?? string.Empty,
                    AcikAlan = selectedIlan.AcikAlan ?? string.Empty,
                    KapaliAlan = selectedIlan.KapaliAlan ?? string.Empty,
                    GunlukMusteriSayisi = selectedIlan.GunlukMusteriSayisi ?? string.Empty,
                    BrutMetrekare = selectedIlan.BrutMetrekare ?? string.Empty,
                    NetMetrekare = selectedIlan.NetMetrekare ?? string.Empty,
                    OdaSayisi = selectedIlan.OdaSayisi ?? string.Empty,
                    sehir = selectedIlan.sehir ?? string.Empty,
                    semt = selectedIlan.semt ?? string.Empty,
                    mahalleKoy = selectedIlan.mahalleKoy ?? string.Empty,
                    YatakSayisi = selectedIlan.YatakSayisi ?? string.Empty,
                    BinaYasi = selectedIlan.BinaYasi ?? string.Empty,
                    KatSayisi = selectedIlan.KatSayisi ?? string.Empty,
                    BulunduguKat = selectedIlan.BulunduguKat ?? string.Empty,
                    Isitma = selectedIlan.Isitma ?? string.Empty,
                    BanyoSayisi = selectedIlan.BanyoSayisi ?? string.Empty,
                    AraziNiteliği = selectedIlan.AraziNiteliği ?? string.Empty,
                    Balkon = selectedIlan.Balkon ?? string.Empty,
                    Asansor = selectedIlan.Asansor ?? string.Empty,
                    Otopark = selectedIlan.Otopark ?? string.Empty,
                    Esyali = selectedIlan.Esyali ?? string.Empty,
                    Takas = selectedIlan.Takas ?? string.Empty,
                    KullanimDurumu = selectedIlan.KullanimDurumu ?? string.Empty,
                    TapuDurumu = selectedIlan.TapuDurumu ?? string.Empty,
                    GayrimenkulSahibi = selectedIlan.GayrimenkulSahibi ?? string.Empty,
                    Konum = selectedIlan.Konum ?? string.Empty,
                    VideoLink = selectedIlan.VideoLink ?? string.Empty,
                    TKGMParselLink = selectedIlan.TKGMParselLink ?? string.Empty,
                    IlanNo = selectedIlan.IlanNo ?? string.Empty,
                    GirisTarihi = selectedIlan.GirisTarihi != default ? selectedIlan.GirisTarihi : DateTime.Now,
                    ImarDurumu = selectedIlan.ImarDurumu ?? string.Empty,
                    Gabari = selectedIlan.Gabari ?? string.Empty,
                    Kaks = selectedIlan.Kaks ?? string.Empty,
                    SerhDurumu = selectedIlan.SerhDurumu ?? string.Empty,
                    KrediyeUygunluk = selectedIlan.KrediyeUygunluk ?? string.Empty,
                    TakasaUygunluk = selectedIlan.TakasaUygunluk ?? string.Empty,
                    Kimden = selectedIlan.Kimden ?? string.Empty,
                    Latitude = selectedIlan.Latitude != 0 ? selectedIlan.Latitude : 0,
                    Longitude = selectedIlan.Longitude != 0 ? selectedIlan.Longitude : 0,
                    FirstName = selectedIlan.FirstName ?? string.Empty,
                    LastName = selectedIlan.LastName ?? string.Empty,
                    PhoneNumber = selectedIlan.PhoneNumber ?? string.Empty,
                    Email = selectedIlan.Email ?? string.Empty,
                    GoruntulenmeSayisi = selectedIlan.GoruntulenmeSayisi > 0 ? selectedIlan.GoruntulenmeSayisi : 0,
                    GoruntulenmeTarihi = selectedIlan.GoruntulenmeTarihi != default ? selectedIlan.GoruntulenmeTarihi : DateTime.Now,
                    Status = selectedIlan.Status ?? string.Empty,
                    ProfilePicture = selectedIlan.ProfilePicture ?? string.Empty,
                    ProfilePicturePath = selectedIlan.ProfilePicturePath ?? string.Empty,
                    MulkTipiArsa = selectedIlan.MulkTipiArsa ?? string.Empty,
                    ArsaDurumu = selectedIlan.ArsaDurumu ?? string.Empty,
                    PatronunNotu = selectedIlan.PatronunNotu ?? string.Empty,
                    MesajSayisi = selectedIlan.MesajSayisi > 0 ? selectedIlan.MesajSayisi : 0,
                    TelefonAramaSayisi = selectedIlan.TelefonAramaSayisi > 0 ? selectedIlan.TelefonAramaSayisi : 0,
                    FavoriSayisi = selectedIlan.FavoriSayisi > 0 ? selectedIlan.FavoriSayisi : 0,
                    AramaTarihi = selectedIlan.AramaTarihi != default ? selectedIlan.AramaTarihi : DateTime.Now,
                    LastActionDate = selectedIlan.LastActionDate ?? DateTime.Now,
                    TalepTarihi = DateTime.Now
                };


                // Talebi veritabanına ekle
                _context.GununIlaniTalepler.Add(yeniTalep);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Dopingler()
        {
            var userEmail = User.Identity.Name; // Oturum açan kullanıcının emaili
            if (string.IsNullOrEmpty(userEmail))
                return Json(new { success = false, message = "Kullanıcı oturum açmamış!" });

            var ilanlar = await _context.IlanBilgileri
                .Where(i => i.Email == userEmail && i.Status == "active")
                .Select(i => new { i.Id, i.Title })
                .ToListAsync();

            // View ile ilanları gönderiyoruz
            return View(ilanlar); // Iletilen ilanlar burada View'a aktarılacak
        }





    }
}
