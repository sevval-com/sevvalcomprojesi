using System.ComponentModel.DataAnnotations;

namespace Sevval.Domain.Entities;
public class ConsultantInvitation
{
    public int Id { get; set; }

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string InvitationToken { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    public string? InvitedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
    public int UserOrder { get; set; }
    public string ProfilePicturePath { get;  set; }
}

public class InviteConsultantViewModel
{
    [Required(ErrorMessage = "E-posta adresi gereklidir.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad gereklidir.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad gereklidir.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePicturePath { get; internal set; }
}


public class ConsultantInvitationEmailModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string SetPasswordUrl { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;


}