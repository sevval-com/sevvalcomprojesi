using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Domain.Entities;

public class IlanModel
{
    public int Id { get; set; }
    public string? Category { get; set; }
    public string? KonutDurumu { get; set; }
    public string? MulkTipi { get; set; }
    public string? SelectedCategories { get; set; }
    public string? Title { get; set; }
    public string? MeyveninCinsi { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal PricePerSquareMeter { get; set; }
    public decimal Aidat { get; set; }
    public decimal TasınmazNumarasi { get; set; }
    public double Area { get; set; }
    public string? AdaNo { get; set; }
    public string? ParselNo { get; set; }
    public string? PaftaNo { get; set; }
    public string? AcikAlan { get; set; }
    public string? KapaliAlan { get; set; }
    public string? GunlukMusteriSayisi { get; set; }
    public string? BrutMetrekare { get; set; }
    public string? NetMetrekare { get; set; }
    public string? OdaSayisi { get; set; }
    public string? sehir { get; set; }
    public string? semt { get; set; }
    public string? mahalleKoy { get; set; }
    public string? YatakSayisi { get; set; }
    public string? BinaYasi { get; set; }
    public string? KatSayisi { get; set; }
    public string? BulunduguKat { get; set; }
    public string? Isitma { get; set; }
    public string? BanyoSayisi { get; set; }
    public string? AraziNiteliği { get; set; }
    public string? Balkon { get; set; }
    public string? Asansor { get; set; }
    public string? Otopark { get; set; }
    public string? Esyali { get; set; }
    public string? Takas { get; set; }
    public string? KullanimDurumu { get; set; }
    public string? TapuDurumu { get; set; }
    public string? GayrimenkulSahibi { get; set; }
    public string? Konum { get; set; }
    public string? VideoLink { get; set; }
    public string? TKGMParselLink { get; set; }
    public string? IlanNo { get; set; }
    public DateTime GirisTarihi { get; set; } = DateTime.Now;
    public string? ImarDurumu { get; set; }
    public string? Gabari { get; set; }
    public string? Kaks { get; set; }
    public string? SerhDurumu { get; set; }
    public string? KrediyeUygunluk { get; set; }
    public string? TakasaUygunluk { get; set; }
    public string? Kimden { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Nullable olarak güncelledik:
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public int GoruntulenmeSayisi { get; set; }
    public DateTime GoruntulenmeTarihi { get; set; }
    public string? Status { get; set; }
    public string? ProfilePicture { get; set; }
    public string? ProfilePicturePath { get; set; }

    [NotMapped]
    public IFormFile[]? UploadedVideos { get; set; }

    [NotMapped]
    public IFormFile[]? UploadedFiles { get; set; }

    public string? MulkTipiArsa { get; set; }
    public string? ArsaDurumu { get; set; }
    public string? PatronunNotu { get; set; }
    public int MesajSayisi { get; set; }
    public int TelefonAramaSayisi { get; set; }
    public int FavoriSayisi { get; set; }
    public DateTime AramaTarihi { get; set; }
    public DateTime? LastActionDate { get; set; }
}
