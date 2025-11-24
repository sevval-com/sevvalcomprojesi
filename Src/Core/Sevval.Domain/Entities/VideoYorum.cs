using Sevval.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Web.Models
{
    public class VideoYorum
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VideoId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Yorum 1000 karakterden uzun olamaz.")]
        public string YorumMetni { get; set; }

        public DateTime YorumTarihi { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [ForeignKey("VideoId")]
        public virtual VideolarSayfasi Video { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Kullanici { get; set; }

        // Yanıtlama sistemi için kendi kendine referans
        public int? ParentYorumId { get; set; }

        [ForeignKey("ParentYorumId")]
        public virtual VideoYorum ParentYorum { get; set; }

        public virtual ICollection<VideoYorum> Yanitlar { get; set; } = new List<VideoYorum>();
    }
}

