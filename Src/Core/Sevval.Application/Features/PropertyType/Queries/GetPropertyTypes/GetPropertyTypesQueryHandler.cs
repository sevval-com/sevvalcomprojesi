using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;

public class GetPropertyTypesQueryHandler : IRequestHandler<GetPropertyTypesQueryRequest, ApiResponse<GetPropertyTypesQueryResponse>>
{
    private readonly IPropertyTypeService _propertyTypeService;

    public GetPropertyTypesQueryHandler(IPropertyTypeService propertyTypeService)
    {
        _propertyTypeService = propertyTypeService;
    }

    public async Task<ApiResponse<GetPropertyTypesQueryResponse>> Handle(GetPropertyTypesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _propertyTypeService.GetPropertyTypesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetPropertyTypesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Mülk tipleri getirilirken bir hata oluştu.",
                Data = new GetPropertyTypesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
