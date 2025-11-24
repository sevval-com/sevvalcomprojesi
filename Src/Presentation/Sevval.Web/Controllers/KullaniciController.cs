using Microsoft.AspNetCore.Mvc;

namespace sevvalemlak.Controllers
{
    public class KullaniciController : Controller
    {
        // KullaniciPaneli action metodu
        public IActionResult KullaniciPaneli()
        {
            // "kullanicipaneli" dosyasını arar
            return View("kullanicipaneli");
        }
    }
}
