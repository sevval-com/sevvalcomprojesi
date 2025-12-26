using Sevval.Application.Features.About.Queries.GetAboutContent;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface IAboutService
{
    Task<ApiResponse<GetAboutContentQueryResponse>> GetAboutContentAsync(CancellationToken cancellationToken);
}
