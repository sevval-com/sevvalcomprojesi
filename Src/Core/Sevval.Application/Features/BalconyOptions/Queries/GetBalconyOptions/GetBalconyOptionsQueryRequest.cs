using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.BalconyOptions.Queries.GetBalconyOptions
{
    public class GetBalconyOptionsQueryRequest : IRequest<ApiResponse<List<GetBalconyOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/balcony-options";

    }
}
