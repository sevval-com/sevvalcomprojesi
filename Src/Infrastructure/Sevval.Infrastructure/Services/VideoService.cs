using Sevval.Application.Features.Common;
using Sevval.Application.Features.Video.Queries.GetVideos;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Infrastructure.Services
{
    public class VideoService : IVideoService
    {
        public async Task<ApiResponse<List<GetVideosQueryResponse>>> GetVideosAsync(GetVideosQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {

                return new ApiResponse<List<GetVideosQueryResponse>>()
                {
                    Data = GetStaticVideoData(),
                    IsSuccessfull = true,
                    Message = "Videolar basariyla getirildi."

                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetVideosQueryResponse>>()
                {
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Videolar getirilirken bir hata olustu: "
                };
            }
        }


        private List<GetVideosQueryResponse> GetStaticVideoData()
        {
            return new List<GetVideosQueryResponse>
            {
                // Tanıtım Category
                new GetVideosQueryResponse
                {
                    Id = 1,
                    Title = "Sevval.com Tanıtım Videosu 1",
                    Description = "Sevval.com emlak platformu tanıtım videosu",
                    VideoUrl = "https://www.youtube.com/embed/aHXV-8gvZ8s",
                    ThumbnailUrl = "https://img.youtube.com/vi/aHXV-8gvZ8s/maxresdefault.jpg",
                    Category = "Tanıtım",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-30),
                    ViewCount = 1250,
                    Duration = 180
                },
                new GetVideosQueryResponse
                {
                    Id = 2,
                    Title = "Sevval.com Tanıtım Videosu 2",
                    Description = "Sevval.com emlak platformu tanıtım videosu",
                    VideoUrl = "https://www.youtube.com/embed/QWS0snIZICk",
                    ThumbnailUrl = "https://img.youtube.com/vi/QWS0snIZICk/maxresdefault.jpg",
                    Category = "Tanıtım",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-25),
                    ViewCount = 980,
                    Duration = 165
                },

                // Bilgi Category
                new GetVideosQueryResponse
                {
                    Id = 3,
                    Title = "Emlak Bilgi Videosu 1",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/PBzoK-ZUWJU",
                    ThumbnailUrl = "https://img.youtube.com/vi/PBzoK-ZUWJU/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20),
                    ViewCount = 2150,
                    Duration = 240
                },
                new GetVideosQueryResponse
                {
                    Id = 4,
                    Title = "Emlak Bilgi Videosu 2",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/nBVAEh-zkb4",
                    ThumbnailUrl = "https://img.youtube.com/vi/nBVAEh-zkb4/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-18),
                    ViewCount = 1890,
                    Duration = 195
                },
                new GetVideosQueryResponse
                {
                    Id = 5,
                    Title = "Emlak Bilgi Videosu 3",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/vlkXht4rUWc",
                    ThumbnailUrl = "https://img.youtube.com/vi/vlkXht4rUWc/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-15),
                    ViewCount = 1650,
                    Duration = 220
                },
                new GetVideosQueryResponse
                {
                    Id = 6,
                    Title = "Emlak Bilgi Videosu 4",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/ePYBvVNceBw",
                    ThumbnailUrl = "https://img.youtube.com/vi/ePYBvVNceBw/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-12),
                    ViewCount = 1420,
                    Duration = 210
                },
                new GetVideosQueryResponse
                {
                    Id = 7,
                    Title = "Emlak Bilgi Videosu 5",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/ZoEwFed6JUM",
                    ThumbnailUrl = "https://img.youtube.com/vi/ZoEwFed6JUM/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-10),
                    ViewCount = 1780,
                    Duration = 185
                },
                new GetVideosQueryResponse
                {
                    Id = 8,
                    Title = "Emlak Bilgi Videosu 6",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/mnKbZOPHIL8",
                    ThumbnailUrl = "https://img.youtube.com/vi/mnKbZOPHIL8/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-8),
                    ViewCount = 1320,
                    Duration = 205
                },
                new GetVideosQueryResponse
                {
                    Id = 9,
                    Title = "Emlak Bilgi Videosu 7",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/LUPlcih8x2c",
                    ThumbnailUrl = "https://img.youtube.com/vi/LUPlcih8x2c/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-6),
                    ViewCount = 1580,
                    Duration = 175
                },
                new GetVideosQueryResponse
                {
                    Id = 10,
                    Title = "Emlak Bilgi Videosu 8",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/YxvWGiaf4HU",
                    ThumbnailUrl = "https://img.youtube.com/vi/YxvWGiaf4HU/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-4),
                    ViewCount = 1890,
                    Duration = 230
                },
                new GetVideosQueryResponse
                {
                    Id = 11,
                    Title = "Emlak Bilgi Videosu 9",
                    Description = "Emlak sektörü hakkında bilgilendirici video",
                    VideoUrl = "https://www.youtube.com/embed/lB1Io0mIe9U",
                    ThumbnailUrl = "https://img.youtube.com/vi/lB1Io0mIe9U/maxresdefault.jpg",
                    Category = "Bilgi",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-2),
                    ViewCount = 1120,
                    Duration = 190
                },

                // Reklam Category
                new GetVideosQueryResponse
                {
                    Id = 12,
                    Title = "Sevval.com Reklam Videosu",
                    Description = "Sevval.com reklam videosu",
                    VideoUrl = "https://www.youtube.com/embed/KMtdRLa_i38",
                    ThumbnailUrl = "https://img.youtube.com/vi/KMtdRLa_i38/maxresdefault.jpg",
                    Category = "Reklam",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-35),
                    ViewCount = 3250,
                    Duration = 30
                },

                // Bunu Biliyor Muydunuz? Category
                new GetVideosQueryResponse
                {
                    Id = 13,
                    Title = "Bunu Biliyor Muydunuz? - Emlak Bilgisi",
                    Description = "Emlak sektörü hakkında ilginç bilgiler",
                    VideoUrl = "https://www.youtube.com/embed/r8yufLx3VE4",
                    ThumbnailUrl = "https://img.youtube.com/vi/r8yufLx3VE4/maxresdefault.jpg",
                    Category = "Bunu Biliyor Muydunuz?",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-14),
                    ViewCount = 2890,
                    Duration = 120
                },

                // Örnek İlan Videoları Category
                new GetVideosQueryResponse
                {
                    Id = 14,
                    Title = "Örnek İlan Videosu",
                    Description = "Emlak ilanı örnek video",
                    VideoUrl = "https://www.youtube.com/embed/NNQvJADgBME",
                    ThumbnailUrl = "https://img.youtube.com/vi/NNQvJADgBME/maxresdefault.jpg",
                    Category = "Örnek İlan Videoları",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-7),
                    ViewCount = 1750,
                    Duration = 300
                }
            };
        }
    }




}
