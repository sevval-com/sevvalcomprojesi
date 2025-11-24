using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.RoadConditionOptions.Queries.GetRoadConditionOptions
{
    public class GetRoadConditionOptionsQueryHandler : IRequestHandler<GetRoadConditionOptionsQueryRequest, ApiResponse<List<GetRoadConditionOptionsQueryResponse>>>
    {
        private readonly IRoadConditionOptionsService _roadConditionOptionsService;

        public GetRoadConditionOptionsQueryHandler(IRoadConditionOptionsService roadConditionOptionsService)
        {
            _roadConditionOptionsService = roadConditionOptionsService;
        }

        public async Task<ApiResponse<List<GetRoadConditionOptionsQueryResponse>>> Handle(GetRoadConditionOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var roadConditionOptions = await _roadConditionOptionsService.GetRoadConditionOptionsAsync(request, cancellationToken);

            return roadConditionOptions;
        }
    }
}
