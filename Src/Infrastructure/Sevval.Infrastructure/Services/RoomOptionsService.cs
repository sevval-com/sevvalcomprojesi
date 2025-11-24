using Sevval.Application.DTOs.RoomOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class RoomOptionsService : IRoomOptionsService
{
    public async Task<ApiResponse<GetRoomOptionsQueryResponse>> GetRoomOptionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var roomOptions = new List<RoomOptionDTO>
            {
                new RoomOptionDTO { Value = "", Text = "Seçiniz" },
                new RoomOptionDTO { Value = "Stüdyo (1+0)", Text = "Stüdyo (1+0)" },
                new RoomOptionDTO { Value = "1+1", Text = "1+1" },
                new RoomOptionDTO { Value = "1.5+1", Text = "1.5+1" },
                new RoomOptionDTO { Value = "2+0", Text = "2+0" },
                new RoomOptionDTO { Value = "2+1", Text = "2+1" },
                new RoomOptionDTO { Value = "2.5+1", Text = "2.5+1" },
                new RoomOptionDTO { Value = "2+2", Text = "2+2" },
                new RoomOptionDTO { Value = "3+0", Text = "3+0" },
                new RoomOptionDTO { Value = "3+1", Text = "3+1" },
                new RoomOptionDTO { Value = "3.5+1", Text = "3.5+1" },
                new RoomOptionDTO { Value = "3+2", Text = "3+2" },
                new RoomOptionDTO { Value = "3+3", Text = "3+3" },
                new RoomOptionDTO { Value = "4+0", Text = "4+0" },
                new RoomOptionDTO { Value = "4+1", Text = "4+1" },
                new RoomOptionDTO { Value = "4.5+1", Text = "4.5+1" },
                new RoomOptionDTO { Value = "4.5+2", Text = "4.5+2" },
                new RoomOptionDTO { Value = "4+2", Text = "4+2" },
                new RoomOptionDTO { Value = "4+3", Text = "4+3" },
                new RoomOptionDTO { Value = "4+4", Text = "4+4" },
                new RoomOptionDTO { Value = "5+1", Text = "5+1" },
                new RoomOptionDTO { Value = "5.5+1", Text = "5.5+1" },
                new RoomOptionDTO { Value = "5+2", Text = "5+2" },
                new RoomOptionDTO { Value = "5+3", Text = "5+3" },
                new RoomOptionDTO { Value = "5+4", Text = "5+4" },
                new RoomOptionDTO { Value = "6+1", Text = "6+1" },
                new RoomOptionDTO { Value = "6+2", Text = "6+2" },
                new RoomOptionDTO { Value = "6.5+1", Text = "6.5+1" },
                new RoomOptionDTO { Value = "6+3", Text = "6+3" },
                new RoomOptionDTO { Value = "6+4", Text = "6+4" },
                new RoomOptionDTO { Value = "7+1", Text = "7+1" },
                new RoomOptionDTO { Value = "7+2", Text = "7+2" },
                new RoomOptionDTO { Value = "7+3", Text = "7+3" },
                new RoomOptionDTO { Value = "8+1", Text = "8+1" },
                new RoomOptionDTO { Value = "8+2", Text = "8+2" },
                new RoomOptionDTO { Value = "8+3", Text = "8+3" },
                new RoomOptionDTO { Value = "8+4", Text = "8+4" },
                new RoomOptionDTO { Value = "9+1", Text = "9+1" },
                new RoomOptionDTO { Value = "9+2", Text = "9+2" },
                new RoomOptionDTO { Value = "9+3", Text = "9+3" },
                new RoomOptionDTO { Value = "9+4", Text = "9+4" },
                new RoomOptionDTO { Value = "9+5", Text = "9+5" },
                new RoomOptionDTO { Value = "9+6", Text = "9+6" },
                new RoomOptionDTO { Value = "10+1", Text = "10+1" },
                new RoomOptionDTO { Value = "10+2", Text = "10+2" },
                new RoomOptionDTO { Value = "10 Üzeri", Text = "10 Üzeri" }
            };

            return new ApiResponse<GetRoomOptionsQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Oda seçenekleri başarıyla getirildi.",
                Data = new GetRoomOptionsQueryResponse
                {
                    RoomOptions = roomOptions,
                    Message = "Oda seçenekleri başarıyla getirildi."
                }
            };
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
