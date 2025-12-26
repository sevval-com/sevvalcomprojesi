namespace Sevval.Application.Features.Company.Queries.GetCompanyByName;

public class GetCompaniesQueryResponse
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Neighbourhood { get; set; } = string.Empty;
    public string UserTypes { get; set; } = string.Empty;

    public string IsActive { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string ProfilePicturePath { get; set; }

    public int? TotalAnnouncement { get; set; }
    public int? CompanyMembershipDuration { get; set; }
    public int? UserOrder { get; set; }
}
