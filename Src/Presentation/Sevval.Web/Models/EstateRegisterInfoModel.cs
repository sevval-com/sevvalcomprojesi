using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models;

public class EstateRegisterInfoModel
{
    [Required(ErrorMessage = "Ad alanını doldurmak zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Soyad alanını doldurmak zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "E-Posta alanını doldurmak zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telefon numarası alanını doldurmak zorunludur.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Şifre alanını doldurmak zorunludur.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre  en az 8 karakter olmalıdır.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Şifre tekrarı alanını doldurmak zorunludur.")]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre tekrarı en az 8 karakter olmalıdır.")]
    public string ConfirmPassword { get; set; }

    [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir.")]
    [Required(ErrorMessage = "Şirket adı alanını doldurmak zorunludur.")]
    public string CompanyName { get; set; }

    [StringLength(50, ErrorMessage = "Şehir en fazla 50 karakter olabilir.")]
    [Required(ErrorMessage = "Şehir alanını doldurmak zorunludur.")]
    public string City { get; set; }

    [StringLength(50, ErrorMessage = "İlçe en fazla 50 karakter olabilir.")]
    [Required(ErrorMessage = "İlçe alanını doldurmak zorunludur.")]

    public string District { get; set; }

    [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
    [Required(ErrorMessage = "Adres alanını doldurmak zorunludur.")]

    public string Address { get; set; }

    [Required(ErrorMessage = "5. Seviye Sertifika yüklemek zorunludur.")]
    public IFormFile Level5Certificate { get; set; }

    [Required(ErrorMessage = "Vergi Levhası yüklemek zorunludur.")]
    public IFormFile TaxPlate { get; set; }

    public string? Level5CertificatePath { get; set; }

    public string? TaxPlatePath { get; set; }

    [StringLength(100, ErrorMessage = "Referans en fazla 100 karakter olabilir.")]
    public string? Reference { get; set; }

    public IFormFile? ProfilePicture { get; set; }

    public string? ProfilePicturePath { get; set; }

    [Required(ErrorMessage = "Kullanıcı türü seçmek zorunludur.")]
    public string UserTypes { get; set; }

   
}
