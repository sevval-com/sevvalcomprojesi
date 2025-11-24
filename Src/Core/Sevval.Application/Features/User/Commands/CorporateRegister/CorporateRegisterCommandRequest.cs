using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sevval.Application.Features.User.Commands.CorporateRegister;

public class CorporateRegisterCommandRequest : IRequest<ApiResponse<CorporateRegisterCommandResponse>>
{
    public const string Route = "/api/v1/corporate-register";
    public string UserTypes { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string CompanyName { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Address { get; set; }
    public IFormFile Level5Certificate { get; set; }
    public IFormFile TaxPlate { get; set; }

    [NotMapped]
    [SwaggerIgnore]
    public string Level5CertificatePath { get; set; }

    [NotMapped]
    [SwaggerIgnore]
    public string TaxPlatePath { get; set; }

    public string Reference { get; set; }

    public IFormFile? ProfilePicture { get; set; } // Profil fotoğrafı nullable hale getirildi

    [NotMapped]
    [SwaggerIgnore]
    public string? ProfilePicturePath { get; set; } // Nullable hale getirildi
}
