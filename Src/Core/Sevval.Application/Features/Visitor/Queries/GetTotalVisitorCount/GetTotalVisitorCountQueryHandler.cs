using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount
{
    public class GetTotalVisitorCountQueryHandler : BaseHandler, IRequestHandler<GetTotalVisitorCountQueryRequest, ApiResponse<GetTotalVisitorCountQueryResponse>>
    {
        private readonly IVisitorService _visitorService;

        public GetTotalVisitorCountQueryHandler(IHttpContextAccessor httpContextAccessor, IVisitorService visitorService) : base(httpContextAccessor)
        {
            _visitorService = visitorService;
        }

        public async Task<ApiResponse<GetTotalVisitorCountQueryResponse>> Handle(GetTotalVisitorCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _visitorService.GetTotalVisitorCountAsync(request);


            return response;
        }
    }
}
