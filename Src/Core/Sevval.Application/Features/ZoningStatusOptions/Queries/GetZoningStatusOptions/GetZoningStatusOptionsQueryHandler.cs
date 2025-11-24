using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.ZoningStatusOptions.Queries.GetZoningStatusOptions
{
    public class GetZoningStatusOptionsQueryHandler : IRequestHandler<GetZoningStatusOptionsQueryRequest, ApiResponse<List<GetZoningStatusOptionsQueryResponse>>>
    {
        private readonly IZoningStatusOptionsService _zoningStatusOptionsService;

        public GetZoningStatusOptionsQueryHandler(IZoningStatusOptionsService zoningStatusOptionsService)
        {
            _zoningStatusOptionsService = zoningStatusOptionsService;
        }

        public async Task<ApiResponse<List<GetZoningStatusOptionsQueryResponse>>> Handle(GetZoningStatusOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var zoningStatusOptions = await _zoningStatusOptionsService.GetZoningStatusOptionsAsync(request,cancellationToken);

            return zoningStatusOptions;

         }
    }
}
