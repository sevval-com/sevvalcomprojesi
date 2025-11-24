using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sevval.Domain.Entities;

public class KardesFirmalarController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public KardesFirmalarController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult KardesFirmalar()
    {
        // Veritabanından sadece City dolu olan kullanıcıları alıyoruz
        var usersWithCity = _userManager.Users
            .Where(u => !string.IsNullOrEmpty(u.City))
            .ToList();

        // Filtrelenmiş kullanıcıları view'e gönderiyoruz
        return View(usersWithCity);
    }



}
