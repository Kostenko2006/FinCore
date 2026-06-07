using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinCore.Api.Tests;

public class Section_6_3_AuthenticationAndRolesTests
{
    [Fact(DisplayName = "6.3 Автентифікація: демо-клієнт входить і отримує JWT")]
    public async Task Section_6_3_Authentication_DemoClientCanLoginAndReceiveJwt()
    {
        await using var factory = new FinCoreApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@fincore.local",
            password = "password123"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("token").GetString()));
        Assert.Equal("demo@fincore.local", json.GetProperty("user").GetProperty("email").GetString());
        Assert.Equal("Client", json.GetProperty("user").GetProperty("role").GetString());
    }

    [Fact(DisplayName = "6.3 Ролі: клієнт не має доступу до адміністративного API")]
    public async Task Section_6_3_Roles_ClientCannotOpenAdminUsersEndpoint()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("demo@fincore.local");

        var response = await client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "6.3 Ролі: адміністратор має доступ до списку користувачів")]
    public async Task Section_6_3_Roles_AdminCanOpenAdminUsersEndpoint()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("admin@fincore.local");

        var response = await client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.True(json.GetArrayLength() >= 2);
    }

    [Fact(DisplayName = "6.3 Автентифікація: захищений API відхиляє запит без токена")]
    public async Task Section_6_3_Authentication_ProtectedApiRejectsAnonymousRequest()
    {
        await using var factory = new FinCoreApiFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        var response = await client.GetAsync("/api/accounts");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
