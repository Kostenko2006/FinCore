using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinCore.Api.Tests;

public static class ApiTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> LoginAndGetTokenAsync(HttpClient client, string email, string password = "password123")
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        return json.GetProperty("token").GetString()!;
    }

    public static async Task<HttpClient> CreateAuthorizedClientAsync(this FinCoreApiFactory factory, string email)
    {
        var client = factory.CreateClient();
        var token = await LoginAndGetTokenAsync(client, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<JsonElement> ReadJsonAsync(this HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        return json;
    }
}
