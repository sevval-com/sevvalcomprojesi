using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Application.Features.User.Commands.CorporateUpdate;

public class CorporateUpdateCommandRequest : IRequest<ApiResponse<CorporateUpdateCommandResponse>>
{
    public const string Route = "/api/v1/corporate-users";
    public string Id { get; set; } // User ID to identify which user to update
    public string UserTypes { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? Password { get; set; } // Optional - only update if provided
    public string? ConfirmPassword { get; set; }
    public string CompanyName { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Address { get; set; }
    public IFormFile? Level5Certificate { get; set; } // Optional - only update if provided
    public IFormFile? TaxPlate { get; set; } // Optional - only update if provided

    [NotMapped]
    [SwaggerIgnore]
    public string? Level5CertificatePath { get; set; }

    [NotMapped]
    [SwaggerIgnore]
    public string? TaxPlatePath { get; set; }

    public string Reference { get; set; }
    public string TaxNumber { get; set; }

    public IFormFile? ProfilePicture { get; set; } // Optional - only update if provided

    [NotMapped]
    [SwaggerIgnore]
    public string? ProfilePicturePath { get; set; }

    public bool RemoveProfilePicture { get; set; } = false; // Flag to remove existing profile picture

    public bool RemoveLevel5Certificate { get; set; } = false; // Flag to remove existing certificate
    public bool RemoveTaxPlate { get; set; } = false; // Flag to remove existing tax plate
}
