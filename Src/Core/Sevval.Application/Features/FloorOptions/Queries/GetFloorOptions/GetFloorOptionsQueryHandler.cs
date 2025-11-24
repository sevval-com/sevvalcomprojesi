using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.FloorOptions.Queries.GetFloorOptions
{
    public class GetFloorOptionsQueryHandler : IRequestHandler<GetFloorOptionsQueryRequest, ApiResponse<List<GetFloorOptionsQueryResponse>>>
    {
        private readonly IFloorOptionsService _floorOptionsService;

        public GetFloorOptionsQueryHandler(IFloorOptionsService floorOptionsService)
        {
            _floorOptionsService = floorOptionsService;
        }

        public async Task<ApiResponse<List<GetFloorOptionsQueryResponse>>> Handle(GetFloorOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var floorOptions = await _floorOptionsService.GetFloorOptionsAsync(request,cancellationToken);

            return floorOptions;

         }
    }
}
