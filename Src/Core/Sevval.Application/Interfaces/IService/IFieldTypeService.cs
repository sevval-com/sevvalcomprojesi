using Sevval.Application.Features.FieldType.Queries.GetFieldTypes;
using Sevval.Application.Features.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.IService
{
    public interface IFieldTypeService
    {
        Task<ApiResponse<GetFieldTypesQueryResponse>> GetFieldTypesAsync(CancellationToken cancellationToken);
    }
}
