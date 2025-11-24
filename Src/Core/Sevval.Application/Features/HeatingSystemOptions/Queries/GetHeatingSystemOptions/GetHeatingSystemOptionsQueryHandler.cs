using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.HeatingSystemOptions.Queries.GetHeatingSystemOptions
{
    public class GetHeatingSystemOptionsQueryHandler : IRequestHandler<GetHeatingSystemOptionsQueryRequest, ApiResponse<List<GetHeatingSystemOptionsQueryResponse>>>
    {
        private readonly IHeatingSystemOptionsService _heatingSystemOptionsService;

        public GetHeatingSystemOptionsQueryHandler(IHeatingSystemOptionsService heatingSystemOptionsService)
        {
            _heatingSystemOptionsService = heatingSystemOptionsService;
        }

        public async Task<ApiResponse<List<GetHeatingSystemOptionsQueryResponse>>> Handle(GetHeatingSystemOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var heatingSystemOptions = await _heatingSystemOptionsService.GetHeatingSystemOptionsAsync(request, cancellationToken);

            return heatingSystemOptions;

        }
    }
}
