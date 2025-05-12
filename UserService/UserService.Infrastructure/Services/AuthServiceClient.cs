using System.Net.Http;
using System.Text.Json;
using UserService.Application.Events;

namespace UserService.Infrastructure.Services;

public class AuthServiceClient
{
    private readonly HttpClient _http;

    public AuthServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<string>> GetAllEmailsAsync()
    {
        var response = await _http.GetAsync("http://auth-service:80/api/auth/emails");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var emails = JsonSerializer.Deserialize<List<string>>(json) ?? new();

        return emails;
    }

    public async Task<UserRegisteredEvent?> GetUserDetailAsync(string email)
    {
        var response = await _http.GetAsync($"http://auth-service:80/api/auth/detail/{email}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserRegisteredEvent>(json);
    }

}
