namespace Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements
{
    public class GetSuitableAnnouncementsQueryResponse
    {
        public int Id { get; set; }
        public string IlanBasligi { get; set; }
        public string IlanAciklamasi { get; set; }
        public string IlanVitrinImageUrl { get; set; }
        public decimal IlanFiyati { get; set; }
        public string? sehir { get; set; } // Oda sayısı
        public string? semt { get; set; } // Oda sayısı
        public string? mahalleKoy { get; set; } // Oda sayısı
        public string? AdaNo { get; set; } // Ada numarası
        public string? ParselNo { get; set; } // Parsel numarası
        public double Area { get; set; } // Alan (m²)
        public int AnnouncementId { get; set; }
        public string? OdaSayisi { get; set; }
        public string? YatakSayisi { get; set; }
        public string? BinaYasi { get; set; }
        public string? NetMetrekare { get; set; }
        public string? Category { get; set; } // Kategori

        public string? KonutDurumu { get; set; } // Konut durumu (Satılık/Kiralık)

        public string? MulkTipi { get; set; }
        public DateTime GirisTarihi { get; set; }
    }


}
