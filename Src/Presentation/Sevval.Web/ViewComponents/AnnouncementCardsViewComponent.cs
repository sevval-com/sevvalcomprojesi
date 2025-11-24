using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using sevvalemlak.csproj.ClientServices.AnnouncementService;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "AnnouncementCards")]

public class AnnouncementCardsViewComponent : ViewComponent
{

    private readonly IAnnouncementClientService _announcementClientService;
    public AnnouncementCardsViewComponent(IAnnouncementClientService announcementClientService)
    {
        _announcementClientService = announcementClientService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var response = await _announcementClientService.GetAnnouncementCountByType(
            new GetAnnouncementCountByTypeQueryRequest(), CancellationToken.None);

        return View(response?.Data);
    }
}
