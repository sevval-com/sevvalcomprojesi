namespace Sevval.Api.Dtos.Messaging;

public class SendMessageRequestDto
{
    // TODO: SenderId'nin kimlik doğrulama kullanıcı bağlamından (claims) gelmesi gerekiyor...
    public string SenderId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}
