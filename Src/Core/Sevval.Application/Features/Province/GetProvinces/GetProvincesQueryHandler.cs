using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Province.GetProvinces
{

    public class GetProvincesQueryHandler : BaseHandler, IRequestHandler<GetProvincesQueryRequest, ApiResponse<IList<GetProvincesQueryResponse>>>
    {
        private readonly IProvinceService _provinceService;

        public GetProvincesQueryHandler(IHttpContextAccessor httpContextAccessor,
           IProvinceService provinceService) : base(httpContextAccessor)
        {
            _provinceService = provinceService;
        }

        public async Task<ApiResponse<IList<GetProvincesQueryResponse>>> Handle(GetProvincesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _provinceService.GetProvinces(request);

            return response;
        }
    }



}
