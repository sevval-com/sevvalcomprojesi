using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.BathroomOptions.Queries.GetBathroomOptions
{
    public class GetBathroomOptionsQueryHandler : IRequestHandler<GetBathroomOptionsQueryRequest, ApiResponse<List<GetBathroomOptionsQueryResponse>>>
    {
        private readonly IBathroomOptionsService _bathroomOptionsService;

        public GetBathroomOptionsQueryHandler(IBathroomOptionsService bathroomOptionsService)
        {
            _bathroomOptionsService = bathroomOptionsService;
        }

        public async Task<ApiResponse<List<GetBathroomOptionsQueryResponse>>> Handle(GetBathroomOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var bathroomOptions = await _bathroomOptionsService.GetBathroomOptionsAsync(request,cancellationToken);

            return bathroomOptions;
        }
    }
}
