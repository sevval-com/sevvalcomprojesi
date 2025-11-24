using System.ComponentModel.DataAnnotations;

namespace Sevval.Domain.Entities;

public class AfisTalep
{
    public int Id { get; set; }

    [Required]
    public string IlanTuru { get; set; }

    [Required]
    public string Telefon { get; set; }

    [Required]
    public string IsimSoyad { get; set; }

    [Required]
    public string Firma { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public DateTime TalepTarihi { get; set; }

    // Yeni "Durum" alanı
    [Required]
    public string Durum { get; set; } = "BEKLEMEDE"; // Varsayılan değer olarak "BEKLEMEDE"
}
