using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Entities
{
    /// <summary>
    /// Tracks deleted user accounts for 30-day recovery window.
    /// Used to calculate deletion date for automatic restoration on login
    /// and permanent deletion after grace period.
    /// </summary>
    public class DeletedAccount : IBaseEntity<int>
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to ApplicationUser.Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// UTC timestamp when account was deleted
        /// </summary>
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// Optional reason provided by user for deletion
        /// </summary>
        public string? DeletionReason { get; set; }

        /// <summary>
        /// Unique recovery token for email link-based account restoration
        /// </summary>
        public string? RecoveryToken { get; set; }

        /// <summary>
        /// Navigation property to ApplicationUser
        /// </summary>
        public virtual ApplicationUser User { get; set; }
    }
}
