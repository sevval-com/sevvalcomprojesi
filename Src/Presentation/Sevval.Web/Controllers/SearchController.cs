using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Persistence.Context;
using sevvalemlak.Dto;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Arama sayfasına yönlendirme
    public async Task<IActionResult> IndexAsync(string q)
    {
        var ilanlar = await _context.IlanBilgileri.AsNoTracking().ToListAsync(); // başarılı ilnalar geliyor

        var fotograflar = await _context.Photos.AsNoTracking().ToListAsync(); // başarısız fotoğraflar gelmiyor ???? 

        TumIlanlarDTO tumIlanlarDTO = new TumIlanlarDTO
        {
            _Fotograflar = fotograflar,
            _Ilanlar = ilanlar
        };

        return View(tumIlanlarDTO);
    }
}