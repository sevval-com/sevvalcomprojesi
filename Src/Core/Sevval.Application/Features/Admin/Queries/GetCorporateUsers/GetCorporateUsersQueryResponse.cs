using System;
using System.Collections.Generic;

namespace Sevval.Application.Features.Admin.Queries.GetCorporateUsers;

public class GetCorporateUsersQueryResponse
{
    public List<CorporateUserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

public class CorporateUserDto
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
    
    public UserStatsDto Stats { get; set; } = new();
    public UserDocumentsDto Documents { get; set; } = new();
}

public class UserStatsDto
{
    public int TotalAnnouncements { get; set; }
    public int PhotoAnnouncements { get; set; }
    public int VideoAnnouncements { get; set; }
    public int NoPhotoAnnouncements { get; set; }
    public int NoVideoAnnouncements { get; set; }
    public DateTime? LastAnnouncementDate { get; set; }
    public DateTime? FirstAnnouncementDate { get; set; }
}

public class UserDocumentsDto
{
    public string? Level5CertificatePath { get; set; }
    public string? TaxPlatePath { get; set; }
    public string? ContractorCertificatePath { get; set; }
    public string? SignatureCircularPath { get; set; }
    public string? FoundationDeedPath { get; set; }
    public string? ProfilePicturePath { get; set; }
}
