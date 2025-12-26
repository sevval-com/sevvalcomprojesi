using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount
{
    public class DecreaseVisitorCountCommandHandler : IRequestHandler<DecreaseVisitorCountCommandRequest, ApiResponse<DecreaseVisitorCountCommandResponse>>
    {
        private readonly IVisitorService _visitorService;

        public DecreaseVisitorCountCommandHandler(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        public async Task<ApiResponse<DecreaseVisitorCountCommandResponse>> Handle(DecreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
        {
            return await _visitorService.DecreaseVisitorCountAsync(request, cancellationToken);
        }
    }
}
