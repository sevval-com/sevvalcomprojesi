using System;

namespace Sevval.Domain.Entities
{
    /// <summary>
    /// Admin panelinde bekleyen videoları göstermek için ViewModel
    /// </summary>
    public class PendingVideoViewModel
    {
        public int Id { get; set; }
        public string VideoAdi { get; set; } = string.Empty;
        public string? KapakFotografiYolu { get; set; }
        public string? VideoYolu { get; set; }
        public bool IsYouTube { get; set; }
        public string UploaderName { get; set; } = string.Empty;
        public string UploaderEmail { get; set; } = string.Empty;
        public DateTime YuklenmeTarihi { get; set; }
        public string Kategori { get; set; } = string.Empty;
    }
}
