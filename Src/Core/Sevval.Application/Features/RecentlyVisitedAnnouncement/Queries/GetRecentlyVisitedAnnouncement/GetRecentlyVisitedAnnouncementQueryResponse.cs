namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement
{
    public class GetRecentlyVisitedAnnouncementQueryResponse
    {
        public int Id { get; set; }
        public string IlanBasligi { get; set; }
        public string IlanAciklamasi { get; set; }
        public string IlanVitrinImageUrl { get; set; }
        public decimal IlanFiyati { get; set; }
        public string? sehir { get; set; } // Oda sayýsý
        public string? semt { get; set; } // Oda sayýsý
        public string? mahalleKoy { get; set; } // Oda sayýsý
        public string? AdaNo { get; set; } // Ada numarasý
        public string? ParselNo { get; set; } // Parsel numarasý
        public double Area { get; set; } // Alan (m²)
        public int AnnouncementId { get; set; }
        public string? OdaSayisi { get; set; }
        public string? YatakSayisi { get; set; }
        public string? BinaYasi { get; set; }
        public string? NetMetrekare { get; set; }
        public string? Category { get; set; } // Kategori

        public string? KonutDurumu { get; set; } // Konut durumu (Satýlýk/Kiralýk)

        public string? MulkTipi { get; set; }
        public DateTime GirisTarihi { get; set; }
    }
}
