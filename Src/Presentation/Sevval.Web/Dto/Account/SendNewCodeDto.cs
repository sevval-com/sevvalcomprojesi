using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.csproj.Dto.Account;


public class SendNewCodeDto
{
    [Required(ErrorMessage = "E-posta gerekli")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
    public string Email { get; set; }
}
