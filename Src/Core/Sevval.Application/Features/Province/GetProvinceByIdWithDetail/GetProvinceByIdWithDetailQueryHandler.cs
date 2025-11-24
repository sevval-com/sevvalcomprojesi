using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Province.GetProvinceByIdWithDetail
{

    public class GetProvinceByIdWithDetailQueryHandler : BaseHandler, IRequestHandler<GetProvinceByIdWithDetailQueryRequest, ApiResponse<GetProvinceByIdWithDetailQueryResponse>>
    {
        private readonly IProvinceService _provinceService;

        public GetProvinceByIdWithDetailQueryHandler(IHttpContextAccessor httpContextAccessor, 
           IProvinceService provinceService) : base(httpContextAccessor)
        {
            _provinceService = provinceService;
        }

        public async Task<ApiResponse<GetProvinceByIdWithDetailQueryResponse>> Handle(GetProvinceByIdWithDetailQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _provinceService.GetProvinceByNameWithDetail(request);

            return response;
        }
    }
}
