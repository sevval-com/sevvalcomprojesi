using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sevval.Domain.Entities;

public class FirmalarController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public FirmalarController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Firmalar()
    {
        // Veritabanından sadece City dolu olan kullanıcıları alıyoruz
        var usersWithCity = _userManager.Users
            .Where(u => !string.IsNullOrEmpty(u.City))
            .ToList();

        // Filtrelenmiş kullanıcıları view'e gönderiyoruz
        return View(usersWithCity);
    }



}
