using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Text.Json;

[ApiController]
[Route("api/tkgm")]
public class TkgmController : ControllerBase
{
    private const string TkgmBaseUrl = "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3.1/api";
    private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new();
    private static readonly TimeSpan ListCacheTtl = TimeSpan.FromHours(6);
    private static readonly TimeSpan ParcelCacheTtl = TimeSpan.FromMinutes(5);

    [HttpGet("parsel-geo")]
    public async Task<IActionResult> GetParselGeo([FromQuery] string mahalleId, [FromQuery] string ada, [FromQuery] string parsel)
    {
        if (string.IsNullOrWhiteSpace(mahalleId) || string.IsNullOrWhiteSpace(ada) || string.IsNullOrWhiteSpace(parsel))
        {
            return BadRequest(new { success = false, message = "mahalleId, ada ve parsel zorunludur." });
        }

        var url = $"{TkgmBaseUrl}/parsel/{Uri.EscapeDataString(mahalleId)}/{Uri.EscapeDataString(ada)}/{Uri.EscapeDataString(parsel)}";
        return await ProxyJson(url, ParcelCacheTtl);
    }

    [HttpGet("il")]
    public Task<IActionResult> GetIller()
    {
        return ProxyJson($"{TkgmBaseUrl}/idariYapi/ilListe", ListCacheTtl);
    }

    [HttpGet("ilce/{ilId}")]
    public Task<IActionResult> GetIlceler(string ilId)
    {
        return ProxyJson($"{TkgmBaseUrl}/idariYapi/ilceListe/{Uri.EscapeDataString(ilId)}", ListCacheTtl);
    }

    [HttpGet("mahalle/{ilceId}")]
    public Task<IActionResult> GetMahalleler(string ilceId)
    {
        return ProxyJson($"{TkgmBaseUrl}/idariYapi/mahalleListe/{Uri.EscapeDataString(ilceId)}", ListCacheTtl);
    }

    private async Task<IActionResult> ProxyJson(string url, TimeSpan ttl)
    {
        if (TryGetCache(url, ttl, out var cached))
        {
            return new ContentResult { Content = cached, ContentType = "application/json" };
        }

        try
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.ParseAdd("application/json");
            request.Headers.Add("Origin", "https://parselsorgu.tkgm.gov.tr");
            request.Headers.Referrer = new Uri("https://parselsorgu.tkgm.gov.tr/");

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var payload = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, payload);
            }

            Cache[url] = new CacheEntry(DateTimeOffset.UtcNow, payload);
            return new ContentResult { Content = payload, ContentType = "application/json" };
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { success = false, message = "TKGM istegi basarisiz.", detail = ex.Message }) { StatusCode = 500 };
        }
    }

    private static bool TryGetCache(string key, TimeSpan ttl, out string payload)
    {
        payload = string.Empty;
        if (!Cache.TryGetValue(key, out var entry))
        {
            return false;
        }

        if (DateTimeOffset.UtcNow - entry.CreatedAt > ttl)
        {
            Cache.TryRemove(key, out _);
            return false;
        }

        payload = entry.Payload;
        return true;
    }

    private readonly record struct CacheEntry(DateTimeOffset CreatedAt, string Payload);
}
