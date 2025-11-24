// Models/BireyselKullaniciViewModel.cs
using Sevval.Domain.Entities;

namespace sevvalemlak.Models
{
    public class BireyselKullaniciViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<ConsultantInvitation> ConsultantInvitations { get; set; }

        // Filtreleme için ek alanlar:
        public IEnumerable<string> DistinctReferansValues { get; set; }
        public string FilterReferans { get; set; }
        public DateTime? FilterRegistrationFrom { get; set; }
        public DateTime? FilterRegistrationTo { get; set; }

        // Yeni özellik: Kullanıcının ilan sayısı
        public Dictionary<string, int> UserIlanSayilari { get; set; }

        // Yeni özellikler: Toplam kullanıcı ve ilan sayısı
        public int ToplamKullaniciSayisi { get; set; }
        public int ToplamIlanSayisi { get; set; }


    }
}