using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.District.Queries.GetAllDistricts;
using Sevval.Application.Features.Neighbourhood.Queries.GetNeighbourhoods;
using Sevval.Application.Features.Province.GetProvinces;
using Sevval.Application.Features.Province.GetProvincesWithDetail;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(GetProvincesWithDetailQueryRequest.Route)]
        public async Task<IActionResult> GetProvincesWithDetail([FromQuery]GetProvincesWithDetailQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet(GetProvincesQueryRequest.Route)]
        public async Task<IActionResult> GetProvinces([FromQuery] GetProvincesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetAllDistrictsQueryRequest.Route)]
        public async Task<IActionResult> GetAllDistricts([FromQuery] GetAllDistrictsQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetNeighbourhoodsQueryRequest.Route)]
        public async Task<IActionResult> GetNeighbourhoods([FromQuery] GetNeighbourhoodsQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
