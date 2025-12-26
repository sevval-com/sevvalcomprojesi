namespace Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount
{
    public class IncreaseVisitorCountCommandResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public int NewVisitorCount { get; set; }
    }
}
