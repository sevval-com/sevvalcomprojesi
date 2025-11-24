using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Company.Queries.GetCompanyByName;

public class GetCompaniesQueryRequest : IRequest<ApiResponse<List<GetCompaniesQueryResponse>>>
{
    public const string Route = "/api/v1/companies";

    public int Page { get; set; }
    public int Size { get; set; }
    public string? CompanyName { get; set; }
    public string? Search { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? SortBy { get; set; }

    // === ÝSTEK DOÐRULTUSUNDA EKLENEN YENÝ FÝLTRE ALANLARI ===
    // Bu alanlar, HomeController'dan gelen sabit filtreleri (Kurumsal, 0, active)
    // yakalamak için eklendi.
    public string? UserTypes { get; set; }
    public string? IsConsultant { get; set; }
    public string? IsActive { get; set; }
    // =======================================================
}