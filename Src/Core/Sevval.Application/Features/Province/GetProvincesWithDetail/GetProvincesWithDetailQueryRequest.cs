using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Province.GetProvincesWithDetail;

public class GetProvincesWithDetailQueryRequest : IRequest<ApiResponse<IList<GetProvincesWithDetailQueryResponse>>>
{
    public const string Route = "/api/v1/provinces/detail";

}
