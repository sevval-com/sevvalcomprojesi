using Sevval.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Web.Models
{
    [Table("VideolarSayfasi")]
    public class VideolarSayfasi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Video başlığı zorunludur.")]
        [StringLength(200)]
        public string VideoAdi { get; set; }

        [StringLength(1000)]
        public string VideoAciklamasi { get; set; }

        [Required]
        public string VideoYolu { get; set; }

        public bool IsYouTube { get; set; }

        public string? KapakFotografiYolu { get; set; }

        public DateTime YuklenmeTarihi { get; set; } = DateTime.UtcNow;

        public int BegeniSayisi { get; set; } = 0;

        public int DislikeSayisi { get; set; } = 0;

        public int GoruntulenmeSayisi { get; set; } = 0;

        [StringLength(100)]
        public string Kategori { get; set; }

        [Required]
        public string YukleyenKullaniciId { get; set; }

        [ForeignKey("YukleyenKullaniciId")]
        public virtual ApplicationUser YukleyenKullanici { get; set; }

        // Navigation Properties
        public virtual ICollection<VideoLike> LikesDislikes { get; set; } = new List<VideoLike>();
        public virtual ICollection<VideoYorum> Yorumlar { get; set; } = new List<VideoYorum>();
    }
}
