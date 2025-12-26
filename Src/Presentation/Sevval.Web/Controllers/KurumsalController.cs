using Microsoft.AspNetCore.Mvc;

namespace sevvalemlak.Controllers
{
    public class KurumsalController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(KurumsalRegister model)
        {
            if (ModelState.IsValid)
            {
                // Veritabanına kaydetme işlemi burada yapılabilir.
                // Örnek:
                // _dbContext.KurumsalRegisters.Add(model);
                // _dbContext.SaveChanges();

                TempData["Message"] = "Üyelik bilgileriniz başarıyla kaydedildi.";
                return RedirectToAction("Success");
            }

            return View(model);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
