using Sevval.Application.Features.Common;
using Sevval.Application.Features.Contract.Queries.GetCorporateAccountContract;
using Sevval.Application.Features.Contract.Queries.GetIndividualAccountContract;
using Sevval.Application.Interfaces.IService;
using Sevval.Infrastructure.Constants;
using System.Net;

namespace Sevval.Infrastructure.Services
{
    public class ContractService : IContractService
    {
        public async Task<ApiResponse<GetCorporateAccountContractQueryResponse>> GetCorporateAccountContract(GetCorporateAccountContractQueryRequest request)
        {
            return new ApiResponse<GetCorporateAccountContractQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = new GetCorporateAccountContractQueryResponse() { Content = Contrats.CorporateContrat },
                IsSuccessfull = true
            };
        }


        public async Task<ApiResponse<GetIndividualAccountContractQueryResponse>> GetIndividualAccountContract(GetIndividualAccountContractQueryRequest request)
        {
            return new ApiResponse<GetIndividualAccountContractQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = new GetIndividualAccountContractQueryResponse() { Content = Contrats.IndividualContrat },
                IsSuccessfull = true
            };
        }
    }
}
