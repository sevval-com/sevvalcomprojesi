using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.IService.Common;

namespace Sevval.Web.Controllers.Api;

[ApiController]
[Route("api/v1/announcements")]
public class AnnouncementsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AnnouncementsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnnouncementById(int id, CancellationToken cancellationToken)
    {
        var announcement = await _context.IlanBilgileri
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new
            {
                i.Id,
                i.Title,
                i.Price,
                i.IlanNo,
                i.sehir,
                i.semt,
                i.Konum,
                i.Area,
                i.AdaNo,
                i.ParselNo,
                i.MulkTipi,
                i.MulkTipiArsa,
                i.ArsaDurumu,
                i.Category,
                i.FirstName,
                i.LastName,
                i.Email,
                i.PhoneNumber
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (announcement == null)
        {
            return NotFound();
        }

        return Ok(new { Announcement = announcement });
    }
}
