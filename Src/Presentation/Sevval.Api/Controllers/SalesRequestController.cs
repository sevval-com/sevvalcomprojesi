using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesRequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni satış talebi oluşturur
    /// </summary>
    /// <param name="request">Satış talebi bilgileri</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    /// <returns>Oluşturulan satış talebi bilgileri</returns>
    [HttpPost(CreateSalesRequestCommandRequest.Route)]
    [SwaggerOperation(Summary = "Yeni satış talebi oluşturur", Description = "Yeni satış talebi oluşturur. Satış talebi bilgileri ile kullanılır.")]
    public async Task<ActionResult<ApiResponse<CreateSalesRequestCommandResponse>>> CreateSalesRequest(
        [FromBody] CreateSalesRequestCommandRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (response.IsSuccessfull)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
