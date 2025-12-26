namespace Sevval.Application.Dtos.Email
{
    public class ForgotPasswordViewDto
    {
        public string Email { get; set; }
        public string Code { get; set; }  // Doğrulama kodu
        public string NewPassword { get; set; }  // Yeni şifre
    }
}
