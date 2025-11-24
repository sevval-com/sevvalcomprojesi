using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.BusinessType.Queries.GetBusinessTypes
{
    public class GetBusinessTypesQueryHandler : IRequestHandler<GetBusinessTypesQueryRequest, ApiResponse<List<GetBusinessTypesQueryResponse>>>
    {
        private readonly IBusinessTypeService _businessTypeService;

        public GetBusinessTypesQueryHandler(IBusinessTypeService businessTypeService)
        {
            _businessTypeService = businessTypeService;
        }

        public async Task<ApiResponse<List<GetBusinessTypesQueryResponse>>> Handle(GetBusinessTypesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _businessTypeService.GetBusinessTypesAsync(cancellationToken);
        }
    }
}
