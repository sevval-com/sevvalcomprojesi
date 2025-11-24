using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.About.Queries.GetAboutContent;

public class GetAboutContentQueryRequest : IRequest<ApiResponse<GetAboutContentQueryResponse>>
{
    public const string Route = "/api/v1/about";

}
