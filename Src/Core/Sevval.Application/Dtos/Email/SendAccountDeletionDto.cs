namespace Sevval.Application.Dtos.Email
{
    /// <summary>
    /// Hesap silme bildirimi email DTO'su
    /// </summary>
    public class SendAccountDeletionDto
    {
        /// <summary>
        /// Alıcı email adresi
        /// </summary>
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// Alıcı adı soyadı
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Hesap silinme tarihi
        /// </summary>
        public DateTime DeletionDate { get; set; }

        /// <summary>
        /// Hesap kurtarma son tarihi (genelde 30 gün sonra)
        /// </summary>
        public DateTime RecoveryDeadline { get; set; }

        /// <summary>
        /// Hesap silme sebebi (opsiyonel)
        /// </summary>
        public string DeletionReason { get; set; }

        /// <summary>
        /// Hesap kurtarma token'ı (email linkinde kullanılır)
        /// </summary>
        public string RecoveryToken { get; set; }

        /// <summary>
        /// Kullanıcı ID'si
        /// </summary>
        public string UserId { get; set; }
    }
}
