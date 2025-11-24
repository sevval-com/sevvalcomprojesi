using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Domain.Entities;

public class GununIlaniTalep
{
    public int Id { get; set; }

    public string? Category { get; set; } // Kategori

    public string? KonutDurumu { get; set; } // Konut durumu (Satılık/Kiralık)

    public string? MulkTipi { get; set; } // Mülk tipi

    public string? SelectedCategories { get; set; } // Seçilen kategoriler

    public string? Title { get; set; } // İlan başlığı
    public string? MeyveninCinsi { get; set; } // İlan başlığı

    public string? Description { get; set; } // İlan açıklaması

    public decimal Price { get; set; } // Fiyat
    public decimal PricePerSquareMeter { get; set; } // Fiyat

    public decimal Aidat { get; set; } // Aylık aidat

    public decimal TasınmazNumarasi { get; set; } // Taşınmaz numarası

    public double Area { get; set; } // Alan (m²)

    public string? AdaNo { get; set; } // Ada numarası

    public string? ParselNo { get; set; } // Parsel numarası

    public string? PaftaNo { get; set; } // Pafta numarası
    public string? AcikAlan { get; set; } // Pafta numarası
    public string? KapaliAlan { get; set; } // Pafta numarası
    public string? GunlukMusteriSayisi { get; set; } // Pafta numarası

    public string? BrutMetrekare { get; set; } // Brüt metrekare

    public string? NetMetrekare { get; set; } // Net metrekare

    public string? OdaSayisi { get; set; } // Oda sayısı
    public string? sehir { get; set; } // Oda sayısı
    public string? semt { get; set; } // Oda sayısı
    public string? mahalleKoy { get; set; } // Oda sayısı
    public string? YatakSayisi { get; set; } // Oda sayısı

    public string? BinaYasi { get; set; } // Bina yaşı

    public string? KatSayisi { get; set; } // Kat sayısı

    public string? BulunduguKat { get; set; } // Bulunduğu kat

    public string? Isitma { get; set; } // Isıtma tipi

    public string? BanyoSayisi { get; set; } // Banyo sayısı

    public string? AraziNiteliği { get; set; } // Arazi niteliği

    public string? Balkon { get; set; } // Balkon durumu

    public string? Asansor { get; set; } // Asansör durumu

    public string? Otopark { get; set; } // Otopark durumu

    public string? Esyali { get; set; } // Eşya durumu

    public string? Takas { get; set; } // Takas durumu

    public string? KullanimDurumu { get; set; } // Kullanım durumu

    public string? TapuDurumu { get; set; } // Tapu durumu

    public string? GayrimenkulSahibi { get; set; } // Gayrimenkul sahibi

    public string? Konum { get; set; } // Konum bilgisi

    public string? VideoLink { get; set; }  // Video için bir alan

    public string? TKGMParselLink { get; set; }  // TKGM Parsel linki

    public string? IlanNo { get; set; }  // İlan numarası

    public DateTime GirisTarihi { get; set; } = DateTime.Now;  // Giriş tarihi

    public string? ImarDurumu { get; set; }  // İmar durumu

    public string? Gabari { get; set; }  // Gabari

    public string? Kaks { get; set; }  // KAKS
    public string? SerhDurumu { get; set; }  // KAKS

    public string? KrediyeUygunluk { get; set; }  // Krediye uygunluk

    public string? TakasaUygunluk { get; set; }  // Takasa uygunluk

    public string? Kimden { get; set; }  // Kimden

    public double Latitude { get; set; }  // Enlem

    public double Longitude { get; set; }  // Boylam

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public int GoruntulenmeSayisi { get; set; }
    public DateTime GoruntulenmeTarihi { get; set; }
    public string? Status { get; set; }

    //[ForeignKey("User")]
    //public string UserId { get; set; }  // Bu kısmı doğru şekilde eklediğinizden emin olun
    //public ApplicationUser User { get; set; }  // Kullanıcıyı ilişkilendirmek için

    public string? ProfilePicture { get; set; }
    public string? ProfilePicturePath { get; set; }

    [NotMapped] // Bu alanı veritabanına dahil etme
    public IFormFile[]? UploadedVideos { get; set; } // Yüklenen video dosyaları

    [NotMapped] // Bu alanı veritabanına dahil etme
    public IFormFile[]? UploadedFiles { get; set; } // Yüklenen fotoğraf dosyaları

    // Yeni alanlar
    public string? MulkTipiArsa { get; set; } // Arsa mülk tipi
    public string? ArsaDurumu { get; set; } // Arsa durumu (Satılık/Kiralık/Kat Karşılığı)
    public string? PatronunNotu { get; set; } // Arsa durumu (Satılık/Kiralık/Kat Karşılığı)
    public int MesajSayisi { get; set; }
    public int TelefonAramaSayisi { get; set; }
    public int FavoriSayisi { get; set; }
    public DateTime AramaTarihi { get; set; }
    public DateTime? LastActionDate { get; set; }
    public DateTime TalepTarihi { get;  set; }
}


