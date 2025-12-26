using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.BusinessStatus.Queries.GetBusinessStatuses
{
    public class GetBusinessStatusesQueryHandler : IRequestHandler<GetBusinessStatusesQueryRequest, ApiResponse<List<GetBusinessStatusesQueryResponse>>>
    {
        private readonly IBusinessStatusService _businessStatusService;

        public GetBusinessStatusesQueryHandler(IBusinessStatusService businessStatusService)
        {
            _businessStatusService = businessStatusService;
        }

        public async Task<ApiResponse<List<GetBusinessStatusesQueryResponse>>> Handle(GetBusinessStatusesQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _businessStatusService.GetBusinessStatusesAsync(request, cancellationToken);
            return result;
        }
    }
}
