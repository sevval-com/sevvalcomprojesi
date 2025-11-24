using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince;
using sevvalemlak.csproj.ClientServices.AnnouncementService;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "WorkingCities")]
public class WorkingCitiesViewComponent : ViewComponent
{
    private readonly IAnnouncementClientService _announcementClientService;

    public WorkingCitiesViewComponent(IAnnouncementClientService announcementClientService)
    {
        _announcementClientService = announcementClientService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string userId)
    {
        try
        {
            var request = new GetCompanyAnnouncementCountByProvinceQueryRequest { UserId = userId, Status = "active" };
            var provinceCounts = await _announcementClientService.GetCompanyAnnouncementCountByProvince(request, CancellationToken.None);

            return View(provinceCounts.Data??new List<GetCompanyAnnouncementCountByProvinceQueryResponse>());
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            System.Diagnostics.Debug.WriteLine($"WorkingCitiesViewComponent Error: {ex.Message}");
            // Return empty list on error
            return View(new List<GetCompanyAnnouncementCountByProvinceQueryResponse>());
        }
    }
}
