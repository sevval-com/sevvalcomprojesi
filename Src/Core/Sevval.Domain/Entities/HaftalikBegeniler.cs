namespace Sevval.Domain.Entities;

public class HaftalikBegeniler
{
    public int Id { get; set; }
    public string UserEmail { get; set; }
    public int Pazar { get; set; }
    public int Pazartesi { get; set; }
    public int Sali { get; set; }
    public int Carsamba { get; set; }
    public int Persembe { get; set; }
    public int Cuma { get; set; }
    public int Cumartesi { get; set; }
    public int Toplam { get; set; }
}
