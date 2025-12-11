namespace sevvalemlak.Dto
{
    public class DetayliAramaRequestDTO
    {
        public int Id { get; set; }
        public string? Kategori { get; set; }
        public string? MülkTipi { get; set; }
        public string? KonutDurumu { get; set; }
        public decimal? EnDusukFiyat { get; set; }
        public decimal? EnYuksekFiyat { get; set; }
        public string? Il { get; set; }
        public string? Ilce { get; set; }
        public string? Mahalle { get; set; }
        public double? EnAzMetrekare { get; set; }
        public double? EnCokMetrekare { get; set; }
        public string? OdaSayisi { get; set; }
        public string? BinaYasi { get; set; }
        public string? IsitmaTipi { get; set; }
        public string? BalkonSayisi { get; set; }
        public string? AsansorDurumu { get; set; }
        public string? OtoparkDurumu { get; set; }
        public bool? FotograDurumu { get; set; }
        public bool? VideoDurumu { get; set; }
        public DateTime? GirisTarihi { get; set; }  // Nullable yapıldı
        public string? IlanVitrinImageUrl { get; set; }  // Nullable yapıldı

        // Sıralama ve Sayfalama
        public string? Siralama { get; set; } // price_asc, price_desc, date_desc, date_asc
        public int Sayfa { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 12;
    }
}
