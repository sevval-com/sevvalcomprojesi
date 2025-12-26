using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {



        /// <summary>
        /// Returns the current user email from the request context.
        /// </summary>
        /// <returns>The email of the current user.</returns>
        protected string GetCurrentUserEmail()
        {
            return User?.FindFirst("email")?.Value ?? string.Empty;

        }

        /// <summary>
        /// Returns the current user ID from the request context.
        /// </summary>
        /// <returns>The ID of the current user.</returns>
        protected string GetCurrentUserId()
        {
            return HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
