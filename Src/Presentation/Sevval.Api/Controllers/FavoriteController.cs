using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Favorite.Commands.AddFavorite;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FavoriteController : BaseController
    {
        private readonly IMediator _mediator;

        public FavoriteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// İlanı favorilere ekler
        /// </summary>
        /// <param name="request">Favori ekleme isteği</param>
        /// <returns>Favori ekleme sonucu</returns>
        
        [Authorize]
        [HttpPost(AddFavoriteCommandRequest.Route)]
        public async Task<IActionResult> AddFavorite([FromBody]AddFavoriteCommandRequest request)
        {
            // IP adresini otomatik olarak al
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            request.UserEmail= GetCurrentUserEmail();


            var result = await _mediator.Send(request);
            
            if (result.IsSuccessfull)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }

        
    }
}
