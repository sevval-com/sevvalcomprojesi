using MediatR;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Admin.Queries.GetCorporateUsers;

public class GetCorporateUsersQueryRequest : IRequest<ApiResponse<GetCorporateUsersQueryResponse>>
{
    public string? UserType { get; set; } // "Emlakçı", "İnşaat", "Banka", "Vakıf", "Tümü"
    public string? Status { get; set; } // "active", "passive", "deleted", "All"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
