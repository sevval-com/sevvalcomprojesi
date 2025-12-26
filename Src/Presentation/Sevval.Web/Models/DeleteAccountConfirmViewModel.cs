using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    /// <summary>
    /// Hesap silme onay modal için view model
    /// </summary>
    public class DeleteAccountConfirmViewModel
    {
        /// <summary>
        /// Silinecek hesabın email adresi
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Silinecek hesabın adı soyadı
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Kullanıcının toplam ilan sayısı
        /// </summary>
        public int TotalAdsCount { get; set; }

        /// <summary>
        /// Kullanıcının okunmamış mesaj sayısı
        /// </summary>
        public int UnreadMessagesCount { get; set; }

        /// <summary>
        /// Hesap kurtarma için geçerli süre (gün)
        /// </summary>
        public int RecoveryPeriodDays { get; set; } = 30;
    }
}
