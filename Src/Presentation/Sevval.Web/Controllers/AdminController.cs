using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class AdminController : Controller
{
    [HttpGet("messaging-demo-modal")]
    public IActionResult MessagingDemoModal()
    {
        return View();
    }

    [HttpGet("messaging")]
    public IActionResult Messaging()
    {
        var query = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty;
        return Redirect($"/messaging{query}");
    }
}
