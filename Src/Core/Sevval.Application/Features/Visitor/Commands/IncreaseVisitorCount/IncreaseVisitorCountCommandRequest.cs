using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount
{
    public class IncreaseVisitorCountCommandRequest : IRequest<ApiResponse<IncreaseVisitorCountCommandResponse>>
    {
        public const string Route = "/api/v1/visitors/increase";
        
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
