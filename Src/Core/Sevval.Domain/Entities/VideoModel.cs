namespace Sevval.Domain.Entities;

public class VideoModel
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public int IlanId { get; set; }  // İlan ID'sini ilişkilendirmek için
    public string? VideoUrl { get; set; }  // Video URL'si
}

