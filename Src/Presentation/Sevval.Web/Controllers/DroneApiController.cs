using System.Security.Claims;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/drone")]
public class DroneApiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DroneApiController> _logger;

    public DroneApiController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DroneApiController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] DroneCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.Lang))
        {
            return BadRequest(new { success = false, message = "Eksik parametreler." });
        }

        var baseUrl = _configuration["SevvalVideo:BaseUrl"];
        var apiKey = _configuration["SevvalVideo:ApiKey"];
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            return StatusCode(500, new { success = false, message = "Video servisi ayarlari eksik." });
        }

        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "sevval";
        var videoId = Guid.NewGuid().ToString("N");
        var payload = new
        {
            videoId,
            description = request.Description,
            language = request.Lang,
            userId,
            propertyDetails = new
            {
                il = request.Il,
                ilce = request.Ilce,
                mahalle = request.Mahalle,
                ada = request.Ada,
                parsel = request.Parsel,
                tapu = request.Tapu
            }
        };

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(15);

        var requestUrl = $"{baseUrl.TrimEnd('/')}/api/sevval/process-video";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = JsonContent.Create(payload)
        };
        httpRequest.Headers.Add("x-api-key", apiKey);
        httpRequest.Headers.Add("Origin", "https://sevval.com");

        try
        {
            var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Sevval video istegi basarisiz. Status: {Status} Body: {Body}", response.StatusCode, responseBody);
                return StatusCode((int)response.StatusCode, new { success = false, message = "Video istegi basarisiz." });
            }

            return Ok(new { success = true, message = "Istek alindi.", payload = responseBody });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sevval video istegi hatasi");
            return StatusCode(500, new { success = false, message = "Video istegi gonderilemedi." });
        }
    }

    public class DroneCreateRequest
    {
        public string? Il { get; set; }
        public string? Ilce { get; set; }
        public string? Mahalle { get; set; }
        public string? Ada { get; set; }
        public string? Parsel { get; set; }
        public string? Lang { get; set; }
        public string? Tapu { get; set; }
        public string? Description { get; set; }
        public IFormFile? Logo { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
