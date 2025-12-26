using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Contract.Queries.GetCorporateAccountContract
{

    public class GetCorporateAccountContractQueryHandler : BaseHandler, IRequestHandler<GetCorporateAccountContractQueryRequest, ApiResponse<GetCorporateAccountContractQueryResponse>>
    {
        private readonly IContractService _contractService;

        public GetCorporateAccountContractQueryHandler(IHttpContextAccessor httpContextAccessor,
          IContractService contractService) : base(httpContextAccessor)
        {

            _contractService = contractService;
        }
        public async Task<ApiResponse<GetCorporateAccountContractQueryResponse>> Handle(GetCorporateAccountContractQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _contractService.GetCorporateAccountContract(request);

            return response;
        }
    }
}
