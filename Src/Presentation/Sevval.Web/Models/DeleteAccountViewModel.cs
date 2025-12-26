using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    /// <summary>
    /// Hesap silme işlemi için view model
    /// </summary>
    public class DeleteAccountViewModel
    {
        /// <summary>
        /// Kullanıcının mevcut şifresi (doğrulama için)
        /// </summary>
        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        /// <summary>
        /// Onay metni: "HESABIMI SIL" (case-sensitive)
        /// </summary>
        [Required(ErrorMessage = "Onay metni zorunludur")]
        [Display(Name = "Onay Metni")]
        [RegularExpression("HESABIMI SIL", ErrorMessage = "Onay metni tam olarak 'HESABIMI SIL' olmalıdır (büyük harfle)")]
        public string ConfirmationText { get; set; }

        /// <summary>
        /// Hesap silme sebebi (opsiyonel)
        /// </summary>
        [Display(Name = "Silme Nedeni")]
        public string DeletionReason { get; set; }
    }
}
