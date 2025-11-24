using Sevval.Application.Features.Common;
using Sevval.Application.Features.Province.GetProvinceByIdWithDetail;
using Sevval.Application.Features.Province.GetProvinces;
using Sevval.Application.Features.Province.GetProvincesWithDetail;

namespace Sevval.Application.Interfaces.IService
{
    public interface IProvinceService
    {
        Task<ApiResponse<GetProvinceByIdWithDetailQueryResponse>> GetProvinceByNameWithDetail(GetProvinceByIdWithDetailQueryRequest request);
        Task<ApiResponse<IList<GetProvincesQueryResponse>>> GetProvinces(GetProvincesQueryRequest request);
        Task<ApiResponse<IList<GetProvincesWithDetailQueryResponse>>> GetProvincesWithDetail(GetProvincesWithDetailQueryRequest request);
    }
}
