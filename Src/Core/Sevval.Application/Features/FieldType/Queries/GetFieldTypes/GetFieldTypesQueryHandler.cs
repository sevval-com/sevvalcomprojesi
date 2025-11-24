using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.FieldType.Queries.GetFieldTypes;

public class GetFieldTypesQueryHandler : IRequestHandler<GetFieldTypesQueryRequest, ApiResponse<GetFieldTypesQueryResponse>>
{
    private readonly IFieldTypeService _fieldTypeService;

    public GetFieldTypesQueryHandler(IFieldTypeService fieldTypeService)
    {
        _fieldTypeService = fieldTypeService;
    }

    public async Task<ApiResponse<GetFieldTypesQueryResponse>> Handle(GetFieldTypesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _fieldTypeService.GetFieldTypesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetFieldTypesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Tarla tipleri getirilirken bir hata oluştu: " + ex.Message
            };
        }
    }
}
