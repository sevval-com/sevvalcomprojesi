namespace sevvalemlak.Models
{
    public class SendVerifyEmailModel
    {
        public string Email { get; set; }
        public string Link { get; set; }
        public string Code { get; set; }
    }
}
