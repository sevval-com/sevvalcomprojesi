using System;
using System.Collections.Generic;

namespace Sevval.Api.Dtos.Messaging;

public class GetConversationMessagesResponseDto
{
    public IReadOnlyList<MessageDto> Items { get; set; } = Array.Empty<MessageDto>();
}
