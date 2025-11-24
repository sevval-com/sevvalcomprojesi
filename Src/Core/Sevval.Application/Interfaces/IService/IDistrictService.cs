using Sevval.Application.Features.Common;
using Sevval.Application.Features.District.Commands.CreateDistrictCommands;
using Sevval.Application.Features.District.Queries.GetAllDistricts;
using Sevval.Application.Features.District.Queries.GetDistrictById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.IService
{
    public interface IDistrictService
    {
        Task<ApiResponse<IList<GetAllDistrictsQueryResponse>>> GetAllDistricts(GetAllDistrictsQueryRequest request);
        Task<ApiResponse<GetDistrictByIdQueryResponse>> GetDistrict(GetDistrictByIdQueryRequest request);
        Task<ApiResponse<CreateDistrictCommandResponse>> CreateDistrict(CreateDistrictCommandRequest request, CancellationToken cancellationToken);
    }
}
