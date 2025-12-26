using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;

namespace Sevval.Web.Controllers;

[ApiController]
[Route("api/tkgm")]
public class TkgmProxyController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly CookieContainer _cookieContainer = new();
    private static readonly Uri BaseUri = new("https://parselsorgu.tkgm.gov.tr");
    private const string ApiBase = "https://parselsorgu.tkgm.gov.tr/api";
    private const string CbsApiBase = "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3.1/api";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114 Safari/537.36";

    public TkgmProxyController()
    {
        var handler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
        };
        _client = new HttpClient(handler)
        {
            Timeout = DefaultTimeout
        };
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.AcceptLanguage.Clear();
        _client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("tr-TR"));
        _client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US", 0.8));
        _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", DefaultUserAgent);
    }

    [HttpGet("il")]
    public async Task<IActionResult> GetIller(CancellationToken cancellationToken)
    {
        // Önce idariYapi/ilListe (kod/id numeric) dene, başarısızsa makSIdariYapi/illiste yedeği kullan
        var primary = await ProxyIdariAsync($"{CbsApiBase}/idariYapi/ilListe", cancellationToken, rawPassthroughOnError: false);
        if (primary is ContentResult cr && cr.StatusCode is >= 200 and < 300)
        {
            return primary;
        }
        return await ProxyIdariAsync($"{CbsApiBase}/maksIdariYapi/illiste", cancellationToken);
    }

    [HttpGet("ilce/{ilKodu}")]
    public async Task<IActionResult> GetIlceler(string ilKodu, CancellationToken cancellationToken)
    {
        // ilKodu burada ilId olarak kullanılıyor
        var cleaned = CleanId(ilKodu);
        return await ProxyIdariAsync($"{CbsApiBase}/idariYapi/ilceListe/{cleaned}", cancellationToken);
    }

    [HttpGet("mahalle/{ilKodu}/{ilceKodu}")]
    public async Task<IActionResult> GetMahalleler(string ilKodu, string ilceKodu, CancellationToken cancellationToken)
    {
        var cleaned = CleanId(ilceKodu);
        return await ProxyIdariAsync($"{CbsApiBase}/idariYapi/mahalleListe/{cleaned}", cancellationToken);
    }

    public record TkgmParselRequest(string IlKodu, string IlceKodu, string MahalleKodu, string? Ada, string? Parsel);

    [HttpGet("parsel-geo")]
    public async Task<IActionResult> GetParselGeo([FromQuery] int mahalleId, [FromQuery] string ada, [FromQuery] string parsel, CancellationToken cancellationToken)
    {
        if (mahalleId <= 0 || string.IsNullOrWhiteSpace(ada) || string.IsNullOrWhiteSpace(parsel))
        {
            return BadRequest(new { message = "mahalleId, ada ve parsel zorunludur" });
        }

        await EnsureSessionAsync(cancellationToken);
        var url = $"{CbsApiBase}/parsel/{mahalleId}/{ada}/{parsel}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        PrepareRequestHeaders(req);
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        req.Headers.TryAddWithoutValidation("Referer", "https://parselsorgu.tkgm.gov.tr/");
        req.Headers.TryAddWithoutValidation("Origin", "https://parselsorgu.tkgm.gov.tr");

        using var response = await _client.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = body,
            ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json"
        };
    }

    [HttpPost("parsel")]
    public async Task<IActionResult> PostParsel([FromBody] TkgmParselRequest request, CancellationToken cancellationToken)
    {
        await EnsureSessionAsync(cancellationToken);

        var payload = new
        {
            ilKodu = request.IlKodu,
            ilceKodu = request.IlceKodu,
            mahalleKodu = request.MahalleKodu,
            ada = string.IsNullOrWhiteSpace(request.Ada) ? null : request.Ada,
            parsel = string.IsNullOrWhiteSpace(request.Parsel) ? null : request.Parsel,
            token = (string?)null
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, $"{ApiBase}/parsel/ara")
        {
            Content = JsonContent.Create(payload)
        };
        PrepareRequestHeaders(req);

        using var response = await _client.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = body,
            ContentType = contentType
        };
    }

    private async Task<IActionResult> ProxyGetAsync(string url, CancellationToken cancellationToken)
    {
        await EnsureSessionAsync(cancellationToken);

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        PrepareRequestHeaders(req);

        using var response = await _client.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = body,
            ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json"
        };
    }

    private async Task<IActionResult> ProxyIdariAsync(string url, CancellationToken cancellationToken, bool rawPassthroughOnError = true)
    {
        await EnsureSessionAsync(cancellationToken);

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        PrepareRequestHeaders(req);
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _client.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        // Eğer HTML dönerse direkt iletelim
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
        if (contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase) || body.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
        {
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = body,
                ContentType = contentType
            };
        }

        try
        {
            var list = NormalizeIdari(body);
            var json = JsonSerializer.Serialize(list);
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = json,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Idari parse error: {ex.Message}");
            if (!rawPassthroughOnError)
            {
                return StatusCode((int)response.StatusCode, new { message = "idari parse failed", detail = ex.Message });
            }
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = body,
                ContentType = contentType
            };
        }
    }

    private static List<IdariResponse> NormalizeIdari(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // GeoJSON feature listesi mi?
        if (root.TryGetProperty("features", out var features) && features.ValueKind == JsonValueKind.Array)
        {
            return features.EnumerateArray()
                .Select(f =>
                {
                    var props = f.TryGetProperty("properties", out var p) ? p : default;
                    return new IdariResponse
                    {
                        Id = GetProp(props, "id", "ID", "kod", "Kod", "ilId", "ilceId", "mahalleId"),
                        Kod = GetProp(props, "ilKodu", "ilceKodu", "mahalleKodu", "kod", "Kod"),
                        Ad = GetProp(props, "text", "ad", "name", "Il", "IlAdi", "Ilce", "IlceAdi", "Mahalle", "MahalleAdi")
                    };
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Id) || !string.IsNullOrWhiteSpace(x.Kod) || !string.IsNullOrWhiteSpace(x.Ad))
                .ToList();
        }

        // Dizi ise doğrudan mapleyelim
        if (root.ValueKind == JsonValueKind.Array)
        {
            return root.EnumerateArray()
                .Select(p => new IdariResponse
                {
                    Id = GetProp(p, "id", "ID", "kod", "Kod", "ilId", "ilceId", "mahalleId"),
                    Kod = GetProp(p, "ilKodu", "ilceKodu", "mahalleKodu", "kod", "Kod"),
                    Ad = GetProp(p, "ilAdi", "ilceAdi", "mahalleAdi", "ad", "name", "text", "Il", "Ilce", "Mahalle")
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Id) || !string.IsNullOrWhiteSpace(x.Kod) || !string.IsNullOrWhiteSpace(x.Ad))
                .ToList();
        }

        return new();
    }

    private static string CleanId(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return value.Trim().Trim('{', '}');
    }

    private static string GetProp(JsonElement element, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (element.ValueKind == JsonValueKind.Undefined) break;
            if (element.TryGetProperty(key, out var val))
            {
                if (val.ValueKind == JsonValueKind.String) return val.GetString() ?? string.Empty;
                if (val.ValueKind == JsonValueKind.Number) return val.GetRawText();
            }
        }
        return string.Empty;
    }

    private record IdariResponse
    {
        public string? Id { get; set; }
        public string? Kod { get; set; }
        public string? Ad { get; set; }
    }

    private async Task EnsureSessionAsync(CancellationToken cancellationToken)
    {
        if (_cookieContainer.GetCookies(BaseUri).Count > 0)
        {
            return;
        }

        using var req = new HttpRequestMessage(HttpMethod.Get, BaseUri);
        PrepareRequestHeaders(req);
        req.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        using var response = await _client.SendAsync(req, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private void PrepareRequestHeaders(HttpRequestMessage req)
    {
        req.Headers.Referrer = BaseUri;
        req.Headers.TryAddWithoutValidation("Origin", BaseUri.ToString().TrimEnd('/'));
        req.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
        req.Headers.TryAddWithoutValidation("Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
        req.Headers.TryAddWithoutValidation("User-Agent", DefaultUserAgent);
        req.Headers.Remove("Authorization"); // Bearer undefined istemiyoruz
    }
}
