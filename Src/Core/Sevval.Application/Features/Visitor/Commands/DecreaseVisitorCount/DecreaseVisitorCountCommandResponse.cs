namespace Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount
{
    public class DecreaseVisitorCountCommandResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public int NewVisitorCount { get; set; }
    }
}
