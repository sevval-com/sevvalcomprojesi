using MediatR;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Features.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses
{
    public class GetFieldStatusesQueryHandler : IRequestHandler<GetFieldStatusesQueryRequest, ApiResponse<GetFieldStatusesQueryResponse>>
    {
        private readonly IFieldStatusService _fieldStatusService;

        public GetFieldStatusesQueryHandler(IFieldStatusService fieldStatusService)
        {
            _fieldStatusService = fieldStatusService;
        }

        public async Task<ApiResponse<GetFieldStatusesQueryResponse>> Handle(GetFieldStatusesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _fieldStatusService.GetFieldStatusesAsync(cancellationToken);
        }
    }
}
