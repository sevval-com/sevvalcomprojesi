using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Sevval.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public int UserOrder { get; set; }

    /// <summary>
    /// Formatlanmış Firma No döndürür (K-0001, B-0001 formatında)
    /// </summary>
    [NotMapped]
    public string FormattedFirmaNo
    {
        get
        {
            if (UserOrder == 0) return "Atanmadı";
            var prefix = UserTypes == "Bireysel" ? "B" : "K";
            return $"{prefix}-{UserOrder:D4}";
        }
    }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string UserTypes { get; set; } = string.Empty;

  
    public string? ProfilePicturePath { get; set; }
    public string? BannerPicturePath { get; set; }
    
    // 🆕 Kurumsal Kullanıcılar için Belge Yolları (UserTypes'a göre dinamik)
    // Emlakçı: Document1 = Seviye 5, Document2 = Vergi Levhası
    // İnşaat: Document1 = Müteahhitlik Belgesi, Document2 = Vergi Levhası
    // Banka: Document1 = İmza Sirküleri, Document2 = Vergi Levhası
    // Vakıf: Document1 = Vakıf Senedi, Document2 = Vergi Levhası
    public string? Document1Path { get; set; }
    public string? Document2Path { get; set; }
    
    // ⚠️ DEPRECATED: Geriye dönük uyumluluk için tutuyoruz, yeni kayıtlar Document1/2 kullanacak
    [Obsolete("Use Document1Path instead")]
    public string? Level5CertificatePath { get; set; }
    [Obsolete("Use Document2Path instead")]
    public string? TaxPlatePath { get; set; }


    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string District { get; set; } = string.Empty;

    [Required]
    public string IPAddress { get; set; } = string.Empty;

    public bool IsConsultant { get; set; }

    public string? ConsultantCompanyId { get; set; }

    [ForeignKey("ConsultantCompanyId")]
    public ApplicationUser? ConsultantCompany { get; set; }

    public virtual ICollection<ApplicationUser>? Consultants { get; set; }

    private DateTime? _registrationDate;

    public DateTime RegistrationDate
    {
        get => _registrationDate ?? DateTime.Now; // Varsayılan olarak mevcut tarih atanır
        set => _registrationDate = value;
    }
    public int IlanSayisi { get; set; }

    public string Ucretlilik { get; set; } = string.Empty;

    // Abonelik Durumu
    public string IsSubscribed { get; set; } // Abonelik durumu (aktif/pasif)

    // Abonelik Başlangıç Tarihi
    public DateTime? SubscriptionStartDate => RegistrationDate;

    // Abonelik Bitiş Tarihi (1 ay sonrası)
    public DateTime? SubscriptionEndDate { get; private set; }
    // Kullanıcının aktif mi pasif mi olduğunu belirten durum
    public string IsActive { get; set; } // Varsayılan olarak "Active"
    public string? VergiNo { get;  set; }
    public string? AcikAdres { get; set; }  // Nullable açık adres alanı

    public string? Referans { get; set; }
    //public DateTime? CreatedDate { get; set; }
    //public string? CreatedBy { get; set; }
    //public DateTime? LastModifiedDate { get; set; }
    //public string? LastModifiedBy { get; set; }
    //public bool IsDeleted { get; set; }

}
