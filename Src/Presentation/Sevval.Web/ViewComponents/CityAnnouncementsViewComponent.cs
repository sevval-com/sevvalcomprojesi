using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince;
using sevvalemlak.csproj.ClientServices.AnnouncementService;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "CityAnnouncements")]

public class CityAnnouncementsViewComponent : ViewComponent
{
    private readonly IAnnouncementClientService _announcementClientService;
    public CityAnnouncementsViewComponent(IAnnouncementClientService announcementClientService)
    {
        _announcementClientService = announcementClientService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var response = await _announcementClientService.GetAnnouncementCountByProvince(new GetAnnouncementCountByProvinceQueryRequest(), CancellationToken.None);

        return View(response?.Data);
    }
}
