
namespace sevvalemlak.Services
{
    public interface INetGsmService
    {
        Task<string> SendSmsAsync(List<string> phoneNumbers, string message);
    }
}