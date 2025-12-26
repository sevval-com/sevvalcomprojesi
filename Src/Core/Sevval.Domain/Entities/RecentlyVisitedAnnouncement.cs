using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Domain.Entities
{
    public class RecentlyVisitedAnnouncement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
     

        [Required]
        public DateTime VisitedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int IlanId { get; set; }

        [ForeignKey("IlanId")]
        public virtual IlanModel Ilan { get; set; }

        public string Province { get; set; } //ilanýn sehir bilgisi
        public string Property { get; set; } //ilanýn türü, arsa,konut,...

    }
}
