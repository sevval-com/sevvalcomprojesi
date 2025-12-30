using System.Collections.Generic;

namespace Sevval.Application.Interfaces.Services
{
    /// <summary>
    /// YouTube URL'lerini parse etmek için interface
    /// </summary>
    public interface IYouTubeUrlParser
    {
        /// <summary>
        /// Herhangi bir YouTube URL formatından video ID'sini çıkarır
        /// </summary>
        /// <param name="url">YouTube URL'i</param>
        /// <returns>Video ID veya geçersiz URL için null</returns>
        string? ExtractVideoId(string url);

        /// <summary>
        /// URL'in geçerli bir YouTube URL'i olup olmadığını kontrol eder
        /// </summary>
        /// <param name="url">Kontrol edilecek URL</param>
        /// <returns>Geçerli ise true</returns>
        bool IsValidYouTubeUrl(string url);

        /// <summary>
        /// Desteklenen URL formatlarını döndürür (tooltip için)
        /// </summary>
        /// <returns>Desteklenen format listesi</returns>
        IEnumerable<string> GetSupportedFormats();
    }
}
