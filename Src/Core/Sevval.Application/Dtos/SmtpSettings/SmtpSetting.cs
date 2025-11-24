namespace Sevval.Application.Dtos.SmtpSettings
{
    public class SmtpSetting
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public string AdminAddress { get; set; }
        public string AdminAddress2 { get; set; } // İkinci admin adresi
    }
}
