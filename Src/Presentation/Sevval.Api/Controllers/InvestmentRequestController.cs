using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvestmentRequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni yatırım talebi oluşturur
    /// </summary>
    /// <param name="request">Yatırım talebi bilgileri</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    /// <returns>Oluşturulan yatırım talebi bilgileri</returns>
    [HttpPost(CreateInvestmentRequestCommandRequest.Route)]
    [SwaggerOperation(Summary = "Yeni yatırım talebi oluşturur", Description = "Yeni yatırım talebi oluşturur. MinBudget ve MaxBudget alanları ile yatırım bütçe aralığı belirtilir.")]
    public async Task<ActionResult<ApiResponse<CreateInvestmentRequestCommandResponse>>> CreateInvestmentRequest(
        [FromBody] CreateInvestmentRequestCommandRequest request,
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
