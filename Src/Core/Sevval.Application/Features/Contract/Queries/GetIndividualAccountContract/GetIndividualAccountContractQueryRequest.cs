using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Contract.Queries.GetIndividualAccountContract
{
    public class GetIndividualAccountContractQueryRequest : IRequest<ApiResponse<GetIndividualAccountContractQueryResponse>>
    {
        public const string Route = "/api/v1/contracts/individual";

    }
}
