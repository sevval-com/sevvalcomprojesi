namespace Sevval.Application.Dtos.Email
{
    public class SendVerifyEmailDto
    {
        public string Email { get; set; }
        public string Link { get; set; }
        public string Code { get; set; }
    }
}
