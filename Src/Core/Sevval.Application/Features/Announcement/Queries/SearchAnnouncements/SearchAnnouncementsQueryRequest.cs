using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.SearchAnnouncements
{
    public class SearchAnnouncementsQueryRequest : IRequest<ApiResponse<SearchAnnouncementsQueryResponse>>
    {

        public const string Route = "/api/v1/announcements";

        // Basit arama için
        public string? Keyword { get; set; }

        // Detaylı filtreleme için
        public string? Category { get; set; } // Konut, İş Yeri, Arsa, Bahçe, Tarla, Turistik Tesis
        public string? PropertyType { get; set; } // MülkTipi
        public string? PropertyStatus { get; set; } // KonutDurumu (Satılık/Kiralık)

        // Fiyat filtreleri
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Lokasyon filtreleri
        public string? Province { get; set; } // İl
        public string? District { get; set; } // İlçe
        public string? Neighborhood { get; set; } // Mahalle

        // Alan filtreleri
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }

        // Konut özellikleri
        public string? RoomCount { get; set; } // Oda sayısı
        public string? BedroomCount { get; set; } // Yatak odası sayısı
         public string? BuildingAge { get; set; } // Bina yaşı
        public string? HeatingType { get; set; } // Isıtma tipi
        public string? BalconyCount { get; set; } // Balkon sayısı
        public string? ElevatorStatus { get; set; } // Asansör durumu
        public string? ParkingStatus { get; set; } // Otopark durumu

        // Medya filtreleri
        public bool? HasPhotos { get; set; } // Fotoğraflı ilanlar
        public bool? HasVideos { get; set; } // Videolu ilanlar

        // Tarih filtreleri
        public DateTime? StartDate { get; set; } // Başlangıç tarihi
        public DateTime? EndDate { get; set; } // Bitiş tarihi

        // Sayfalama
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Sıralama
        public string? SortBy { get; set; } = "Date"; // Date, Price, Area
        public string? SortOrder { get; set; } = "DESC"; // ASC, DESC

        // Kullanıcı filtreleri
        public string? UserEmail { get; set; } // Belirli kullanıcının ilanları
        public string? Status { get; set; } // İlan durumu
    }
}
