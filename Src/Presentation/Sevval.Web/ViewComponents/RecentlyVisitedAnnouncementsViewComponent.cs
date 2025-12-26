using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement;
using Sevval.Application.Interfaces.IService.Common;
using sevvalemlak.csproj.ClientServices.RecentlyVisitedAnnouncement;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "RecentlyVisitedAnnouncements")]

public class RecentlyVisitedAnnouncementsViewComponent : ViewComponent
{

    private readonly IRecentlyVisitedAnnouncementClientService _recentlyVisitedAnnouncementService;
    private readonly ICurrentUserService _currentUserService;

    public RecentlyVisitedAnnouncementsViewComponent(IRecentlyVisitedAnnouncementClientService recentlyVisitedAnnouncementService, ICurrentUserService currentUserService)
    {
        _recentlyVisitedAnnouncementService = recentlyVisitedAnnouncementService;
        _currentUserService = currentUserService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var response = await _recentlyVisitedAnnouncementService
             .GetRecentlyVisitedAnnouncementAsync(new GetRecentlyVisitedAnnouncementQueryRequest()
             {
                 Page = 1,
                 Size = 4,
                 UserId = _currentUserService.UserId

             }, CancellationToken.None);

        return View(response?.Data);
    }
}
