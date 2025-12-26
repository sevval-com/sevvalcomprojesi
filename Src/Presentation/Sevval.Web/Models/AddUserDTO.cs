using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sevval.Web.Models
{
    public class AddUserDTO
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [MinLength(8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Parola tekrar alanı zorunludur.")]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        [MinLength(8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]

        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Kullanıcı tipi seçimi zorunludur.")]
        public string UserTypes { get; set; }

        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Abonelik durumu seçimi zorunludur.")]
        public string IsSubscribed { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Address { get; set; }

        public string Reference { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public IFormFile? Level5Certificate { get; set; }

        public IFormFile? TaxPlate { get; set; }

       
    }
}
