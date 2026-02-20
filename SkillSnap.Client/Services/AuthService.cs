using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SkillSnap.Shared.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SkillSnap.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("api");
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<LoginResult> LoginAsync(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

        if (!response.IsSuccessStatusCode)
        {
            return new LoginResult { Success = false, Message = "Invalid email or password" };
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (result?.Token == null)
        {
            return new LoginResult { Success = false, Message = "No token received" };
        }

        await _localStorage.SetItemAsync("authToken", result.Token);
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);

        return new LoginResult { Success = true };
    }

    public async Task<RegisterResult> RegisterAsync(RegisterDto registerDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return new RegisterResult { Success = false, Message = errorContent };
        }

        return new RegisterResult { Success = true };
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }
}

// Helper classes
public class LoginResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class RegisterResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}