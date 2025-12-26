using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta adresi veya telefon numarası gereklidir.")]
        [Display(Name = "E-posta Adresi veya Telefon Numarası")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
