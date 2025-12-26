using Sevval.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Web.Models
{
    public class VideoLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VideoId { get; set; }

        public string? UserId { get; set; }

        /// <summary>
        /// True = Like, False = Dislike
        /// </summary>
        [Required]
        public bool IsLike { get; set; }

        /// <summary>
        /// Kullanıcının IP adresi (login olmayan kullanıcılar için)
        /// </summary>
        public string? IpAddress { get; set; }

        [ForeignKey("VideoId")]
        public virtual VideolarSayfasi Video { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
