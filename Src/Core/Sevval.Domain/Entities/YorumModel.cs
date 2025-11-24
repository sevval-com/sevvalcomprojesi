namespace Sevval.Domain.Entities;

public class YorumModel
{
    public int Id { get; set; }
    public int IlanId { get; set; } // Yorumun hangi ilana ait olduğunu belirten ID
    public string Yorum { get; set; } // Yorum metni
    public ApplicationUser Kullanici { get; set; }
    public DateTime YorumTarihi { get; set; } // Yorum tarihi
    public IlanModel Ilan { get; set; }
}


