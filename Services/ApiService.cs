using GloboClima.Server.Models.Requests;
using GloboClima.Server.Models.Requests.Auth;
using GloboClima.Server.Pages;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GloboClima.Server.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly AuthenticationStateProvider _authProvider;

    public ApiService(HttpClient http, IConfiguration config, AuthenticationStateProvider authProvider)
    {
        _http = http;
        _config = config;
        _authProvider = authProvider;
        _http.BaseAddress = new Uri(_config["ApiBaseUrl"]!);
    }
    public async Task<WeatherRequest?> GetWeatherAsync(string city)
    {
        return await _http.GetFromJsonAsync<WeatherRequest>($"/api/weather/{city}");
    }

    public async Task<List<CountryRequest>?> GetCountryAsync(string name)
    {
        try
        {
            var response = await _http.GetAsync($"/api/countries/{name}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<CountryRequest>();

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CountryRequest>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro na API: {ex.StatusCode} - {ex.Message}");
            throw new ApplicationException("Serviço indisponível. Tente novamente mais tarde.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado: {ex}");
            throw new ApplicationException("Erro ao processar a resposta.");
        }
    }

    public async Task<List<FavoriteRequest>> GetFavoritesAsync()
    {
        var response = await _http.GetAsync("/api/favorites");

        if (response.StatusCode == HttpStatusCode.NoContent)
            return new List<FavoriteRequest>();

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FavoriteRequest>>() ?? new List<FavoriteRequest>();
    }

    public async Task AddFavoriteAsync(string type, string name)
    {
        var request = new { Type = type.ToLower(), Name = name };
        var response = await _http.PostAsJsonAsync("/api/favorites", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteFavoriteAsync(string favoriteId)
    {
        var response = await _http.DeleteAsync($"/api/favorites/{favoriteId}");
        response.EnsureSuccessStatusCode();
    }

    private async Task AddAuthHeader()
    {
        var authState = await _authProvider.GetAuthenticationStateAsync();
        var token = authState.User.FindFirst("access_token")?.Value;

        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}