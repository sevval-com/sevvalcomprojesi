namespace Sevval.Application.Features.User.Commands.IndividualUpdate;

public class IndividualUpdateCommandResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? ProfilePicturePath { get; set; }
    public DateTime UpdatedDate { get; set; }
}
