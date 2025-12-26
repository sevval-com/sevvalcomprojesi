using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany
{
    public class GetAnnouncementsByCompanyQueryHandler : IRequestHandler<GetAnnouncementsByCompanyQueryRequest, ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetAnnouncementsByCompanyQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>> Handle(GetAnnouncementsByCompanyQueryRequest request, CancellationToken cancellationToken)
        {
            
            return await _announcementService.GetAnnouncementsByCompanyAsync(request, cancellationToken);
            
        }
    }
}
