using System.Collections.Generic;

namespace Sevval.Application.Features.Admin.Commands.BulkAction;

public class BulkActionCommandResponse
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkActionResult> Results { get; set; } = new();
}

public class BulkActionResult
{
    public string UserId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
