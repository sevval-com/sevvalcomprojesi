using Sevval.Application.Features.Common;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount;
using Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount;
using Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount;

namespace Sevval.Application.Interfaces.IService
{
    public interface IVisitorService
    {
        public Task<ApiResponse<GetActiveVisitorCountQueryResponse>> GetActiveVisitorCountAsync(GetActiveVisitorCountQueryRequest request);
        public Task<ApiResponse<GetTotalVisitorCountQueryResponse>> GetTotalVisitorCountAsync(GetTotalVisitorCountQueryRequest request);
        public Task<ApiResponse<IncreaseVisitorCountCommandResponse>> IncreaseVisitorCountAsync(IncreaseVisitorCountCommandRequest request, CancellationToken cancellationToken);
        public Task<ApiResponse<DecreaseVisitorCountCommandResponse>> DecreaseVisitorCountAsync(DecreaseVisitorCountCommandRequest request, CancellationToken cancellationToken);
    }
}
