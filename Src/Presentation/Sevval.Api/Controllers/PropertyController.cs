
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;
using Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Konut durumlarını getirir (Satılık, Kiralık)
    /// </summary>
    /// <returns>Konut durumları listesi</returns>
    [HttpGet(GetPropertyStatusesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Konut durumları listesi", Description = "Konut durumlarını getirir (Satılık, Kiralık)")]

    public async Task<IActionResult> GetPropertyStatuses([FromQuery] GetPropertyStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);



    }

    /// <summary>
    /// Mülk tiplerini getirir (Daire, Villa, Müstakil Ev vb.)
    /// </summary>
    /// <returns>Mülk tipleri listesi</returns>
    [HttpGet(GetPropertyTypesQueryRequest.Route)]
    [SwaggerOperation(Summary = "Konut tipleri listesi", Description = "Konut/Mülk tiplerini getirir (Daire, Villa, Müstakil Ev, vb.)")]
    public async Task<IActionResult> GetPropertyTypes([FromQuery] GetPropertyTypesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (response.IsSuccessfull)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
