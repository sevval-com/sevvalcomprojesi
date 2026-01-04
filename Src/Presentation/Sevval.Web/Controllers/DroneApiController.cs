using System.Security.Claims;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Sevval.Domain.Entities;

[ApiController]
[Route("api/drone")]
public class DroneApiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DroneApiController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private const long MaxUploadBytes = 5 * 1024 * 1024;

    public DroneApiController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DroneApiController> logger,
        UserManager<ApplicationUser> userManager)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] DroneCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.Lang))
        {
            return BadRequest(new { success = false, message = "Eksik parametreler." });
        }
        if (request.Logo != null && request.Logo.Length > MaxUploadBytes)
        {
            return StatusCode(413, new { success = false, message = "Firma logosu 5MB üstü. Lütfen değiştirin." });
        }
        if (request.Avatar != null && request.Avatar.Length > MaxUploadBytes)
        {
            return StatusCode(413, new { success = false, message = "Danışman fotoğrafı 5MB üstü. Lütfen değiştirin." });
        }

        var baseUrl = _configuration["SevvalVideo:BaseUrl"];
        var dataApiBaseUrl = _configuration["EmlakDroneApi:BaseUrl"];
        var droneApiKey = _configuration["EmlakDroneApi:ApiKey"];
        var dataApiKey = _configuration["EmlakDroneApi:VideoDataApiKey"] ?? droneApiKey;
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return StatusCode(500, new { success = false, message = "Video servisi ayarlari eksik." });
        }
        if (string.IsNullOrWhiteSpace(dataApiBaseUrl) || string.IsNullOrWhiteSpace(dataApiKey))
        {
            return StatusCode(500, new { success = false, message = "Video data servisi ayarlari eksik." });
        }

        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "sevval";
        var firstName = User?.FindFirstValue("FirstName") ?? string.Empty;
        var lastName = User?.FindFirstValue("LastName") ?? string.Empty;
        var phoneNumber = User?.FindFirstValue("PhoneNumber") ?? string.Empty;

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            var email = User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("Email");
            if (!string.IsNullOrWhiteSpace(email))
            {
                currentUser = await _userManager.FindByEmailAsync(email);
            }
        }

        if (currentUser == null && userId != "sevval")
        {
            currentUser = await _userManager.FindByIdAsync(userId);
        }

        if (currentUser != null)
        {
            if (string.IsNullOrWhiteSpace(firstName)) firstName = currentUser.FirstName ?? string.Empty;
            if (string.IsNullOrWhiteSpace(lastName)) lastName = currentUser.LastName ?? string.Empty;
            if (string.IsNullOrWhiteSpace(phoneNumber)) phoneNumber = currentUser.PhoneNumber ?? string.Empty;
        }

        _logger.LogInformation(
            "Drone create user data. Auth={IsAuth} UserId={UserId} First={First} Last={Last} Phone={Phone} EmailClaim={EmailClaim} UserFound={UserFound}",
            User?.Identity?.IsAuthenticated == true,
            userId,
            firstName,
            lastName,
            phoneNumber,
            User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("Email"),
            currentUser != null
        );
        var logoDataUrl = await ToDataUrlAsync(request.Logo);
        var avatarDataUrl = await ToDataUrlAsync(request.Avatar);
        if (currentUser != null)
        {
            var (logoPath, avatarPath) = await ResolveUserMediaPathsAsync(currentUser);
            if (logoDataUrl == null)
            {
                if (IsMediaTooLarge(logoPath))
                {
                    return StatusCode(413, new { success = false, message = "Firma logosu 5MB üstü. Lütfen değiştirin." });
                }
                logoDataUrl = await ToDataUrlFromPathAsync(logoPath);
            }
            if (avatarDataUrl == null)
            {
                if (IsMediaTooLarge(avatarPath))
                {
                    return StatusCode(413, new { success = false, message = "Danışman fotoğrafı 5MB üstü. Lütfen değiştirin." });
                }
                avatarDataUrl = await ToDataUrlFromPathAsync(avatarPath);
            }
        }
        var jobId = (string?)null;
        var parcelId = (string?)null;
        var preparedVideoId = (string?)null;

        if (!string.IsNullOrWhiteSpace(dataApiBaseUrl) && !string.IsNullOrWhiteSpace(droneApiKey))
        {
            try
            {
                var jobClient = _httpClientFactory.CreateClient();
                jobClient.DefaultRequestHeaders.Add("x-api-key", droneApiKey);
                await AttachUserHeadersAsync(jobClient);

                var jobResponse = await jobClient.PostAsJsonAsync(
                    $"{dataApiBaseUrl.TrimEnd('/')}/sevvaldrone/api/jobs/prepare",
                    new
                    {
                        parcel = new
                        {
                            il = request.Il,
                            ilce = request.Ilce,
                            mahalle = request.Mahalle,
                            ada = request.Ada,
                            parsel = request.Parsel,
                            nitelik = request.Tapu,
                            mevkii = "",
                            alan = 0,
                            coordinates = Array.Empty<object>()
                        }
                    });

                var jobBody = await jobResponse.Content.ReadAsStringAsync();
                if (!jobResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Drone job prepare istegi basarisiz. Status: {Status} Body: {Body}", jobResponse.StatusCode, jobBody);
                    return StatusCode((int)jobResponse.StatusCode, new { success = false, message = "Drone job olusturulamadi." });
                }

                var jobPayload = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jobBody);
                if (jobPayload != null)
                {
                    jobId = jobPayload.TryGetValue("jobId", out var jobVal) ? jobVal?.ToString() : null;
                    parcelId = jobPayload.TryGetValue("parcelId", out var parcelVal) ? parcelVal?.ToString() : null;
                    preparedVideoId = jobPayload.TryGetValue("videoId", out var videoVal) ? videoVal?.ToString() : null;
                }

                if (string.IsNullOrWhiteSpace(jobId))
                {
                    _logger.LogWarning("Drone job prepare response missing jobId. Body: {Body}", jobBody);
                    return StatusCode(502, new { success = false, message = "Drone job olusturulamadi." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Drone job prepare istegi basarisiz.");
            }
        }

        var videoId = preparedVideoId ?? Guid.NewGuid().ToString("N");
        var payload = new
        {
            videoId,
            description = request.Description,
            language = request.Lang,
            userId,
            brand = "sevval",
            source = "web",
            jobId,
            parcelId,
            userData = new
            {
                name = firstName,
                surname = lastName,
                phone = phoneNumber
            },
            logo = logoDataUrl,
            avatar = avatarDataUrl,
            propertyDetails = new
            {
                il = request.Il,
                ilce = request.Ilce,
                mahalle = request.Mahalle,
                adaNo = request.Ada,
                parselNo = request.Parsel,
                tapu = request.Tapu
            }
        };

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(15);

        try
        {
            var dataRequestUrl = $"{dataApiBaseUrl.TrimEnd('/')}/api/video-data";
            using var dataRequest = new HttpRequestMessage(HttpMethod.Post, dataRequestUrl)
            {
                Content = JsonContent.Create(payload)
            };
            dataRequest.Headers.Add("x-api-key", dataApiKey);

            var dataResponse = await client.SendAsync(dataRequest, HttpCompletionOption.ResponseHeadersRead);
            var dataBody = await dataResponse.Content.ReadAsStringAsync();
            if (!dataResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Video data istegi basarisiz. Status: {Status} Body: {Body}", dataResponse.StatusCode, dataBody);
                return StatusCode((int)dataResponse.StatusCode, new { success = false, message = "Video verisi olusturulamadi." });
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    var requestUrl = $"{baseUrl.TrimEnd('/')}/api/video/process-video";
                    using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                    {
                        Content = JsonContent.Create(payload)
                    };
                    var backgroundClient = _httpClientFactory.CreateClient();
                    backgroundClient.Timeout = TimeSpan.FromMinutes(15);
                    var response = await backgroundClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Sevval video istegi basarisiz (async). Status: {Status} Body: {Body}", response.StatusCode, responseBody);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sevval video istegi hatasi (async)");
                }
            });

            return Ok(new
            {
                success = true,
                message = "Istek alindi.",
                videoId,
                jobId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sevval video istegi hatasi");
            return StatusCode(500, new { success = false, message = "Video istegi gonderilemedi." });
        }
    }

    [HttpPost("save-parcel")]
    public async Task<IActionResult> SaveParcel([FromBody] DroneParcelSaveRequest request)
    {
        try
        {
            var dataApiBaseUrl = _configuration["EmlakDroneApi:BaseUrl"];
            var dataApiKey = _configuration["EmlakDroneApi:ApiKey"];
            if (string.IsNullOrWhiteSpace(dataApiBaseUrl) || string.IsNullOrWhiteSpace(dataApiKey))
            {
                return StatusCode(500, new { success = false, message = "Drone servis ayarlari eksik." });
            }

            _logger.LogInformation("SaveParcel hit. BaseUrl={BaseUrl}", dataApiBaseUrl);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", dataApiKey);
            await AttachUserHeadersAsync(client);

            var response = await client.PostAsJsonAsync($"{dataApiBaseUrl.TrimEnd('/')}/sevvaldrone/api/parcels/save", request);
            var length = response.Content.Headers.ContentLength;
            _logger.LogInformation("SaveParcel upstream status={Status} length={Length}", response.StatusCode, length);
            if (length == 0)
            {
                _logger.LogWarning("Drone API empty response for save-parcel. Status={Status}", response.StatusCode);
                return StatusCode(502, new
                {
                    success = false,
                    message = "Drone API boş cevap döndü.",
                    status = (int)response.StatusCode
                });
            }

            return await ForwardResponseAsync(response, "save-parcel");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveParcel proxy error");
            return StatusCode(500, new { success = false, message = "Parsel kaydetme istegi iletilemedi." });
        }
    }

    [HttpGet("saved-parcels")]
    public async Task<IActionResult> GetSavedParcels()
    {
        var dataApiBaseUrl = _configuration["EmlakDroneApi:BaseUrl"];
        var dataApiKey = _configuration["EmlakDroneApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(dataApiBaseUrl) || string.IsNullOrWhiteSpace(dataApiKey))
        {
            return StatusCode(500, new { success = false, message = "Drone servis ayarlari eksik." });
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", dataApiKey);
        await AttachUserHeadersAsync(client);

        var response = await client.GetAsync($"{dataApiBaseUrl.TrimEnd('/')}/sevvaldrone/api/parcels");
        return await ForwardResponseAsync(response, "saved-parcels", new { success = true, items = Array.Empty<object>() });
    }

    [HttpPost("create-job")]
    public async Task<IActionResult> CreateJob([FromBody] DroneJobRequest request)
    {
        var dataApiBaseUrl = _configuration["EmlakDroneApi:BaseUrl"];
        var dataApiKey = _configuration["EmlakDroneApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(dataApiBaseUrl) || string.IsNullOrWhiteSpace(dataApiKey))
        {
            return StatusCode(500, new { success = false, message = "Drone servis ayarlari eksik." });
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", dataApiKey);
        await AttachUserHeadersAsync(client);

        var response = await client.PostAsJsonAsync($"{dataApiBaseUrl.TrimEnd('/')}/sevvaldrone/api/jobs", request);
        return await ForwardResponseAsync(response, "create-job");
    }

    [HttpGet("videos")]
    public async Task<IActionResult> GetVideos()
    {
        var dataApiBaseUrl = _configuration["EmlakDroneApi:BaseUrl"];
        var dataApiKey = _configuration["EmlakDroneApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(dataApiBaseUrl) || string.IsNullOrWhiteSpace(dataApiKey))
        {
            return StatusCode(500, new { success = false, message = "Drone servis ayarlari eksik." });
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", dataApiKey);
        await AttachUserHeadersAsync(client);

        var response = await client.GetAsync($"{dataApiBaseUrl.TrimEnd('/')}/sevvaldrone/api/videos");
        return await ForwardResponseAsync(response, "videos", new { success = true, items = Array.Empty<object>() });
    }

    [HttpGet("user-media")]
    public async Task<IActionResult> GetUserMedia()
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Ok(new { logoUrl = (string?)null, avatarUrl = (string?)null });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Ok(new { logoUrl = (string?)null, avatarUrl = (string?)null });
        }

        var (logoPath, avatarPath) = await ResolveUserMediaPathsAsync(currentUser);
        return Ok(new
        {
            logoUrl = NormalizePublicUrl(logoPath),
            avatarUrl = NormalizePublicUrl(avatarPath),
            logoTooLarge = IsMediaTooLarge(logoPath),
            avatarTooLarge = IsMediaTooLarge(avatarPath)
        });
    }


    public class DroneParcelSaveRequest
    {
        public string? Il { get; set; }
        public string? Ilce { get; set; }
        public string? Mahalle { get; set; }
        public string? Mevkii { get; set; }
        public string? Ada { get; set; }
        public string? Parsel { get; set; }
        public string? Nitelik { get; set; }
        public decimal? Alan { get; set; }
        public List<DroneCoordinate>? Coordinates { get; set; }
    }

    public class DroneJobRequest
    {
        public string? SavedParcelId { get; set; }
        public string? ParcelId { get; set; }
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

    public class DroneCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    private async Task AttachUserHeadersAsync(HttpClient client)
    {
        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var firstName = User?.FindFirstValue("FirstName");
        var lastName = User?.FindFirstValue("LastName");
        var phoneNumber = User?.FindFirstValue("PhoneNumber");
        var email = User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("Email");
        var userType = User?.FindFirstValue("UserTypes");

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null && !string.IsNullOrWhiteSpace(email))
        {
            currentUser = await _userManager.FindByEmailAsync(email);
        }
        if (currentUser == null && !string.IsNullOrWhiteSpace(userId))
        {
            currentUser = await _userManager.FindByIdAsync(userId);
        }

        if (currentUser != null)
        {
            userId ??= currentUser.Id;
            if (string.IsNullOrWhiteSpace(firstName)) firstName = currentUser.FirstName;
            if (string.IsNullOrWhiteSpace(lastName)) lastName = currentUser.LastName;
            if (string.IsNullOrWhiteSpace(phoneNumber)) phoneNumber = currentUser.PhoneNumber;
            if (string.IsNullOrWhiteSpace(email)) email = currentUser.Email;
            if (string.IsNullOrWhiteSpace(userType)) userType = currentUser.UserTypes;
        }

        if (!string.IsNullOrWhiteSpace(userId)) client.DefaultRequestHeaders.Add("x-sevval-user-id", NormalizeHeaderValue(userId));
        if (!string.IsNullOrWhiteSpace(firstName)) client.DefaultRequestHeaders.Add("x-user-name", NormalizeHeaderValue(firstName));
        if (!string.IsNullOrWhiteSpace(lastName)) client.DefaultRequestHeaders.Add("x-user-surname", NormalizeHeaderValue(lastName));
        if (!string.IsNullOrWhiteSpace(phoneNumber)) client.DefaultRequestHeaders.Add("x-user-phone", NormalizeHeaderValue(phoneNumber));
        if (!string.IsNullOrWhiteSpace(email)) client.DefaultRequestHeaders.Add("x-user-email", NormalizeHeaderValue(email));
        if (!string.IsNullOrWhiteSpace(userType)) client.DefaultRequestHeaders.Add("x-user-type", NormalizeHeaderValue(userType));
        client.DefaultRequestHeaders.Add("x-tenant", "sevval");
    }

    private static string NormalizeHeaderValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var cleaned = value.Trim();
        var buffer = new char[cleaned.Length];
        var index = 0;
        foreach (var ch in cleaned)
        {
            if (ch <= 127)
            {
                buffer[index++] = ch;
            }
        }
        return index == 0 ? string.Empty : new string(buffer, 0, index);
    }

    private async Task<IActionResult> ForwardResponseAsync(HttpResponseMessage response, string label, object? emptyPayload = null)
    {
        var raw = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(raw))
        {
            _logger.LogWarning("Drone API empty response for {Label}. Status={Status}", label, response.StatusCode);
            if (emptyPayload != null)
            {
                return StatusCode((int)response.StatusCode, emptyPayload);
            }
            return StatusCode((int)response.StatusCode, new
            {
                success = false,
                message = "Drone API boş cevap döndü.",
                status = (int)response.StatusCode
            });
        }

        try
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize<object>(raw);
            return StatusCode((int)response.StatusCode, payload);
        }
        catch (System.Text.Json.JsonException)
        {
            _logger.LogWarning("Drone API non-JSON response for {Label}: {Body}", label, raw);
            return StatusCode((int)response.StatusCode, new
            {
                success = response.IsSuccessStatusCode,
                message = raw
            });
        }
    }

    private static async Task<string?> ToDataUrlAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var base64 = Convert.ToBase64String(stream.ToArray());
        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        return $"data:{contentType};base64,{base64}";
    }

    private async Task<(string? LogoPath, string? AvatarPath)> ResolveUserMediaPathsAsync(ApplicationUser user)
    {
        var avatarPath = NormalizeMediaPath(user.ProfilePicturePath);
        string? logoPath = null;

        if (user.IsConsultant && !string.IsNullOrWhiteSpace(user.ConsultantCompanyId))
        {
            var companyUser = await _userManager.FindByIdAsync(user.ConsultantCompanyId);
            logoPath = NormalizeMediaPath(companyUser?.ProfilePicturePath);
        }

        if (string.IsNullOrWhiteSpace(logoPath))
        {
            logoPath = NormalizeMediaPath(user.ProfilePicturePath);
        }

        return (logoPath, avatarPath);
    }

    private static string? NormalizeMediaPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        var trimmed = path.Trim();
        return IsPlaceholderPath(trimmed) ? null : trimmed;
    }

    private static string? NormalizePublicUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }
        return path.StartsWith('/') ? path : $"/{path}";
    }

    private static bool IsPlaceholderPath(string path)
    {
        var lowered = path.ToLowerInvariant();
        return lowered.Contains("bosprofifoto") ||
               lowered.Contains("boşprofifoto") ||
               lowered.Contains("kurumsalbosfoto") ||
               lowered.Contains("default-profile") ||
               lowered.Contains("defaultuser");
    }

    private static bool IsHttpUrl(string path)
    {
        return path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetLocalFullPath(string path)
    {
        var relativePath = path.StartsWith('/') ? path[1..] : path;
        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        return Path.Combine(webRoot, relativePath.Replace("\\", "/").TrimStart('/'));
    }

    private static long? GetLocalFileSize(string path)
    {
        var fullPath = GetLocalFullPath(path);
        if (!System.IO.File.Exists(fullPath)) return null;
        return new FileInfo(fullPath).Length;
    }

    private static bool IsMediaTooLarge(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        if (IsHttpUrl(path)) return false;
        var size = GetLocalFileSize(path);
        return size.HasValue && size.Value > MaxUploadBytes;
    }

    private async Task<string?> ToDataUrlFromPathAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (IsHttpUrl(path))
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                using var response = await client.GetAsync(path);
                if (!response.IsSuccessStatusCode) return null;
                var bytes = await response.Content.ReadAsByteArrayAsync();
                if (bytes.Length == 0) return null;
                var responseContentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                return $"data:{responseContentType};base64,{Convert.ToBase64String(bytes)}";
            }
            catch
            {
                return null;
            }
        }

        var fullPath = GetLocalFullPath(path);
        var size = GetLocalFileSize(path);
        if (size.HasValue && size.Value > MaxUploadBytes) return null;
        if (!System.IO.File.Exists(fullPath)) return null;

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytesLocal = await System.IO.File.ReadAllBytesAsync(fullPath);
        if (bytesLocal.Length == 0) return null;
        return $"data:{contentType};base64,{Convert.ToBase64String(bytesLocal)}";
    }
}
