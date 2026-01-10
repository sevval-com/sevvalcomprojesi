using MediatR;

namespace Sevval.Application.Features.Messaging.Queries.GetUnreadCountsByCategory;

public class GetUnreadCountsByCategoryQuery : IRequest<GetUnreadCountsByCategoryResult>
{
    public string UserId { get; set; } = string.Empty;
}
