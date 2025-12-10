using System.ComponentModel.DataAnnotations;

namespace Sevval.Web.Models.Api
{
    /// <summary>
    /// Mobil uygulama için hesap silme request modeli
    /// </summary>
    public class DeleteAccountRequest
    {
        /// <summary>
        /// Kullanıcının mevcut şifresi (doğrulama için)
        /// </summary>
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }

        /// <summary>
        /// Onay metni - "HESABIMI SIL" olmalı
        /// </summary>
        [Required(ErrorMessage = "Onay metni zorunludur")]
        public string ConfirmationText { get; set; }

        /// <summary>
        /// Kullanıcının hesap silme nedeni (opsiyonel)
        /// </summary>
        public string? DeletionReason { get; set; }
    }

    /// <summary>
    /// Hesap silme başarılı response modeli
    /// </summary>
    public class DeleteAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime? DeletionDate { get; set; }
        public DateTime? RecoveryDeadline { get; set; }
        public string? RecoveryToken { get; set; }
    }

    /// <summary>
    /// API hata response modeli
    /// </summary>
    public class ApiErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}
