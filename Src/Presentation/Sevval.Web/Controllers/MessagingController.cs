using Microsoft.AspNetCore.Mvc;

namespace Sevval.Web.Controllers;

[Route("messaging")]
public class MessagingController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Admin/Messaging.cshtml");
    }

    [HttpGet("/mesajlarim/detay/{token}/yeni")]
    public IActionResult MaskedDetail(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Redirect("/messaging");
        }

        try
        {
            var padded = token.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            var bytes = Convert.FromBase64String(padded);
            var decoded = System.Text.Encoding.UTF8.GetString(bytes);
            var parts = decoded.Split('|');
            if (parts.Length < 2)
            {
                return Redirect("/messaging");
            }

            ViewData["MaskedOtherId"] = parts[0];
            ViewData["MaskedType"] = parts[1];
            if (parts.Length >= 3)
            {
                ViewData["MaskedListingId"] = parts[2];
            }
        }
        catch
        {
            return Redirect("/messaging");
        }

        return View("~/Views/Admin/Messaging.cshtml");
    }
}
