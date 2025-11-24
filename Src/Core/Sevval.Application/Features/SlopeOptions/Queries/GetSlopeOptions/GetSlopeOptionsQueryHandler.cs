using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.SlopeOptions.Queries.GetSlopeOptions
{
    public class GetSlopeOptionsQueryHandler : IRequestHandler<GetSlopeOptionsQueryRequest, ApiResponse<List<GetSlopeOptionsQueryResponse>>>
    {
        private readonly ISlopeOptionsService _slopeOptionsService;

        public GetSlopeOptionsQueryHandler(ISlopeOptionsService slopeOptionsService)
        {
            _slopeOptionsService = slopeOptionsService;
        }

        public async Task<ApiResponse<List<GetSlopeOptionsQueryResponse>>> Handle(GetSlopeOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var slopeOptions = await _slopeOptionsService.GetSlopeOptionsAsync(request,cancellationToken);

            return slopeOptions;

        }
    }
}
