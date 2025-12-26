using Sevval.Domain.Entities;

namespace sevvalemlak.Dto
{
    public class IlanDuzenleDTO
    {
        public IlanModel Ilan { get; set; }
        public List<PhotoModel> _Fotograflar { get; set; }
        public List<IFormFile> UploadedFiles { get; set; }
        public List<VideoModel> _Videolar { get; set; }  // Videolar için ekleme
        public List<IFormFile> UploadedVideos { get; set; }  // Videolar için ekleme
        public int? DeleteVideoId { get; set; } // Silinecek video ID'si
    }
}
