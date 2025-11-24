using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;

public class GetRoomOptionsQueryHandler : IRequestHandler<GetRoomOptionsQueryRequest, ApiResponse<GetRoomOptionsQueryResponse>>
{
    private readonly IRoomOptionsService _roomOptionsService;

    public GetRoomOptionsQueryHandler(IRoomOptionsService roomOptionsService)
    {
        _roomOptionsService = roomOptionsService;
    }

    public async Task<ApiResponse<GetRoomOptionsQueryResponse>> Handle(GetRoomOptionsQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _roomOptionsService.GetRoomOptionsAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetRoomOptionsQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Oda seçenekleri getirilirken bir hata oluştu.",
                Data = new GetRoomOptionsQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
