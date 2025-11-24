using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Interfaces.IService;
using sevvalemlak.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEMailService _emailService;

        // Dependency Injection ile EmailService sınıfını alıyoruz
        public EmailController(IEMailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendEmail(string Name, string Email, string Subject, string Message)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your_email@gmail.com", "your_email_password"),
                    EnableSsl = true,
                };

                smtpClient.Send("your_email@gmail.com", "sevvalemlakiletisim@gmail.com", Subject, $"Ad Soyad: {Name}\nE-posta: {Email}\nMesaj: {Message}");

                // Başarı durumunda kullanıcıya mesaj gösterme
                ViewBag.Message = "Mesajınız başarıyla gönderildi!";
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösterme
                ViewBag.Message = "Bir hata oluştu: " + ex.Message;
            }

            return View("About"); // Aynı sayfaya geri döner
        }


    }

    public class EmailRequest
    {
        public string Email { get; set; }
        public string PropertyDetails { get; set; }
    }
}
