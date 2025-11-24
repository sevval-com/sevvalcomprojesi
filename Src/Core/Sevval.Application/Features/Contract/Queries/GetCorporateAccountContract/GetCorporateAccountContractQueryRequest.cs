using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Contract.Queries.GetCorporateAccountContract
{
    public class GetCorporateAccountContractQueryRequest : IRequest<ApiResponse<GetCorporateAccountContractQueryResponse>>
    {
        public const string Route = "/api/v1/contracts/corporate";

    }
}
