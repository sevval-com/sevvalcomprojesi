using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount
{
    public class IncreaseVisitorCountCommandHandler : IRequestHandler<IncreaseVisitorCountCommandRequest, ApiResponse<IncreaseVisitorCountCommandResponse>>
    {
        private readonly IVisitorService _visitorService;

        public IncreaseVisitorCountCommandHandler(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        public async Task<ApiResponse<IncreaseVisitorCountCommandResponse>> Handle(IncreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
        {
            return await _visitorService.IncreaseVisitorCountAsync(request, cancellationToken);
        }
    }
}
