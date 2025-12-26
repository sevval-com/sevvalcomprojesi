using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.District.Queries.GetAllDistricts
{
    public class GetAllDistrictsQueryRequest : IRequest<ApiResponse<IList<GetAllDistrictsQueryResponse>>>
    {
        public const string Route = "/api/v1/districts";
        //public string CacheKey => "GetAllDistricts";
        //public double CacheTime => 5; //5dk

        public string ProvinceName { get; set; }
    }
}
