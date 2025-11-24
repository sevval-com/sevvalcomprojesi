using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Auth.Queries.Auth;
using Sevval.Application.Features.User.Commands.RefreshToken;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(AuthQueryRequest.Route)]
        [SwaggerOperation(Summary = "Oturum açma işlemini gerçekleştirir.")]
        public async Task<IActionResult> Login([FromBody]AuthQueryRequest request)
        {
            var response = await _mediator.Send(request, CancellationToken.None);

            return Ok(response);
        }

        [HttpPost(RefreshTokenCommandRequest.Route)]
        [SwaggerOperation(Summary = "Oturum açma işlemini gerçekleştirir.")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommandRequest request)
        {
            var response = await _mediator.Send(request, CancellationToken.None);

            return Ok(response);
        }

        
    }
}
