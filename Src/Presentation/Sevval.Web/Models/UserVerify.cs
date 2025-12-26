using Microsoft.AspNetCore.Mvc;

namespace sevvalemlak.Models
{
    //public class UserVerify : Controller
    //{
    //    public IActionResult Index()
    //    {
    //        return View();
    //    }
    //}
    public class KullaniciYetkisi
    {
        public int Id { get; set; }
        public string KullaniciKodu { get; set; }
        public bool YetkiDurumu { get; set; }
        public DateTime SonGuncelleme { get; set; }
    }
}
