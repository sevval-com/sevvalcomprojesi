using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    public class Step2Model
    {
        [Required(ErrorMessage = "Lütfen ilan başlığını giriniz.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Lütfen açıklama kısmını doldurunuz.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Lütfen fiyat bilgisi giriniz.")]
        [Range(0, double.MaxValue, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Lütfen en az bir fotoğraf yükleyin.")]
        public List<IFormFile> Photos { get; set; }  // Birden fazla fotoğraf için liste

        public IFormFile Video { get; set; }  // Video dosyası

        // Video dosyası yerine video linki eklemek istiyorsanız:
        public string VideoLink { get; set; }  // Video Linki

        // Kategori seçimi
        public string SelectedCategory { get; set; }  // Kullanıcının seçtiği kategori
    }
}
