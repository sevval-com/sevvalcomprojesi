using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Dtos.Front.Auth
{
    public class RegisterInput
    {

        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Sadece A-Z aralığında karakter girebilirsiniz."),
            Required(ErrorMessage = "İsim alanı boş geçilemez."),
           MinLength(3, ErrorMessage = "İsim alanı 3 karakterden az olamaz"),
            MaxLength(100, ErrorMessage = "İsim alanı 100 karakterden fazla olamaz")]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Sadece A-Z aralığında karakter girebilirsiniz."),
            Required(ErrorMessage = "Soyisim alanı boş geçilemez."),
            MinLength(3, ErrorMessage = "Soyisim alanı 3 karakterden az olamaz"),
            MaxLength(100, ErrorMessage = "Soyisim alanı 100 karakterden fazla olamaz")]

        public string Surname { get; set; }




        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Sadece A-Z aralığında karakter girebilirsiniz."),
            Required(ErrorMessage = "Ünvan alanı boş geçilemez."),
            MinLength(3, ErrorMessage = "Ünvan alanı 3 karakterden az olamaz"),
            MaxLength(100, ErrorMessage = "Ünvan alanı 100 karakterden fazla olamaz")]
        public string Title { get; set; }




        [Required(ErrorMessage = "E-Posta alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "E-Posta adresi uygun formatta değil")]
        [MinLength(3, ErrorMessage = "E-Posta alanı 3 karakterden az olamaz")]
        [MaxLength(100, ErrorMessage = "E-Posta alanı 100 karakterden fazla olamaz")]
        public string Mail { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefon alanı sadece sayılardan oluşmalıdır")]
        [Required(ErrorMessage = "Telefon alanı boş geçilemez.")]
        [Phone]
        [MinLength(10, ErrorMessage = "Telefon alanı 10 karakterden az olamaz"), MaxLength(15, ErrorMessage = "Telefon alanı 15 karakterden fazla olamaz")]
        public string Phone { get; set; } //[0-9]  

        [Required(ErrorMessage = "Şirket adı alanı boş geçilemez.")]
        [StringLength(200, ErrorMessage = "Şirket adı alanı 200 karakterden fazla olamaz")]

        public string CompanyName { get; set; }





        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Sadece A-Z aralığında karakter girebilirsiniz."),
            Required(ErrorMessage = "Şehir alanı boş geçilemez."),
            MinLength(3, ErrorMessage = "Şehir alanı 3 karakterden az olamaz"),
            MaxLength(20, ErrorMessage = "Şehir alanı 20 karakterden fazla olamaz")]
        public string City { get; set; }




        [Required(ErrorMessage = "Adres alanı boş geçilemez.")]
        [StringLength(500, ErrorMessage = "Adres  alanı 500 karakterden fazla olamaz")]
        public string Address { get; set; }

         public string Rechapta { get; set; }

    }

}
