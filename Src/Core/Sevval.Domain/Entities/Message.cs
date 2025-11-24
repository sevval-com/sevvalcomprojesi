public class Message
{
    public int Id { get; set; }

    public int? ChatId { get; set; }

    public int? IlanId { get; set; }

    public string ReceiverEmail { get; set; }

    public string ReceiverFullName { get; set; }

    public string SenderEmail { get; set; }

    public string SenderFullName { get; set; }

    public string Content { get; set; }

    public DateTime SentDate { get; set; }

    public bool IsRead { get; set; } = false; // Yeni eklendi, varsayılan olarak "Okunmadı"
}
