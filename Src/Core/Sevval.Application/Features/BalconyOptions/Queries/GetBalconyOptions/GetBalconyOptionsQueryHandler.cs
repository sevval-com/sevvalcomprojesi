using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.BalconyOptions.Queries.GetBalconyOptions
{
    public class GetBalconyOptionsQueryHandler : IRequestHandler<GetBalconyOptionsQueryRequest, ApiResponse<List<GetBalconyOptionsQueryResponse>>>
    {
        private readonly IBalconyOptionsService _balconyOptionsService;

        public GetBalconyOptionsQueryHandler(IBalconyOptionsService balconyOptionsService)
        {
            _balconyOptionsService = balconyOptionsService;
        }

        public async Task<ApiResponse<List<GetBalconyOptionsQueryResponse>>> Handle(GetBalconyOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var balconyOptions = await _balconyOptionsService.GetBalconyOptionsAsync(request,cancellationToken);

            return balconyOptions;
        }
    }
}
