using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Dtos.Front.Auth
{
    public class LoginDto
    {

        [Required(ErrorMessage = "E-Posta alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "E-Posta adresi uygun formatta değil")]
        [MinLength(10, ErrorMessage = "E-Posta alanı 10 karakterden az olamaz")]
        [MaxLength(100, ErrorMessage = "E-Posta alanı 100 karakterden fazla olamaz")]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
            ErrorMessage = "Şifre 8-15 Karakter en az bir küçük harf,bir büyük harf,bir sayısal değer ve bir özel karakter içermelidir.")]
        [Required(ErrorMessage = "Şifre alanı boş geçilemez.")]
        public string Password { get; set; }
        public bool? IsRememberMe { get; set; }

        public string Message { get; set; }

    }
}
