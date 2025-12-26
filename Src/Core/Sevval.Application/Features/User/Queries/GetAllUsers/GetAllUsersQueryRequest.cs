using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Queries.GetAllUsers
{
    public class GetAllUsersQueryRequest : IRequest<ApiResponse<IList<GetAllUsersQueryResponse>>>
    {
        public const string Route = "/api/v1/users";
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public bool? Status { get; set; }
    }
}
