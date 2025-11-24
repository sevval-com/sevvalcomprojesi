using MediatR;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;

public class CreateSalesRequestCommandHandler : IRequestHandler<CreateSalesRequestCommandRequest, ApiResponse<CreateSalesRequestCommandResponse>>
{
    private readonly ISalesRequestService _salesRequestService;

    public CreateSalesRequestCommandHandler(ISalesRequestService salesRequestService)
    {
        _salesRequestService = salesRequestService;
    }

    public async Task<ApiResponse<CreateSalesRequestCommandResponse>> Handle(CreateSalesRequestCommandRequest request, CancellationToken cancellationToken)
    {

        var response = await _salesRequestService.CreateSalesRequestAsync(request, cancellationToken);
        return response;

    }
}
