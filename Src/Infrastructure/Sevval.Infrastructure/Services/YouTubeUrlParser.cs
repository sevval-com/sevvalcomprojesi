using Sevval.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sevval.Infrastructure.Services
{
    /// <summary>
    /// YouTube URL'lerini parse eden servis
    /// Desteklenen formatlar:
    /// - https://www.youtube.com/watch?v=VIDEO_ID
    /// - https://youtu.be/VIDEO_ID
    /// - https://www.youtube.com/embed/VIDEO_ID
    /// - URL'ler timestamp, playlist ve diğer parametrelerle birlikte
    /// </summary>
    public class YouTubeUrlParser : IYouTubeUrlParser
    {
        // YouTube video ID pattern: 11 karakter, alfanumerik + tire ve alt çizgi
        private static readonly Regex VideoIdPattern = new Regex(
            @"^[a-zA-Z0-9_-]{11}$",
            RegexOptions.Compiled);

        // Tüm YouTube URL formatlarını yakalayan pattern
        private static readonly Regex YouTubeUrlPattern = new Regex(
            @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:watch\?(?:.*&)?v=|embed\/|v\/)|youtu\.be\/)([a-zA-Z0-9_-]{11})(?:[?&].*)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <inheritdoc/>
        public string? ExtractVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            url = url.Trim();

            // Direkt video ID kontrolü (11 karakter)
            if (VideoIdPattern.IsMatch(url))
                return url;

            // URL pattern ile eşleştir
            var match = YouTubeUrlPattern.Match(url);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <inheritdoc/>
        public bool IsValidYouTubeUrl(string url)
        {
            return ExtractVideoId(url) != null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetSupportedFormats()
        {
            return new[]
            {
                "https://www.youtube.com/watch?v=VIDEO_ID",
                "https://youtu.be/VIDEO_ID",
                "https://www.youtube.com/embed/VIDEO_ID",
                "https://www.youtube.com/watch?v=VIDEO_ID&t=120 (zaman damgası ile)",
                "https://www.youtube.com/watch?v=VIDEO_ID&list=PLAYLIST_ID (playlist ile)"
            };
        }
    }
}
