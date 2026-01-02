using Microsoft.AspNetCore.Mvc;

public class DroneController : Controller
{
    public IActionResult Index() { return RedirectToAction(nameof(ParselSorgu)); }
    public IActionResult Olustur() { return View(); }
    public IActionResult ParselSorgu() { return View(); }
    public IActionResult KayitliParseller() { return View(); }
    public IActionResult ArsaVideolarim() { return View(); }
}
