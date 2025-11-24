using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Province.GetProvincesWithDetail;



public class GetProvincesWithDetailQueryHandler : BaseHandler, IRequestHandler<GetProvincesWithDetailQueryRequest, ApiResponse<IList<GetProvincesWithDetailQueryResponse>>>
{
    private readonly IProvinceService _provinceService;

    public GetProvincesWithDetailQueryHandler(IHttpContextAccessor httpContextAccessor,
       IProvinceService provinceService) : base(httpContextAccessor)
    {
        _provinceService = provinceService;
    }

    public async Task<ApiResponse<IList<GetProvincesWithDetailQueryResponse>>> Handle(GetProvincesWithDetailQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _provinceService.GetProvincesWithDetail(request);

        return response;
    }
}
