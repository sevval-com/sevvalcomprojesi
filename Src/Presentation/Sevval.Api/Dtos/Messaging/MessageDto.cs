using System;

namespace Sevval.Api.Dtos.Messaging;

public class MessageDto
{
    public Guid Id { get; set; }

    public string SenderId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public DateTime CreatedOnUtc { get; set; }

    public string Status { get; set; } = string.Empty;
}
