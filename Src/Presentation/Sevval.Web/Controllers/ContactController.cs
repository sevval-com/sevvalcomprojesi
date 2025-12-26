using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

public class ContactController : Controller
{
    [HttpPost]
    public async Task<IActionResult> SendEmail(string name, string email, string subject, string message)
    {
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(email);
        mailMessage.To.Add("sevvalemlakiletisim@gmail.com");
        mailMessage.Subject = subject;
        mailMessage.Body = $"Ad Soyad: {name}\nE-posta: {email}\nMesaj: {message}";
        mailMessage.IsBodyHtml = false;

        using (var smtpClient = new SmtpClient("smtp.gmail.com")) // Gmail için SMTP sunucusu
        {
            smtpClient.Port = 587; // Gmail için SMTP portu
            smtpClient.Credentials = new NetworkCredential("sevvalemlak@gmail.com", "2020SevvalEmlak2020."); // Kendi e-posta adresi ve şifre
            smtpClient.EnableSsl = true; // SSL kullanımı
            await smtpClient.SendMailAsync(mailMessage); // E-posta gönderme
        }


        return RedirectToAction("About"); // Başka bir sayfaya yönlendirme
    }
}
