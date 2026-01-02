using Sevval.Domain.Entities;
using System.Collections.Concurrent;

namespace sevvalemlak.Dto
{
    public class TumIlanlarDTO
    {
        public List<IlanModel> _Ilanlar { get; set; }
        public List<GununIlanModel> _GununIlanı { get; set; }
        public Dictionary<int, Dictionary<int, Dictionary<int, List<IlanModel>>>> GroupedIlanlar { get; set; }
        public List<PhotoModel> _Fotograflar { get; set; }
        public List<VideoModel> _Videolar { get; set; }  // Videolar için ekleme
        public List<ApplicationUser> Users { get; set; } // Kullanıcı bilgilerini tutan liste
        public List<string> AvailableCities { get; set; } // Şehir listesi
        public List<string> AvailableDistricts { get; set; } // İlçe listesi
        public string SelectedUser { get; set; }  // Bu durumda, bir string olarak kalır
        // EİDS onaylı kullanıcıların detaylarını tutacak property
        public List<UserVerification> UserVerifications { get; set; }
        public List<ConsultantDTO> Consultants { get; set; }  // Danışmanlar
        public List<KullaniciDTO> _Kullanicilar { get; set; } // Yeni eklendi
        public int TotalViewsToday { get; set; } // Bugünkü toplam görüntülenme sayısı
        public int ToplamGoruntulenme { get; set; }  // Bu satırı ekleyin
        public List<string> ConsultantEmails { get; set; }
        public ApplicationUser User { get; internal set; }
        public string ProfilePicturePath { get; set; }
        public int ToplamArama { get; set; }
        public List<int> WeeklySearches { get; set; } // Haftalık arama verileri için
        public List<Comment> Comments { get; set; }  // Yorumları ekledik
        public int IlanSayisi { get; set; }  // Burada ismi değiştirdik
        public int TotalIlanCount { get; set; }
        public int ActiveCount { get; set; }
        public int DeletedCount { get; set; }
        public int ArchivedCount { get; set; }
        public int InactiveCount { get; set; }
        public int RejectedCount { get; set; }
        public int ArchiveCount { get; set; }
        // Kategorilere özel ilan sayıları
        public int KonutIlanlariCount { get; set; }
        public int IsYeriIlanlariCount { get; set; }
        public int TuristikTesisIlanlariCount { get; set; }
        public int ArsaIlanlariCount { get; set; }
        public int BahceIlanlariCount { get; set; }
        public int TarlaIlanlariCount { get; set; }
        public List<string> KategoriAdlari { get; set; }
        public List<int> KategoriVerileri { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } // Telefon numarası
        public int TotalIlanlar { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public Dictionary<string, int> SehirIlanlari { get; set; } // Şehir ve ilan sayısı
        public List<int> WeeklyViews { get; internal set; }
        public List<string> WeekDays { get; internal set; }
        // Geçici olarak önceki görüntülenme sayılarını tutan sözlük
        public static ConcurrentDictionary<int, int> OncekiGoruntulenmeSayilari { get; } = new ConcurrentDictionary<int, int>();
        public string FilterType { get; internal set; }
        public string? SelectedCategory { get; internal set; }
        public string? SelectedDurum { get; internal set; }
        public string? SelectedSubcategory { get; internal set; }
        public List<IlanModel> EnCokGoruntulenenIlanlar { get; internal set; }
        public object IlanBasliklari { get; internal set; }
        public List<YorumModel> Yorumlar { get; set; } // Yorumlar için liste
        public List<int> GoruntulenmeSayilari { get; internal set; }
        // Haftalık görüntülenme verilerini statik olarak tutmak için
        private static Dictionary<string, List<int>> _weeklyViewData = new Dictionary<string, List<int>>();
        public List<IlanModel> AllIlanlar { get; set; }
        // Yeni eklenen özellik: En yüksek ilan Id'si
        public int MaxIlanId { get; set; }
        public int TotalUsersCount { get; internal set; }
        public Dictionary<string, int> KonutDurumlari { get; internal set; }
        public object RecentUsers { get; internal set; }
        public string Filter { get; set; }
        // Firma sahibi için logo ekleme
        public string CompanyLogoPath { get; set; }
        public string CompanyName { get; set; }
        public List<int> TelefonAramaSayisi { get; set; }
        public static ConcurrentDictionary<int, int> OncekiAramaSayilari { get; } = new ConcurrentDictionary<int, int>();
        // Haftalık arama verisi
        public List<int> WeeklySearchData { get; set; }
        // Kullanıcı ve şirket sahibinin profil fotoğrafı için alanlar
        public string UserProfilePicturePath { get; set; } // İlanı veren kullanıcının profil fotoğrafı
        public string CompanyOwnerProfilePicturePath { get; set; } // Şirket sahibinin profil fotoğrafı
        public DateTime? RegistrationDate { get; set; }
        // Haftanın günleri
        public List<string> WeekDaysLabels { get; set; } // Haftanın günleri için
        public List<IlanModel> RandomIlanlar { get; internal set; }
        public List<PhotoModel> RandomIlanFotograflari { get; set; } // Rastgele ilanların fotoğrafları (ayrı tutulmalı)
        // Yeni eklenen firma bilgileri
        public string CompanyLocation { get; set; }
        public int CompanyTotalIlanCount { get; set; }
        public bool IsConsultant { get; set; }
        // Diğer propertyler...
        public string CompanyOwnerId { get; set; }
        public ApplicationUser CompanyOwner { get; set; }
        public List<dynamic> UsersWithCompany { get; set; } // Dinamik eşleşme
        public List<int> WeeklyFavorites { get; internal set; }
        public List<string> HaftaGunleri { get; internal set; }
        public int ToplamFavori { get; internal set; }
        public int TotalArchiveCount { get; set; } // Yeni özellik
        public List<string> WeekDaysFavori { get; internal set; }
        public string SelectedUserEmail { get; internal set; }
        public List<SehirIlanSayisiDTO> SehirIlanSayilari { get; set; }
        public string SearchTerm { get; internal set; }
        public int TalepCount { get; internal set; }
        public int? PageSize { get; internal set; }
        public string TKGMParselLink { get; internal set; }

        // İlanların Id'lerinden en yüksek olanı bulan fonksiyon
        public void SetMaxIlanId()
        {
            if (_Ilanlar != null && _Ilanlar.Any())
            {
                MaxIlanId = _Ilanlar.Max(i => i.Id); // Id'lerin maksimum değerini al
            }
            else
            {
                MaxIlanId = 0; // Eğer ilan yoksa, 0 dönecek
            }
        }
        public static List<int> GetWeeklyViews(string userEmail)
        {
            // Veriler önceden saklanmışsa onları döndür
            if (_weeklyViewData.ContainsKey(userEmail))
            {
                return _weeklyViewData[userEmail];
            }
            else
            {
                // Yeni veriler için sıfırla başlat
                _weeklyViewData[userEmail] = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
                return _weeklyViewData[userEmail];
            }
        }
        public static void SaveWeeklyViews(string userEmail, List<int> weeklyViews)
        {
            // Verileri sakla
            _weeklyViewData[userEmail] = weeklyViews;
        }

        // Haftalık arama verilerini tutmak için yeni bir statik sözlük
        private static Dictionary<string, List<int>> _weeklySearchData = new Dictionary<string, List<int>>();

        public static List<int> GetWeeklySearches(string userEmail)
        {
            if (_weeklySearchData.ContainsKey(userEmail))
            {
                return _weeklySearchData[userEmail];
            }
            else
            {
                _weeklySearchData[userEmail] = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
                return _weeklySearchData[userEmail];
            }
        }

        public static void SaveWeeklySearches(string userEmail, List<int> weeklySearches)
        {
            _weeklySearchData[userEmail] = weeklySearches;
        }


        public class SehirIlanSayisiDTO
        {
            public string Sehir { get; set; }
            public int IlanSayisi { get; set; }
            public bool ResimVar { get; set; } // Yeni özellik
            public string Aciklama { get; internal set; }
            public string SehirMetni { get; internal set; }
        }

        public TumIlanlarDTO()
        {
            AllIlanlar = new List<IlanModel>();
            // Yeni eklenen alanları burada başlat
            WeeklySearches = new List<int>();
            WeekDaysLabels = new List<string>();
            RandomIlanlar = new List<IlanModel>();
            RandomIlanFotograflari = new List<PhotoModel>();
        }
    }
}
