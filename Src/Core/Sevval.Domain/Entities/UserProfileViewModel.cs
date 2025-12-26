using Sevval.Domain.Entities;
using System.Collections.Generic;

namespace Sevval.Web.Models
{
    /// <summary>
    /// Kullanıcının profil sayfasında gösterilecek verileri tutar.
    /// </summary>
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<VideolarSayfasi> Videos { get; set; }
        public int TotalVideos { get; set; }
    }
}
