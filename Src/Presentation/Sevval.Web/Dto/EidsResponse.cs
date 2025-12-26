using System.Text.Json.Serialization;

namespace sevvalemlak.Dto;

public class EidsResponse
{
    [JsonPropertyName("data")]
    public TasinmazData Data { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("errors")]
    public string[] Errors { get; set; }

    // Başarılı yanıt mı?
    [JsonIgnore]
    public bool IsSuccessful => StatusCode == 200 && Errors == null && Data != null;
}

public class TasinmazData
{
    [JsonPropertyName("ilanSuresi")]
    public DateTime IlanSuresi { get; set; }

    [JsonPropertyName("tasinmazBilgileri")]
    public string TasinmazBilgileri { get; set; }

    [JsonPropertyName("tasinmazId")]
    public decimal TasinmazId { get; set; }
}