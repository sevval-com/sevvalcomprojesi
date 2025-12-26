using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using sevvalemlak.csproj.ClientServices.AnnouncementService;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "CategoryCards")]
public class CategoryCardsViewComponent : ViewComponent
{
    private readonly IAnnouncementClientService _announcementClientService;
    public CategoryCardsViewComponent(IAnnouncementClientService announcementClientService)
    {
        _announcementClientService = announcementClientService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var response = await _announcementClientService.GetAnnouncementCountByType(new GetAnnouncementCountByTypeQueryRequest(), CancellationToken.None);

        return View(response?.Data);
    }
}
