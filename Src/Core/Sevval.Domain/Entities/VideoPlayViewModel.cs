using System.Collections.Generic;

namespace Sevval.Web.Models
{
    public class VideoPlayViewModel
    {
        public VideolarSayfasi Video { get; set; }
        public List<VideolarSayfasi> OneriVideolar { get; set; }
        public List<VideoYorum> Yorumlar { get; set; }

        /// <summary>
        /// Mevcut kullanıcının oyunu tutar. True: Like, False: Dislike, Null: Oy yok.
        /// </summary>
        public bool? CurrentUserVote { get; set; }
    }
}
