namespace Sevval.Application.Features.AboutUs.Queries.GetAboutUs
{
    public class GetAboutUsQueryResponse 
    {
        public string GoogleMapsAddress { get; set; } = string.Empty;
        public string GoogleMapsApiKey { get; set; } 
        public string MapWarningMessage { get; set; } = "Bu adres Google İşletme sayfanızdan çekilmiştir. Adres yanlışsa Google Maps'teki adresinizi düzeltiniz.";
        public string CompanyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public string? BannerPicturePath { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int TotalAnnouncementCount{ get; set; }
        public string? Level5CertificatePath { get; set; }
        public string? TaxPlatePath { get; set; }
    }

     

  

    
}
