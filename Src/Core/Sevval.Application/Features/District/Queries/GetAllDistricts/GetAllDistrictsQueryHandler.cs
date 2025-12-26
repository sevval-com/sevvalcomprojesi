using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.District.Queries.GetAllDistricts;

public class GetAllDistrictsQueryHandler : BaseHandler, IRequestHandler<GetAllDistrictsQueryRequest, ApiResponse<IList<GetAllDistrictsQueryResponse>>>
{
    private readonly IDistrictService _districtService;

    public GetAllDistrictsQueryHandler(IHttpContextAccessor httpContextAccessor, IDistrictService districtService) : base(httpContextAccessor)
    {
        _districtService = districtService;
    }

    public async Task<ApiResponse<IList<GetAllDistrictsQueryResponse>>> Handle(GetAllDistrictsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _districtService.GetAllDistricts(request);

        return response;
    }
}
