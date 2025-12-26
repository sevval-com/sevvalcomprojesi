using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class StepsController : Controller
    {
        public IActionResult Index()
        {
            // Adım verisi
            var steps = new List<string>
            {
                "Kategori Seçimi",
                "İlan Detayları",
                "Önizleme",
                "Doping",
                "Tebrikler"
            };

            // Veriyi View'a gönderme
            return View(steps);
        }
    }
}
