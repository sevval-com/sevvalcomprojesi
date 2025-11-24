using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.ContactInfo.Queries.GetContactInfo
{
    public class GetContactInfoQueryHandler : IRequestHandler<GetContactInfoQueryRequest, ApiResponse<GetContactInfoQueryResponse>>
    {
        private readonly IContactInfoService _contactInfoService;

        public GetContactInfoQueryHandler(IContactInfoService contactInfoService)
        {
            _contactInfoService = contactInfoService;
        }

        public async Task<ApiResponse<GetContactInfoQueryResponse>> Handle(GetContactInfoQueryRequest request, CancellationToken cancellationToken)
        {
            return await _contactInfoService.GetContactInfoAsync(cancellationToken);
        }
    }
}
