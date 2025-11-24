using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad alanı gereklidir.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı gereklidir.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası alanı gereklidir.")]
        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Şifre en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 8)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifreyi onaylama alanı gereklidir.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifreyi Onayla")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 8)]
        public string ConfirmPassword { get; set; }

        // Profil fotoğrafı için dosya yükleme alanı
        [Display(Name = "Profil Fotoğrafı")]
        public IFormFile? ProfilePicture { get; set; }



        public string? CompanyName { get; set; }

        public string? UserTypes { get; internal set; }

        public string? City { get; set; }
        public string? District { get; set; }
    }
}
