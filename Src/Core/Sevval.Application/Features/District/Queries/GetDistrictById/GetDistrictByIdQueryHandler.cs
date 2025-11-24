using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.District.Queries.GetDistrictById
{
    public class GetDistrictByIdQueryHandler : BaseHandler, IRequestHandler<GetDistrictByIdQueryRequest, ApiResponse<GetDistrictByIdQueryResponse>>
    {
        private readonly IDistrictService _districtService;

        public GetDistrictByIdQueryHandler(IHttpContextAccessor httpContextAccessor, IDistrictService districtService) : base(httpContextAccessor)
        {
            _districtService = districtService;
        }

        public async Task<ApiResponse<GetDistrictByIdQueryResponse>> Handle(GetDistrictByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _districtService.GetDistrict(request);

            return response;
        }
    }
}
