using System.ComponentModel.DataAnnotations;

namespace Sevval.Domain.Entities
{
    public class BireyselIlanTakibi
    {
        [Key]
        public int Id { get; set; }
        public string? Isim { get; set; }
        public string? CepTel { get; set; }
        public string? Il { get; set; }
        public string? Ilce { get; set; }
        public string? MahalleKoy { get; set; }
        public string? Ada { get; set; }
        public string? Parsel { get; set; }
        public string? Metrekare { get; set; }
        public string? Imar { get; set; }
        public string? Tapu { get; set; }
        public string? IlanNo { get; set; }
        public string? Fiyat { get; set; }
        public string? Referans { get; set; }
        public int? OdaSayisi { get; set; }
        public string? Kategori { get; set; }
        public int? BinaYasi { get; set; }
        public int? BinaKatSayisi { get; set; }
        public int? BulunduguKat { get; set; }
        public string? IsitmaSistemi { get; set; }
        public string? Balkon { get; set; }
        public string? IlanGirildiMi { get; set; } // Yeni sütun
        public string? IlanFiyati { get; set; } // Yeni sütun
    }
}
