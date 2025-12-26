using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount
{
    public class GetActiveVisitorCountQueryHandler : BaseHandler, IRequestHandler<GetActiveVisitorCountQueryRequest, ApiResponse<GetActiveVisitorCountQueryResponse>>
    {
        private readonly IVisitorService _visitorService;

        public GetActiveVisitorCountQueryHandler(IHttpContextAccessor httpContextAccessor, IVisitorService visitorService) : base(httpContextAccessor)
        {
            _visitorService = visitorService;
        }

        public async Task<ApiResponse<GetActiveVisitorCountQueryResponse>> Handle(GetActiveVisitorCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _visitorService.GetActiveVisitorCountAsync(request);

            return response;
        }
    }
}
