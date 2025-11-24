using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.HeatingSystemOptions.Queries.GetHeatingSystemOptions
{
    public class GetHeatingSystemOptionsQueryRequest : IRequest<ApiResponse<List<GetHeatingSystemOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/heating-system-options";

    }
}
