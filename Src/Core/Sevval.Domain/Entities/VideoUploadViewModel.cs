using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sevval.Web.Models
{
    public class VideoUploadViewModel
    {
        // Edit işlemleri için Id (Upload'ta kullanılmaz)
        public int? Id { get; set; }
        [Required(ErrorMessage = "Video başlığı zorunludur.")]
        [StringLength(200)]
        public string VideoAdi { get; set; }

        [StringLength(1000)]
        public string VideoAciklamasi { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Kategori zorunludur.")]
        public string Kategori { get; set; }

        // Lokal dosya yükleme için
        public IFormFile? VideoDosyasi { get; set; }

        // YouTube linki için
        public string? YouTubeLink { get; set; }

        // Kapak fotoğrafı için
        public IFormFile? KapakFotografi { get; set; }
    }

    public class VideoEditViewModel : VideoUploadViewModel
    {
        // Mevcut veriyi göstermek için ek alanlar
        public string? ExistingCoverPath { get; set; }
        public string? ExistingVideoPath { get; set; }
        public bool ExistingIsYouTube { get; set; }
    }
}
