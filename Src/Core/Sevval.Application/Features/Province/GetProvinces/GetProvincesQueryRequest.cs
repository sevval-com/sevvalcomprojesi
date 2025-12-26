using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Province.GetProvinces;

public class GetProvincesQueryRequest : IRequest<ApiResponse<IList<GetProvincesQueryResponse>>>
{
    public const string Route = "/api/v1/provinces";

}
