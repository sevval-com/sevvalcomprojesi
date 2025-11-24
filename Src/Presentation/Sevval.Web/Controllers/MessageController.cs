using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Persistence.Context;

public class MessageController : Controller
{
    private readonly ApplicationDbContext _context;

    public MessageController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Send(string receiverEmail, string receiverFullName, string content, int ilanId)
    {
        var senderEmail = User?.Identity?.Name;
        var senderUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == senderEmail);
        if (senderUser == null)
            return Unauthorized();

        // Veritabanındaki genel en yüksek ChatId'yi bul
        int newChatId = 1;
        var maxChatId = await _context.Messages
            .MaxAsync(m => (int?)m.ChatId);

        if (maxChatId.HasValue)
        {
            newChatId = maxChatId.Value + 1;
        }

        var message = new Message
        {
            ReceiverEmail = receiverEmail,
            ReceiverFullName = receiverFullName,
            SenderEmail = senderEmail,
            SenderFullName = senderUser.FirstName + " " + senderUser.LastName,
            Content = content,
            SentDate = DateTime.Now,
            ChatId = newChatId,
            IlanId = ilanId,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Başarılı işlemden sonra "Mesajlar" sayfasına yönlendir
        return Json(new { success = true, redirectUrl = Url.Action("Mesajlar", "Ilan") });
    }

}