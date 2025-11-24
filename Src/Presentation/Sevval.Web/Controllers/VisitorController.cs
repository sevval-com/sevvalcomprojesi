using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount;
using Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount;
using Sevval.Persistence.Context;
using sevvalemlak.csproj.ClientServices.VisitoryServices;

namespace sevvalemlak.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IVisitoryClientService _visitoryClientService;
        public VisitorController(ApplicationDbContext context, IVisitoryClientService visitoryClientService)
        {
            _context = context;
            _visitoryClientService = visitoryClientService;
        }



        // Aktif ziyaretçi sayısını döndüren metod
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveVisitors()
        {
            var result = await _visitoryClientService.GetActiveVisitorCount(new GetActiveVisitorCountQueryRequest(), CancellationToken.None);

            return Ok(result);

        }

        // Toplam ziyaretçi sayısını döndüren metod
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalVisitors()
        {
            var result = await _visitoryClientService.GetTotalVisitorCount(new GetTotalVisitorCountQueryRequest(), CancellationToken.None);

            return Ok(result);
        }

        // Kullanıcı siteye girdiğinde çağrılan metod
        [HttpPost("enter")]
        public async Task<IActionResult> Enter()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            var result = await _visitoryClientService.IncreaseVisitorCount(new IncreaseVisitorCountCommandRequest()
            {
                IpAddress = ipAddress,
            }, CancellationToken.None);

            return Ok(result);
        }

        // Kullanıcı siteyi kapattığında çağrılan metod
        [HttpPost("exit")]
        public async Task<IActionResult> Exit()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            var result = await _visitoryClientService.DecreaseVisitorCount(new DecreaseVisitorCountCommandRequest()
            {
                IpAddress = ipAddress,
            }, CancellationToken.None);

            return Ok(result);
        }








        // Haftalık aktif ziyaretçi sıfırlaması (şimdi 3 günlük)
        [HttpPost("reset-active")]
        public IActionResult ResetActiveVisitors()
        {
            // 3 günden eski ziyaretçileri bul ve sil
            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);
            var outdatedVisitors = _context.Visitors.Where(v => v.VisitTime <= threeDaysAgo);

            _context.Visitors.RemoveRange(outdatedVisitors);
            var visitorCount = _context.VisitorCounts.FirstOrDefault();
            if (visitorCount != null)
            {
                // Aktif ziyaretçi sayısını güncel kalan ziyaretçi sayısıyla eşitle
                visitorCount.ActiveVisitors = _context.Visitors.Count();
            }

            _context.SaveChanges();
            return Ok();
        }
    }
}