using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;

namespace Sevval.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/v1/recently-visited-announcements")]
    public class RecentlyVisitedAnnouncementController : BaseController
    {
        private readonly IMediator _mediator;

        public RecentlyVisitedAnnouncementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Kullanıcının son gezdiği ilanları getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="limit">Getirilecek ilan sayısı (varsayılan: 10, maksimum: 50)</param>
        /// <returns>Son gezilen ilanların listesi</returns>
        [HttpGet(GetRecentlyVisitedAnnouncementQueryRequest.Route)]
        [SwaggerOperation(Summary = "Kullanıcının son gezdiği ilanları getirir", Description = "Kullanıcının son gezdiği ilanları getirir. Kullanıcı ID'si ve limit parametreleri ile kullanılır.")]
        public async Task<IActionResult> GetRecentlyVisitedAnnouncements([FromQuery] GetRecentlyVisitedAnnouncementQueryRequest request, CancellationToken cancellationToken)
        {
            request.UserId =request.UserId ?? GetCurrentUserId();
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcının ziyaret ettiği ilanı son gezilen listesine ekler
        /// </summary>
        /// <param name="request">İlan bilgileri ve kullanıcı ID'si</param>
        /// <returns>Ekleme işleminin sonucu</returns>
        [HttpPost(AddRecentlyVisitedAnnouncementCommandRequest.Route)]
        public async Task<IActionResult> AddRecentlyVisitedAnnouncement([FromBody] AddRecentlyVisitedAnnouncementCommandRequest request,CancellationToken cancellationToken)
        {
            request.UserId = request.UserId ?? GetCurrentUserId();
           var response = await _mediator.Send(request,cancellationToken);
            return Ok(response);
        }



        [HttpGet(GetSuitableAnnouncementsQueryRequest.Route)]
        [SwaggerOperation(
           Summary = "Size uygun ilanları getirir",
           Description = "Kullanıcının tercihlerine göre uygun ilanları getirir. Kategori ve diğer kriterlere göre filtreleme yapar. Uygunluk skoru ile sıralama desteği içerir."
       )]
        public async Task<IActionResult> GetSuitableAnnouncements([FromQuery] GetSuitableAnnouncementsQueryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
