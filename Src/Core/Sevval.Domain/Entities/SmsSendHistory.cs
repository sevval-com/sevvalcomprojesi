namespace Sevval.Domain.Entities;

public class SmsSendHistory
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Message { get; set; }
    public DateTime SendDate { get; set; }
    public string JobId { get; set; }
    public string Status { get; set; } // "success", "failed", "pending"
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
}