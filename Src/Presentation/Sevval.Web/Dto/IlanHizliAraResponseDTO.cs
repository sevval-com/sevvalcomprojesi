namespace sevvalemlak.Dto
{
    public class IlanHizliAraResponseDTO
    {
        public string IlanBasligi { get; set; }
        public string IlanAciklamasi { get; set; }
        public string IlanVitrinImageUrl { get; set; }
        public decimal IlanFiyati { get; set; }
        public string? sehir { get; set; }
        public string? semt { get; set; }
        public string? mahalleKoy { get; set; }
        public string? AdaNo { get; set; }
        public string? ParselNo { get; set; }
        public double Area { get; set; } // Alan (m²)
        public int Id { get; set; }
        public string? OdaSayisi { get; set; }
        public string? YatakSayisi { get; set; }
        public string? BinaYasi { get; set; }
        public string? NetMetrekare { get; set; }
        public string? BrutMetrekare { get; set; } // Yeni Eklendi
        public string? FirmaAdi { get; set; } // Yeni Eklendi
        public string? Category { get; set; }
        public string? KonutDurumu { get; set; }
        public string? MulkTipi { get; set; }
        public DateTime GirisTarihi { get; set; }
    }
}