namespace Sevval.Web.Dtos.Messaging;

public class UnreadCountsResponseDto
{
    public IReadOnlyDictionary<string, int> Counts { get; set; } =
        new Dictionary<string, int>();
}
