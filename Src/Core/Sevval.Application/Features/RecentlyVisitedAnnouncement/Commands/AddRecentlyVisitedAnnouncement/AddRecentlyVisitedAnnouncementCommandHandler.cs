using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement
{
    public class AddRecentlyVisitedAnnouncementCommandHandler : IRequestHandler<AddRecentlyVisitedAnnouncementCommandRequest, ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>>
    {
        private readonly IRecentlyVisitedAnnouncementService _recentlyVisitedAnnouncementService;

        public AddRecentlyVisitedAnnouncementCommandHandler(IRecentlyVisitedAnnouncementService recentlyVisitedAnnouncementService)
        {
            _recentlyVisitedAnnouncementService = recentlyVisitedAnnouncementService;
        }

        public async Task<ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>> Handle(AddRecentlyVisitedAnnouncementCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new AddRecentlyVisitedAnnouncementCommandValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return new ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>
                    {
                        Code = 400,
                        IsSuccessfull = false,
                        Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Data = null
                    };
                }

                var result = await _recentlyVisitedAnnouncementService.AddRecentlyVisitedAnnouncementAsync(request, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>
                {
                    Code = 500,
                    IsSuccessfull = false,
                    Message = "Son gezilen ilan eklenirken bir hata oluştu: " + ex.Message,
                    Data = null
                };
            }
        }
    }
}
