using Sevval.Application.Features.Common;
using Sevval.Application.Features.Contract.Queries.GetCorporateAccountContract;
using Sevval.Application.Features.Contract.Queries.GetIndividualAccountContract;

namespace Sevval.Application.Interfaces.IService
{
    public interface IContractService
    {
        Task<ApiResponse<GetCorporateAccountContractQueryResponse>> GetCorporateAccountContract(GetCorporateAccountContractQueryRequest request);
        Task<ApiResponse<GetIndividualAccountContractQueryResponse>> GetIndividualAccountContract(GetIndividualAccountContractQueryRequest request);
    }
}
