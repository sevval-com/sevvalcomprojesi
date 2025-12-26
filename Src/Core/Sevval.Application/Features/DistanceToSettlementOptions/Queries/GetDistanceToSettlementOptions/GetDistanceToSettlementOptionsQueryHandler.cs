using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.DistanceToSettlementOptions.Queries.GetDistanceToSettlementOptions
{
    public class GetDistanceToSettlementOptionsQueryHandler : IRequestHandler<GetDistanceToSettlementOptionsQueryRequest, ApiResponse<List<GetDistanceToSettlementOptionsQueryResponse>>>
    {
        private readonly IDistanceToSettlementOptionsService _distanceToSettlementOptionsService;

        public GetDistanceToSettlementOptionsQueryHandler(IDistanceToSettlementOptionsService distanceToSettlementOptionsService)
        {
            _distanceToSettlementOptionsService = distanceToSettlementOptionsService;
        }

        public async Task<ApiResponse<List<GetDistanceToSettlementOptionsQueryResponse>>> Handle(GetDistanceToSettlementOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var distanceToSettlementOptions = await _distanceToSettlementOptionsService.GetDistanceToSettlementOptionsAsync(request,cancellationToken);

            return distanceToSettlementOptions;
        }
    }
}
