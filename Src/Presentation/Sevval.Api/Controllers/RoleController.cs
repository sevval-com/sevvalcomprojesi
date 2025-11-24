using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Role.Queries.GetUserRoles;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet(GetUserRolesQueryRequest.Route)]
        public async Task<IActionResult> GetUserRoles([FromQuery] GetUserRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
