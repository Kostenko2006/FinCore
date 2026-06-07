using System.Net;
using System.Net.Http.Json;

namespace FinCore.Api.Tests;

public class Section_6_4_CoreFunctionsTests
{
    [Fact(DisplayName = "6.4 Основні функції: клієнт бачить dashboard, рахунки, картки та операції")]
    public async Task Section_6_4_CoreFunctions_ClientCanReadDashboardAccountsCardsAndTransactions()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("demo@fincore.local");

        var dashboard = await client.GetAsync("/api/dashboard/summary");
        var accounts = await client.GetAsync("/api/accounts");
        var cards = await client.GetAsync("/api/cards");
        var transactions = await client.GetAsync("/api/transactions");

        Assert.Equal(HttpStatusCode.OK, dashboard.StatusCode);
        Assert.Equal(HttpStatusCode.OK, accounts.StatusCode);
        Assert.Equal(HttpStatusCode.OK, cards.StatusCode);
        Assert.Equal(HttpStatusCode.OK, transactions.StatusCode);

        Assert.True((await accounts.ReadJsonAsync()).GetArrayLength() >= 3);
        Assert.True((await cards.ReadJsonAsync()).GetArrayLength() >= 2);
        Assert.True((await transactions.ReadJsonAsync()).GetArrayLength() >= 5);
    }

    [Fact(DisplayName = "6.4 Основні функції: клієнт створює новий рахунок")]
    public async Task Section_6_4_CoreFunctions_ClientCanCreateAccount()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("demo@fincore.local");

        var response = await client.PostAsJsonAsync("/api/accounts", new
        {
            name = "Test UAH Account",
            type = "Checking",
            currency = "UAH"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal("Test UAH Account", json.GetProperty("name").GetString());
        Assert.Equal("UAH", json.GetProperty("currency").GetString());
        Assert.Equal(0, json.GetProperty("balance").GetDecimal());
    }

    [Fact(DisplayName = "6.4 Основні функції: клієнт випускає картку до свого рахунку")]
    public async Task Section_6_4_CoreFunctions_ClientCanCreateCardForOwnAccount()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("demo@fincore.local");
        var accounts = await (await client.GetAsync("/api/accounts")).ReadJsonAsync();
        var accountId = accounts[0].GetProperty("id").GetInt64();

        var response = await client.PostAsJsonAsync("/api/cards", new
        {
            accountId,
            type = "Debit",
            dailyLimit = 12000
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal(accountId, json.GetProperty("accountId").GetInt64());
        Assert.Contains("****", json.GetProperty("maskedNumber").GetString());
        Assert.Equal("Active", json.GetProperty("status").GetString());
    }
}
