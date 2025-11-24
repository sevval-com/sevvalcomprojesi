using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;

public class GetLandStatusesQueryHandler : IRequestHandler<GetLandStatusesQueryRequest, ApiResponse<GetLandStatusesQueryResponse>>
{
    private readonly ILandStatusService _landStatusService;

    public GetLandStatusesQueryHandler(ILandStatusService landStatusService)
    {
        _landStatusService = landStatusService;
    }

    public async Task<ApiResponse<GetLandStatusesQueryResponse>> Handle(GetLandStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _landStatusService.GetLandStatusesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetLandStatusesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Arsa durumları getirilirken bir hata oluştu.",
                Data = new GetLandStatusesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
