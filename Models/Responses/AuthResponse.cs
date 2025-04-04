using System.Text.Json.Serialization;

namespace GloboClima.Server.Models.Responses;

public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
