using GridBox.Solar.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia;
using Sevval.Application.Interfaces.Services;
using Sevval.Persistence.Context.sevvalemlak.Models;

namespace Sevval.Infrastructure.Services
{
    public class SocialMediaService : ISocialMediaService
    {
        private readonly IReadRepository<AboutUsContent> _readRepository;

        public SocialMediaService(IReadRepository<AboutUsContent> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<ApiResponse<GetSocialMediaQueryResponse>> GetSocialMediaAsync(CancellationToken cancellationToken)
        {
            // Default values
            var socialMedia = new GetSocialMediaQueryResponse
                {
                    Youtube = new SocialMediaPlatform
                    {
                        Name = "YouTube",
                        Url = "https://www.kisa.link/wmZoa",
                        ImagePath = "https://upload.wikimedia.org/wikipedia/commons/4/42/YouTube_icon_%282013-2017%29.png",
                        Title = "YouTube Kanalımızı Takip Edin",
                        Description = "Kaçırmak istemeyeceğiniz özel içerikler için hemen YouTube kanalımıza abone olun ve en yeni videoları ilk siz izleyin!"
                    },
                    Instagram = new SocialMediaPlatform
                    {
                        Name = "Instagram",
                        Url = "https://upload.wikimedia.org/wikipedia/commons/a/a5/Instagram_icon.png",
                        ImagePath = "",
                        Title = "Instagram Hesabımızı Keşfedin",
                        Description = "En güzel anları ve özel içerikleri kaçırmayın! Instagram'da bizi takip edin ve topluluğumuza katılın!"
                    },
                    Facebook = new SocialMediaPlatform
                    {
                        Name = "Facebook",
                        Url = "https://www.facebook.com/profile.php?id=61576226942639",
                        ImagePath = "https://upload.wikimedia.org/wikipedia/commons/5/51/Facebook_f_logo_%282019%29.svg",
                        Title = "Facebook Sayfamızı Ziyaret Edin",
                        Description = "En yeni duyurularımız ve etkinliklerimiz için Facebook'ta bizi takip edin ve topluluğumuzun bir parçası olun!"
                    },

                    TikTok = new SocialMediaPlatform
                    {
                        Name = "TikTok",
                        Url = "https://www.kisa.link/KWiFP",
                        ImagePath = "https://sevval.com/sablon/img/tiktok.webp",
                        Title = "TikTok Hesabımızı Takip Edin",
                        Description = "En eğlenceli ve yaratıcı içerikler için TikTok'ta bizi takip edin ve eğlencenin bir parçası olun!"
                    },
                    Google = new SocialMediaPlatform
                    {
                        Name = "Google İşletme",
                        Url = "https://www.google.com/search?q=%C5%9Eevval+Emlak",
                        ImagePath = "https://sevval.com/sablon/img/google.webp",
                        Title = "Google İşletme Profilimizi Ziyaret Edin",
                        Description = "İşletmemiz hakkında daha fazla bilgi almak için Google İşletme Profilimize göz atın ve müşteri yorumlarımızı inceleyin!"
                    },
                };

            // Load overrides from AboutUsContents where key starts with "social:"
            var overrideList = await _readRepository
                .Queryable()
                .AsNoTracking()
                .Where(c => c.Key.StartsWith("social:"))
                .ToListAsync(cancellationToken);

            foreach (var item in overrideList)
            {
                // Expected key format: social:{platform}:{field}
                var parts = item.Key.Split(':');
                if (parts.Length != 3) continue;

                var platform = parts[1].Trim().ToLowerInvariant();
                var field = parts[2].Trim().ToLowerInvariant();
                var value = item.Content ?? string.Empty;

                SocialMediaPlatform target = platform switch
                {
                    "facebook" => socialMedia.Facebook,
                    "instagram" => socialMedia.Instagram,
                    "youtube" => socialMedia.Youtube,
                    "google" => socialMedia.Google,
                    "tiktok" => socialMedia.TikTok, // accept lowercase tiktok
                    _ => null
                };

                if (target == null) continue;

                switch (field)
                {
                    case "title": target.Title = value; break;
                    case "description": target.Description = value; break;
                    case "url": target.Url = value; break;
                    case "imagepath": target.ImagePath = value; break;
                }
            }

            return new ApiResponse<GetSocialMediaQueryResponse>
            {
                Code = 200,
                Data = socialMedia,
                IsSuccessfull = true,
                Message = "Sosyal medya bilgileri başarıyla getirildi."
            };
        }
    }
}
