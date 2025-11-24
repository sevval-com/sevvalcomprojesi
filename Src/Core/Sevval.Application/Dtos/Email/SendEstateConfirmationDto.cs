using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Sevval.Application.Dtos.Email
{
    public class SendEstateConfirmationDto
    {
        public string Email { get; set; }
        public string ConfirmUrl { get; set; }
        public string RejectUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string Reference { get; set; }

        [SwaggerIgnore]
        [JsonIgnore]
        public string Level5CertificatePath { get; set; }

        [SwaggerIgnore]
        [JsonIgnore]
        public string TaxPlatePath { get; set; }

        [SwaggerIgnore]
        [JsonIgnore]
        public string? ProfilePicturePath { get; set; } // Nullable hale getirildi


    }
}
