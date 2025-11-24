using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Contract.Queries.GetIndividualAccountContract
{


    public class GetIndividualAccountContractQueryHandler : BaseHandler, IRequestHandler<GetIndividualAccountContractQueryRequest, ApiResponse<GetIndividualAccountContractQueryResponse>>
    {
        private readonly IContractService _contractService;

        public GetIndividualAccountContractQueryHandler(IHttpContextAccessor httpContextAccessor,
          IContractService contractService) : base(httpContextAccessor)
        {

            _contractService = contractService;
        }
        public async Task<ApiResponse<GetIndividualAccountContractQueryResponse>> Handle(GetIndividualAccountContractQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _contractService.GetIndividualAccountContract(request);

            return response;
        }
    }
}
