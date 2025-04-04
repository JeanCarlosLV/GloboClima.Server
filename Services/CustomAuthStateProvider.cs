namespace GloboClima.Server.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;

    public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var tokenResult = await _localStorage.GetAsync<string>("authToken");

            if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "authenticated"),
                    new Claim("access_token", tokenResult.Value)
                }, "apiauth");

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "authenticated"),
            new Claim("access_token", token)
        }, "apiauth");

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }
}
