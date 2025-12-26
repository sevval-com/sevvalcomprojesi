namespace Sevval.Domain.Entities;

public class Visitor
{
    public int Id { get; set; }
    public string IpAddress { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? VisitTime { get; set; }
}

