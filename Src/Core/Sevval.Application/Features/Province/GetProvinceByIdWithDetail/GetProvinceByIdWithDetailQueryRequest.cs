using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Province.GetProvinceByIdWithDetail
{
    public class GetProvinceByIdWithDetailQueryRequest:IRequest<ApiResponse<GetProvinceByIdWithDetailQueryResponse>>
    {
        public const string Route = "/api/v1/provinces/{name}/detail";

        public string Name { get; set; }
    }
}
