using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements;
using Sevval.Application.Features.Announcement.Queries.SearchAnnouncements;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnnouncementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(GetAnnouncementCountQueryRequest.Route)]
        [SwaggerOperation(Summary = "İlan sayısını getirir", Description = "Belirtilen kriterlere göre ilan sayısını döndürür. Status ve UserEmail parametreleri opsiyoneldir.")]
        public async Task<IActionResult> GetAnnouncementCount([FromQuery]GetAnnouncementCountQueryRequest request, CancellationToken cancellationToken = default)
        {
           
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetAnnouncementCountByTypeQueryRequest.Route)]
        [SwaggerOperation(Summary = "İlan sayılarını türe göre getirir", Description = "Belirtilen kriterlere göre ilan sayılarını türe göre gruplandırarak döndürür. Status ve UserEmail parametreleri opsiyoneldir.")]
        public async Task<IActionResult> GetAnnouncementCountByType([FromQuery] GetAnnouncementCountByTypeQueryRequest request, CancellationToken cancellationToken = default)
        {
           
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet(GetAnnouncementCountByProvinceQueryRequest.Route)]
        [SwaggerOperation(Summary = "İlan sayılarını ile göre getirir", Description = "Belirtilen kriterlere göre ilan sayılarını ile göre gruplandırarak döndürür. Status ve UserEmail parametreleri opsiyoneldir.")]
        public async Task<IActionResult> GetAnnouncementCountByProvince([FromQuery] GetAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken = default)
        {
         
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet(GetTodaysAnnouncementsQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "Günün ilanını getirir", 
            Description = @"Bugün eklenen en son ilanı ve toplam günlük ilan sayısını döndürür. 
            
            📊 ÖZELLIKLER:
            • Status parametresi opsiyoneldir (default: 'active')
            • DeviceId parametresi opsiyoneldir - gönderilirse aynı cihaz 24 saat içinde tekrar sayılmaz
            • Her çağrıda görüntülenme sayacı otomatik artırılır (benzersiz cihazlar için)
            • Hem GununIlanlari hem de IlanBilgileri tablosundaki sayaçlar güncellenir")]
        public async Task<IActionResult> GetTodaysAnnouncements([FromQuery] GetTodaysAnnouncementsQueryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet(GetAnnouncementsByCompanyQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "Firmaya ait ilanları getirir",
            Description = "Belirtilen firma adına ait tüm ilanları sayfalama ve sıralama desteği ile döndürür."
        )]
        public async Task<IActionResult> GetAnnouncementsByCompany([FromQuery] GetAnnouncementsByCompanyQueryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet(SearchAnnouncementsQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "İlan arama ve filtreleme", 
            Description = "Hem basit anahtar kelime araması hem de detaylı filtreleme seçeneklerini destekleyen kapsamlı ilan arama endpoint'i. Sayfalama, sıralama ve çoklu filtre desteği içerir."
        )]
        public async Task<IActionResult> SearchAnnouncements([FromQuery] SearchAnnouncementsQueryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetCompanyAnnouncementCountByProvinceQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "Firmaya ait ilanların il bazında sayısını getirir",
            Description = "Belirtilen şirket adına ait ilanları illere göre gruplandırarak her ilde kaç ilan olduğunu döndürür. CompanyName parametresi zorunludur, Status parametresi opsiyoneldir."
        )]
        public async Task<IActionResult> GetCompanyAnnouncementCountByProvince([FromQuery] GetCompanyAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetAnnouncementsByUserQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "Kullanıcıya ait ilanları getirir",
            Description = "Belirtilen e-posta adresine ait kullanıcının tüm ilanlarını sayfalama desteği ile getirir. E-posta parametresi zorunludur, diğer parametreler opsiyoneldir."
        )]
        public async Task<IActionResult> GetAnnouncementsByUser([FromQuery] GetAnnouncementsByUserQueryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetAnnouncementDetailsQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "İlan detaylarını getirir",
            Description = "Belirtilen ID'ye sahip ilanın tüm detaylarını getirir. Fotoğraflar, videolar, kullanıcı bilgileri, ilgili ilanlar ve şirket detayları dahildir."
        )]
        public async Task<IActionResult> GetAnnouncementDetails([FromRoute]int id, CancellationToken cancellationToken = default)
        {            
            var response = await _mediator.Send(new GetAnnouncementDetailsQueryRequest() { Id=id}, cancellationToken);
            return Ok(response);
        }

        
    }
}
