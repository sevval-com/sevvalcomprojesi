using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProjectName.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı boş bırakılamaz.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı boş bırakılamaz.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Görsel URL")]
        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Yayınlanma Tarihi")]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Display(Name = "Yazar")]
        [StringLength(100, ErrorMessage = "Yazar adı en fazla 100 karakter olabilir.")]
        public string? Author { get; set; }

        [Display(Name = "Kategori")]
        [StringLength(50, ErrorMessage = "Kategori en fazla 50 karakter olabilir.")]
        public string? Category { get; set; }

        [Display(Name = "Kısa Açıklama")]
        [StringLength(500, ErrorMessage = "Kısa açıklama en fazla 500 karakter olabilir.")]
        public string? ShortDescription { get; set; }
    }
}