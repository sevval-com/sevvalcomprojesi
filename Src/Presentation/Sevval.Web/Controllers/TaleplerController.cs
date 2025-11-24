using Microsoft.AspNetCore.Mvc;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;   // DbContext’inizin bulunduğu namespace (örneğin ApplicationDbContext)

namespace YourProject.Controllers
{
    public class TaleplerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TaleplerController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Talepler
        public IActionResult Index()
        {
            // Veritabanından tüm satış taleplerini çekiyoruz
            var satisTalepleri = _dbContext.SatisTalepleri.ToList();

            // Modelin tüm özelliklerini alıp, herhangi bir kayıtta değeri bulunanları filtreleyelim
            var properties = typeof(SatisTalep).GetProperties()
                .Where(prop => satisTalepleri.Any(x =>
                    prop.GetValue(x) != null &&
                    !string.IsNullOrEmpty(prop.GetValue(x)?.ToString())))
                .ToList();

            // Model özellikleri için Türkçe sütun başlıkları sözlüğü
            var columnHeaders = new Dictionary<string, string>
            {
               {"selectCategory", "Kategori"},
               {"selectStatus", "Durum"},
               {"Rooms", "Oda Sayısı"},
               {"Area", "Alan"},
               {"BuildingAge", "Bina Yaşı"},
               {"Floor", "Kat"},
               {"Bathrooms", "Banyo"},
               {"HeatingSystem", "Isıtma Sistemi"},
               {"Parking", "Otopark"},
               {"Price", "Fiyat"},
               {"ResidentialCity", "İl"},
               {"ResidentialDistrict", "İlçe"},
               {"ResidentialVillage", "Mahalle"},
               {"LandAda", "Ada"},
               {"LandParsel", "Parsel"},
               {"LandArea", "Arazi Alanı"},
               {"Slope", "Eğim"},
               {"RoadCondition", "Yol Durumu"},
               {"DistanceToSettlement", "Yerleşim Yeri Uzaklığı"},
               {"ZoningStatus", "İmar Durumu"},
               {"LandCity", "Arazi - İl"},
               {"LandDistrict", "Arazi - İlçe"},
               {"LandVillage", "Arazi - Köy"},
               {"LandPrice", "Arazi Fiyatı"},
               {"FirstName", "Ad"},
               {"LastName", "Soyad"},
               {"Email", "E-Posta"},
               {"Phone", "Telefon"},
               {"LivingCity", "Yaşanılan Şehir"},
               {"CreatedDate", "Oluşturulma Tarihi"}
            };

            // İlgili verileri ViewBag üzerinden view’a gönderiyoruz
            ViewBag.Properties = properties;
            ViewBag.ColumnHeaders = columnHeaders;

            return View(satisTalepleri);
        }
    }
}
