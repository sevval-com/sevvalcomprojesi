namespace Sevval.Domain.Entities;

public class UserVerification
{
    public int Id { get; set; }
    public string UserId { get; set; } // ApplicationUser ile ilişkilendirilecek
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string VerificationCode { get; set; }
    public DateTime VerificationDate { get; set; }
    public DateTime VerificationExpiryDate { get; set; }
    public string IPAddress { get; set; }
    public string UserCode { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; }
    // Diğer property'ler
    public List<string> AuthorizedPropertyNumbers { get; set; } = new List<string>();
}

