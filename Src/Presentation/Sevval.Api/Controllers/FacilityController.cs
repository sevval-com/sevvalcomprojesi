using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.FacilityStatus.Queries.GetFacilityStatuses;
using Sevval.Application.Features.FacilityType.Queries.GetFacilityTypes;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FacilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tesis türlerini getirir (1-5 Yıldızlı Otel, Apart, Bungalov, Butik Otel, vb.)
    /// </summary>
    /// <returns>Tesis türleri listesi</returns>
    [HttpGet(GetFacilityTypesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Tesis türleri listesi", Description = "Tesis türlerini getirir")]

    public async Task<IActionResult> GetFacilityTypes([FromQuery] GetFacilityTypesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        
        if (response.IsSuccessfull)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }


    /// <summary>
    /// Tesis durumlarını getirir
    /// </summary>
    /// <returns>Tesis durumları listesi</returns>
    [HttpGet]
    [Route(GetFacilityStatusesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Tesis durumları listesi", Description = "Tesis durumlarını getirir (Satılık, Kiralık, Devren Satılık, Devren Kiralık)")]

    public async Task<IActionResult> GetFacilityStatuses([FromQuery] GetFacilityStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
