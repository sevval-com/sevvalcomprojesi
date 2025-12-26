using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using sevvalemlak.Dto;

namespace sevvalemlak.Services;

public class NetGsmService: INetGsmService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public NetGsmService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<string> SendSmsAsync(List<string> phoneNumbers, string message)
    {
        try
        {
            var username = _configuration["NetGSM:Username"];
            var password = _configuration["NetGSM:Password"];
            var messageHeader = _configuration["NetGSM:MessageHeader"];

            var messageItems = new List<MessageItem>();
            foreach (var phone in phoneNumbers)
                messageItems.Add(new MessageItem
                {
                    msg = message,
                    no = phone
                });

            var data = new NetGsmSmsRequest
            {
                msgheader = messageHeader,
                messages = messageItems.ToArray(),
                encoding = "TR"
            };

            var url = "https://api.netgsm.com.tr/sms/rest/v2/send";

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
        catch (Exception ex)
        {
            return $"Hata: {ex.Message}";
        }
    }
}