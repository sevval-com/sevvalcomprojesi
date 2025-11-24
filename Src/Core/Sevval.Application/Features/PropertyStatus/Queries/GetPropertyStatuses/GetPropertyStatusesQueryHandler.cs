using MediatR;
using Sevval.Application.Interfaces.Services;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;

public class GetPropertyStatusesQueryHandler : IRequestHandler<GetPropertyStatusesQueryRequest, ApiResponse<GetPropertyStatusesQueryResponse>>
{
    private readonly IPropertyStatusService _propertyStatusService;

    public GetPropertyStatusesQueryHandler(IPropertyStatusService propertyStatusService)
    {
        _propertyStatusService = propertyStatusService;
    }

    public async Task<ApiResponse<GetPropertyStatusesQueryResponse>> Handle(GetPropertyStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _propertyStatusService.GetPropertyStatusesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetPropertyStatusesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Konut durumları getirilirken bir hata oluştu: " + ex.Message
            };
        }
    }
}
