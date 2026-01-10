using Microsoft.AspNetCore.Mvc;

namespace Sevval.Web.Controllers;

[Route("messages")]
public class MessagesController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
