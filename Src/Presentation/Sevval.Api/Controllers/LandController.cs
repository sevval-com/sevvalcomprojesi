using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;
using Sevval.Application.Features.LandType.Queries.GetLandTypes;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LandController : ControllerBase
{
    private readonly IMediator _mediator;

    public LandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Arsa durumlarını getirir (Satılık, Kiralık, Kat Karşılığı)
    /// </summary>
    /// <returns>Arsa durumları listesi</returns>
    [HttpGet(GetLandStatusesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Arsa durumları listesi", Description = "Arsa durumlarını getirir (Satılık, Kiralık, Devren Satılık, Devren Kiralık)")]

    public async Task<IActionResult> GetLandStatuses([FromQuery] GetLandStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        
        if (response.IsSuccessfull)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }

    /// <summary>
    /// Arsa tiplerini getirir (A-Lejantlı Arsa, Arsa, Çiftlik, vb.)
    /// </summary>
    /// <returns>Arsa tipleri listesi</returns>
    [HttpGet(GetLandTypesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Arsa türleri listesi", Description = "Arsa türlerini getirir")]

    public async Task<IActionResult> GetLandTypes([FromQuery] GetLandTypesQueryRequest request, CancellationToken cancellationToken)
    {

        var response = await _mediator.Send(request, cancellationToken);


        return Ok(response);


    }
}
