using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;

public class GetGardenStatusesQueryHandler : IRequestHandler<GetGardenStatusesQueryRequest, ApiResponse<GetGardenStatusesQueryResponse>>
{
    private readonly IGardenStatusService _gardenStatusService;

    public GetGardenStatusesQueryHandler(IGardenStatusService gardenStatusService)
    {
        _gardenStatusService = gardenStatusService;
    }

    public async Task<ApiResponse<GetGardenStatusesQueryResponse>> Handle(GetGardenStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _gardenStatusService.GetGardenStatusesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetGardenStatusesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Bahçe durumları getirilirken bir hata oluştu.",
                Data = new GetGardenStatusesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
