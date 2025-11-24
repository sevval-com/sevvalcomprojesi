using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.LandType.Queries.GetLandTypes;

public class GetLandTypesQueryHandler : IRequestHandler<GetLandTypesQueryRequest, ApiResponse<GetLandTypesQueryResponse>>
{
    private readonly ILandTypeService _landTypeService;

    public GetLandTypesQueryHandler(ILandTypeService landTypeService)
    {
        _landTypeService = landTypeService;
    }

    public async Task<ApiResponse<GetLandTypesQueryResponse>> Handle(GetLandTypesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _landTypeService.GetLandTypesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetLandTypesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Arsa tipleri getirilirken bir hata oluştu.",
                Data = new GetLandTypesQueryResponse
                {
                    Message = ex.Message,
                    LandTypes = new List<DTOs.LandType.LandTypeDTO>()
                }
            };
        }
    }
}
