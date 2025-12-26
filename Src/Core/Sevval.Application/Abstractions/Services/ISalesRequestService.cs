using Sevval.Application.Features.Common;
using Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;

namespace Sevval.Application.Abstractions.Services;

public interface ISalesRequestService
{
    Task<ApiResponse<CreateSalesRequestCommandResponse>> CreateSalesRequestAsync(CreateSalesRequestCommandRequest request, CancellationToken cancellationToken);
}
