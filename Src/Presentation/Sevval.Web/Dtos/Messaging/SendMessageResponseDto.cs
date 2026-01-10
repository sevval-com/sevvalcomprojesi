namespace Sevval.Web.Dtos.Messaging;

public class SendMessageResponseDto
{
    public Guid MessageId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }

    public string? Error { get; set; }
}
