using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;

public class GetBuildingAgesQueryHandler : IRequestHandler<GetBuildingAgesQueryRequest, ApiResponse<GetBuildingAgesQueryResponse>>
{
    private readonly IBuildingAgeService _buildingAgeService;

    public GetBuildingAgesQueryHandler(IBuildingAgeService buildingAgeService)
    {
        _buildingAgeService = buildingAgeService;
    }

    public async Task<ApiResponse<GetBuildingAgesQueryResponse>> Handle(GetBuildingAgesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _buildingAgeService.GetBuildingAgesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetBuildingAgesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Bina yaşı seçenekleri getirilirken bir hata oluştu.",
                Data = new GetBuildingAgesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
