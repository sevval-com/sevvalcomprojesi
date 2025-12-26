using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "AboutUs")]
public class AboutUsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GetAboutUsQueryResponse model)
    {
        return View(model ?? new GetAboutUsQueryResponse());
    }
}
