using System.ComponentModel.DataAnnotations;

namespace Sevval.Domain.Entities;

public class Sepet
{
    [Key]
    public int Id { get; set; }
    
    public string UserEmail { get; set; }
    
    public int IlanId { get; set; }
    
    // Yeni alanlar: Ürün Adı ve Adedi
    public string UrunAdi { get; set; }
    
    public int Adet { get; set; }
    // Fiyat ekledik
    public decimal Fiyat { get; set; }

    public DateTime CreatedAt { get; set; }
}
