using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Contract.Queries.GetCorporateAccountContract;
using Sevval.Application.Features.Contract.Queries.GetIndividualAccountContract;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Interfaces.IService;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContractController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet(GetIndividualAccountContractQueryRequest.Route)]
        [SwaggerOperation(Summary = "Bireysel hesap kontratını getirir", Description = "Bu endpoint, bireysel hesap kontratını döner.")]
        public async Task<IActionResult> GetIndividualAccountContract([FromQuery]GetIndividualAccountContractQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet(GetCorporateAccountContractQueryRequest.Route)]
        [SwaggerOperation(Summary = "Kurumsal hesap kontratını getirir", Description = "Bu endpoint, kurumsal hesap kontratını döner.")]
        public async Task<IActionResult> GetCorporateAccountContract([FromQuery] GetCorporateAccountContractQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}
