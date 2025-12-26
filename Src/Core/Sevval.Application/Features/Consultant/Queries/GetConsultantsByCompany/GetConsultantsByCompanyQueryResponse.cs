using Swashbuckle.AspNetCore.Annotations;

namespace Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

public class GetConsultantsByCompanyQueryResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePhoto { get; set; }

    [SwaggerIgnore]
    public string Email { get; set; }
    public int TotalAnnouncementCount { get; set; }
}

