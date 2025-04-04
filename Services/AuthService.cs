namespace GloboClima.Server.Services;

using System.Net.Http.Json;
using GloboClima.Server.Models.Requests;
using GloboClima.Server.Models.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;


public class AuthService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthenticationStateProvider _authProvider;

    public AuthService(
        HttpClient http,
        IConfiguration config,
        ProtectedLocalStorage localStorage,
        AuthenticationStateProvider authProvider)
    {
        _http = http;
        _config = config;
        _localStorage = localStorage;
        _authProvider = authProvider;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result == null || string.IsNullOrEmpty(result.Token))
                return false;

            await _localStorage.SetAsync("authToken", result.Token);
            ((CustomAuthStateProvider)_authProvider).NotifyUserAuthentication(result.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public async Task<bool> RegisterAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync(
            $"{_config["ApiBaseUrl"]}/api/auth/register",
            new { email, password });

        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<string>("authToken");
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.DeleteAsync("authToken");
        ((CustomAuthStateProvider)_authProvider).NotifyUserLogout();
    }
}
