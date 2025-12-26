using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;
using Sevval.Application.Interfaces.IService.Common;
using sevvalemlak.csproj.ClientServices.SuitableAnnouncements;

namespace Sevval.Web.ViewComponents;

[ViewComponent(Name = "SuitableAnnouncements")]
public class SuitableAnnouncementsViewComponent : ViewComponent
{
    private readonly ISuitableAnnouncementsClientService _suitableAnnouncementsService;
    private readonly ICurrentUserService _currentUserService;

    public SuitableAnnouncementsViewComponent(
        ISuitableAnnouncementsClientService suitableAnnouncementsService, 
        ICurrentUserService currentUserService)
    {
        _suitableAnnouncementsService = suitableAnnouncementsService;
        _currentUserService = currentUserService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var response = await _suitableAnnouncementsService
             .GetSuitableAnnouncementsAsync(new GetSuitableAnnouncementsQueryRequest()
             {
                 Page = 1,
                 Size = 4,
                 UserId = _currentUserService.UserId

             }, CancellationToken.None);

        return View(response?.Data);
    }
}
