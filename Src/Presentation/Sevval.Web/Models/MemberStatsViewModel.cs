namespace sevvalemlak.Models;

public class MemberStatsViewModel
{
    public int ToplamIlanSayisi { get; set; }
    public int FotografliIlanSayisi { get; set; }
    public int FotoografsizIlanSayisi { get; set; }
    public int VideoluIlanSayisi { get; set; }
    public int VideosuzIlanSayisi { get; set; }
    public DateTime? SonIlanTarihi { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
