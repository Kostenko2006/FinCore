using System.Net;

namespace FinCore.Api.Tests;

public class Section_6_2_SystemStartupAndApiTests
{
    [Fact(DisplayName = "6.2 Запуск системи: Swagger API документація доступна")]
    public async Task Section_6_2_SystemStartup_SwaggerApiDocumentationIsAvailable()
    {
        await using var factory = new FinCoreApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("FinCore Bank API", json);
    }

    [Fact(DisplayName = "6.2 Запуск системи: головна адреса перенаправляє на Swagger")]
    public async Task Section_6_2_SystemStartup_RootUrlRedirectsToSwagger()
    {
        await using var factory = new FinCoreApiFactory();
        var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/swagger", response.Headers.Location?.OriginalString);
    }
}
