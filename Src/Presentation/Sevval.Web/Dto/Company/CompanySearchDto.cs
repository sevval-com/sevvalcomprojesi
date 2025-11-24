using Sevval.Application.Features.Company.Queries.GetCompanyByName;

namespace sevvalemlak.csproj.Dto.Company;

public class CompanySearchDto
{
    public string? Search { get; set; }
    public string? Province { get; set; }
    public string? SortBy { get; set; }
    public string? District { get; set; }

    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;

    public List<GetCompaniesQueryResponse> Companies { get; set; }=new List<GetCompaniesQueryResponse>();

}
