using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Doğrulama kodu gereklidir.")]
        public string Code { get; set; }  // Doğrulama kodu

        [Required(ErrorMessage = "Yeni şifre gereklidir.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
        public string NewPassword { get; set; }  // Yeni şifre
    }

}
