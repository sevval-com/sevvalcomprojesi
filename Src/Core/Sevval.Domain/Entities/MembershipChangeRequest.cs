using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sevval.Domain.Entities;

public class MembershipChangeRequest
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    public string CurrentType { get; set; }
    public string RequestedType { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string TaxNumber { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ProcessDate { get; set; }
    public string Status { get; set; }
    public string? AdminNote { get; set; }

    [JsonIgnore]
    public string? Level5CertificatePath { get; set; }

    [JsonIgnore]
    public string? TaxDocumentPath { get; set; }
}
