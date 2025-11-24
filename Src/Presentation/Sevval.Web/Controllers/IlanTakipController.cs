using ClosedXML.Excel; // Added for Excel export
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using System.Reflection;

namespace YourProjectNamespace.Controllers
{
    public class IlanTakipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IlanTakipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IlanTakip
        public async Task<IActionResult> Index()
        {
            var ilanlar = await _context.BireyselIlanTakipleri.ToListAsync();
            var consultantOptions = await _context.ConsultantInvitations
               .Where(c => c.CompanyName == "ŞEVVAL EMLAK" || c.CompanyName == "ACR EMLAK")
               .Select(c => new
               {
                   Value = c.FirstName + " " + c.LastName,
                   Text = c.FirstName + " " + c.LastName
               })
               .ToListAsync();

            var userOptions = await _context.Users
                .Where(u => u.CompanyName == "ŞEVVAL EMLAK" || u.CompanyName == "ACR EMLAK")
                .Select(u => new
                {
                    Value = u.FirstName + " " + u.LastName,
                    Text = u.FirstName + " " + u.LastName
                })
                .ToListAsync();

            var referansOptions = consultantOptions.Union(userOptions).ToList();
            ViewBag.ReferansOptions = referansOptions;
            return View(ilanlar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BireyselIlanTakibi model)
        {
            if (ModelState.IsValid)
            {
                _context.BireyselIlanTakipleri.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var consultantOptions = await _context.ConsultantInvitations
               .Where(c => c.CompanyName == "ŞEVVAL EMLAK" || c.CompanyName == "ACR EMLAK")
               .Select(c => new
               {
                   Value = c.FirstName + " " + c.LastName,
                   Text = c.FirstName + " " + c.LastName
               })
               .ToListAsync();

            var userOptions = await _context.Users
                .Where(u => u.CompanyName == "ŞEVVAL EMLAK" || u.CompanyName == "ACR EMLAK")
                .Select(u => new
                {
                    Value = u.FirstName + " " + u.LastName,
                    Text = u.FirstName + " " + u.LastName
                })
                .ToListAsync();

            var referansOptions = consultantOptions.Union(userOptions).ToList();
            ViewBag.ReferansOptions = referansOptions;
            var ilanlar = await _context.BireyselIlanTakipleri.ToListAsync();
            return View("Index", ilanlar);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCell(int id, string fieldName, string newValue)
        {
            // İlgili kaydı bul
            var record = await _context.BireyselIlanTakipleri.FindAsync(id);
            if (record == null)
            {
                return Json(new { success = false });
            }

            // Reflection ile property bilgisini al
            var property = typeof(BireyselIlanTakibi).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null || !property.CanWrite)
            {
                return Json(new { success = false });
            }

            try
            {
                // Eğer property int veya nullable int ise dönüştürme yapılır
                if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                {
                    if (int.TryParse(newValue, out int parsedValue))
                    {
                        property.SetValue(record, parsedValue);
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                else
                {
                    property.SetValue(record, newValue);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpGet] // Ensure this attribute is present
        public async Task<IActionResult> ExportToExcel()
        {
            // Veritabanından ilanları al
            var ilanlar = await _context.BireyselIlanTakipleri.ToListAsync();

            // Excel çalışma kitabını oluştur
            using (var workbook = new XLWorkbook())
            {
                // Çalışma sayfası ekle
                var worksheet = workbook.Worksheets.Add("Bireysel İlanlar");

                // Başlık satırunu oluştur
                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 12;
                worksheet.Row(1).Style.Font.FontColor = XLColor.White;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#003366"); // Mavi Arkaplan

                // Birleştirilmiş hücre oluştur ve başlığı yaz
                worksheet.Range(1, 1, 1, 22).Merge().Value = "BİREYSEL İLAN TAKİP"; //sütun sayısı arttı
                worksheet.Range(1, 1, 1, 22).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(1, 1, 1, 22).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Sütun başlıklarını ekle
                worksheet.Cell(2, 1).Value = "No";
                worksheet.Cell(2, 2).Value = "Kategori";
                worksheet.Cell(2, 3).Value = "İsim";
                worksheet.Cell(2, 4).Value = "Cep Tel";
                worksheet.Cell(2, 5).Value = "İl";
                worksheet.Cell(2, 6).Value = "İlçe";
                worksheet.Cell(2, 7).Value = "Mahalle/Köy";
                worksheet.Cell(2, 8).Value = "Ada";
                worksheet.Cell(2, 9).Value = "Parsel";
                worksheet.Cell(2, 10).Value = "M²";
                worksheet.Cell(2, 11).Value = "Oda Sayısı";
                worksheet.Cell(2, 12).Value = "Bina Yaşı";
                worksheet.Cell(2, 13).Value = "Bina Kat Sayısı";
                worksheet.Cell(2, 14).Value = "Bulunduğu Kat";
                worksheet.Cell(2, 15).Value = "Isıtma Sistemi";
                worksheet.Cell(2, 16).Value = "Balkon";
                worksheet.Cell(2, 17).Value = "İmar";
                worksheet.Cell(2, 18).Value = "Tapu";
                worksheet.Cell(2, 19).Value = "İlan No";
                worksheet.Cell(2, 20).Value = "Eline İstenen Fiyat";
                worksheet.Cell(2, 20).Value = "İlan Fiyatı";
                worksheet.Cell(2, 21).Value = "Referans";
                worksheet.Cell(2, 22).Value = "İlan Girildi Mi?"; // Yeni sütun

                // Verileri ekle
                for (int i = 0; i < ilanlar.Count; i++)
                {
                    var ilan = ilanlar[i];
                    worksheet.Cell(i + 3, 1).Value = ilan.Id;
                    worksheet.Cell(i + 3, 2).Value = ilan.Kategori;
                    worksheet.Cell(i + 3, 3).Value = ilan.Isim;
                    worksheet.Cell(i + 3, 4).Value = ilan.CepTel;
                    worksheet.Cell(i + 3, 5).Value = ilan.Il;
                    worksheet.Cell(i + 3, 6).Value = ilan.Ilce;
                    worksheet.Cell(i + 3, 7).Value = ilan.MahalleKoy;
                    worksheet.Cell(i + 3, 8).Value = ilan.Ada;
                    worksheet.Cell(i + 3, 9).Value = ilan.Parsel;
                    worksheet.Cell(i + 3, 10).Value = ilan.Metrekare;
                    worksheet.Cell(i + 3, 11).Value = ilan.OdaSayisi;
                    worksheet.Cell(i + 3, 12).Value = ilan.BinaYasi;
                    worksheet.Cell(i + 3, 13).Value = ilan.BinaKatSayisi;
                    worksheet.Cell(i + 3, 14).Value = ilan.BulunduguKat;
                    worksheet.Cell(i + 3, 15).Value = ilan.IsitmaSistemi;
                    worksheet.Cell(i + 3, 16).Value = ilan.Balkon;
                    worksheet.Cell(i + 3, 17).Value = ilan.Imar;
                    worksheet.Cell(i + 3, 18).Value = ilan.Tapu;
                    worksheet.Cell(i + 3, 19).Value = ilan.IlanNo;
                    worksheet.Cell(i + 3, 20).Value = ilan.Fiyat;
                    worksheet.Cell(i + 3, 20).Value = ilan.IlanFiyati;
                    worksheet.Cell(i + 3, 21).Value = ilan.Referans;
                    worksheet.Cell(i + 3, 22).Value = ilan.IlanGirildiMi; // Yeni sütun
                }

                // Tüm hücrelere kenarlık ekle
                worksheet.Cells().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Cells().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                // Sütunları otomatik boyutlandır
                worksheet.Columns().AdjustToContents();

                // Hücre hizalamasını sola ayarla
                worksheet.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                // Excel dosyasını belleğe yaz
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Dosyayı indirme için gerekli başlıkları ayarla
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "BireyselIlanTakip.xlsx");
                }
            }
        }
    }
}