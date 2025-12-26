using System;
using Sevval.Application.Features.Admin.Queries.GetCorporateUsers;

namespace Sevval.Application.Features.Admin.Queries.GetUserStats;

public class GetUserStatsQueryResponse
{
    public UserInfoDto UserInfo { get; set; } = new();
    public UserStatsDto Statistics { get; set; } = new();
}

public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserTypes { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public bool IsConsultant { get; set; }
    public string? InvitedBy { get; set; }
    public string? ParentCompanyName { get; set; }
    public string IsActive { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
