namespace Sevval.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdQueryResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string UserTypes { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsConsultant { get; set; }


        private DateTime? RegistrationDate;


        public int IlanSayisi { get; set; }

        public string Ucretlilik { get; set; } = string.Empty;

        // Abonelik Durumu
        public string IsSubscribed { get; set; } // Abonelik durumu (aktif/pasif)

        // Abonelik Başlangıç Tarihi
        public DateTime? SubscriptionStartDate;

        // Abonelik Bitiş Tarihi (1 ay sonrası)
        public DateTime? SubscriptionEndDate { get; set; }
        // Kullanıcının aktif mi pasif mi olduğunu belirten durum
        public string IsActive { get; set; }  // Varsayılan olarak "Active"
        public string? VergiNo { get; internal set; }
        public string? AcikAdres { get; set; }  // Nullable açık adres alanı

        public string? Referans { get; set; }
        public string? Level5CertificatePath { get; set; }
        public string? TaxPlatePath { get; set; }

    }
}
