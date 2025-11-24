using Sevval.Domain.Entities;
using System.Collections.Generic;

namespace sevvalemlak.Dto
{
    /// <summary>
    /// İlan Yönetim sayfasında kullanılacak verileri bir arada tutan model.
    /// </summary>
    public class IlanYonetimDTO
    {
        public List<IlanYonetimItem> Ilanlar { get; set; }
        public List<ApplicationUser> Firmalar { get; set; }
        public List<ApplicationUser> Danismanlar { get; set; }

        // Filtreleme alanları
        public string IlanNo { get; set; }
        public string FirmaId { get; set; }
        public string DanismanEmail { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string IlanSahibiArama { get; set; }

        // Sayfalama
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public IlanYonetimDTO()
        {
            Ilanlar = new List<IlanYonetimItem>();
            Firmalar = new List<ApplicationUser>();
            Danismanlar = new List<ApplicationUser>();
        }
    }

    /// <summary>
    /// Yönetim sayfasındaki listede her bir ilanın ilişkili verilerle gösterimi için model.
    /// </summary>
    public class IlanYonetimItem
    {
        public IlanModel Ilan { get; set; }
        public ApplicationUser IlanSahibi { get; set; }
        public string FirmaAdi { get; set; }
        public PhotoModel VitrinFotografi { get; set; }
    }
}
