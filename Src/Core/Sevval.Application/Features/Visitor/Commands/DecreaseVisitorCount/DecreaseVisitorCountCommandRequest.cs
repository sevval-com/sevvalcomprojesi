using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount
{
    public class DecreaseVisitorCountCommandRequest : IRequest<ApiResponse<DecreaseVisitorCountCommandResponse>>
    {
        public const string Route = "/api/v1/visitors/decrease";
        
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
