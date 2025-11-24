public class MessengerViewModel
{
    public List<SohbetDTO> SohbetListesi { get; set; }
    public List<MesajDTO> SeciliChatMesajlari { get; set; }
    public int? SeciliChatId { get; set; }
}

public class SohbetDTO
{
    public int ChatId { get; set; }
    public string DigerKisiAdSoyad { get; set; }
}

public class MesajDTO
{
    public string Content { get; set; }
    public bool GondericiMi { get; set; }
}
