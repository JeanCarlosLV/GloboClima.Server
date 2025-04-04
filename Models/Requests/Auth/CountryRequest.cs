namespace GloboClima.Server.Models.Requests.Auth;

// Models/CountryResponse.cs
using System.Text.Json.Serialization;

public class CountryRequest
{
    [JsonPropertyName("name")]
    public CountryName? Name { get; set; }

    [JsonPropertyName("population")]
    public long Population { get; set; }

    [JsonPropertyName("currencies")]
    public Dictionary<string, Currency>? Currencies { get; set; }

    [JsonPropertyName("languages")]
    public Dictionary<string, string>? Languages { get; set; }

    public class CountryName
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }
    }

    public class Currency
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
