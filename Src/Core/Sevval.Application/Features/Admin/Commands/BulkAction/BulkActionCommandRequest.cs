using MediatR;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using System.Collections.Generic;

namespace Sevval.Application.Features.Admin.Commands.BulkAction;

public class BulkActionCommandRequest : IRequest<ApiResponse<BulkActionCommandResponse>>
{
    public List<string> UserIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // "approve" or "delete"
}
