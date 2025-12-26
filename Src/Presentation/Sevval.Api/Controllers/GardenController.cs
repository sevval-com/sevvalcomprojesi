using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;
using Sevval.Application.Features.GardenType.Queries.GetGardenTypes;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GardenController : ControllerBase
{
    private readonly IMediator _mediator;

    public GardenController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Bahçe durumlarını getirir (Satılık, Kiralık, Kat Karşılığı)
    /// </summary>
    /// <returns>Bahçe durumları listesi</returns>
    [HttpGet(GetGardenStatusesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Bahçe durumları listesi", Description = "Bahçe durumlarını getirir (Satılık, Kiralık, Devren Satılık, Devren Kiralık)")]

    public async Task<IActionResult> GetGardenStatuses([FromQuery] GetGardenStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        
        if (response.IsSuccessfull)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }

    
    /// <summary>
    /// Bahçe türlerini getirir (Meyve türleri: Elma, Armut, Üzüm, vb.)
    /// </summary>
    /// <returns>Bahçe türleri listesi</returns>
    [HttpGet(GetGardenTypesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Bahçe türleri listesi", Description = "Bahçe türlerini getirir")]

    public async Task<IActionResult> GetGardenTypes([FromQuery] GetGardenTypesQueryRequest request,CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (response.IsSuccessfull)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
