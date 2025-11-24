using Sevval.Application.Features.Common;
using Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount;
using Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount;

namespace sevvalemlak.csproj.ClientServices.VisitoryServices
{
    public interface IVisitoryClientService
    {
        Task<ApiResponse<DecreaseVisitorCountCommandResponse>> DecreaseVisitorCount(DecreaseVisitorCountCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<GetActiveVisitorCountQueryResponse>> GetActiveVisitorCount(GetActiveVisitorCountQueryRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<GetTotalVisitorCountQueryResponse>> GetTotalVisitorCount(GetTotalVisitorCountQueryRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<IncreaseVisitorCountCommandResponse>> IncreaseVisitorCount(IncreaseVisitorCountCommandRequest request, CancellationToken cancellationToken);
    }
}