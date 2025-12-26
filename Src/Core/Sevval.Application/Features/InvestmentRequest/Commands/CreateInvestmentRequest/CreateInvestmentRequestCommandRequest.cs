using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;

public class CreateInvestmentRequestCommandRequest : IRequest<ApiResponse<CreateInvestmentRequestCommandResponse>>
{
    public const string Route = "/api/v1/investment-requests";

    public string? SelectCategory { get; set; }
    public string? SelectStatus { get; set; }
    public string? Rooms { get; set; }
    public string? Area { get; set; }
    public string? BuildingAge { get; set; }
    public string? Floor { get; set; }
    public string? Bathrooms { get; set; }
    public string? HeatingSystem { get; set; }
    public string? Parking { get; set; }
    public string? Price { get; set; }
    public string? ResidentialCity { get; set; }
    public string? ResidentialDistrict { get; set; }
    public string? ResidentialVillage { get; set; }
    public string? LandArea { get; set; }
    public string? Slope { get; set; }
    public string? RoadCondition { get; set; }
    public string? DistanceToSettlement { get; set; }
    public string? ZoningStatus { get; set; }
    public string? LandCity { get; set; }
    public string? LandDistrict { get; set; }
    public string? LandVillage { get; set; }
    public string? LandPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LivingCity { get; set; }
    public string? InterestedPerson { get; set; }
    public string? MinBudget { get; set; }
    public string? MaxBudget { get; set; }
}
