using System.Net;
using System.Net.Http.Json;

namespace FinCore.Api.Tests;

public class Section_6_5_TransfersTests
{
    [Fact(DisplayName = "6.5 Перекази: внутрішній переказ змінює баланси і створює операції")]
    public async Task Section_6_5_Transfers_InternalTransferChangesBalancesAndCreatesTransactions()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("demo@fincore.local");

        var targetResponse = await client.PostAsJsonAsync("/api/accounts", new
        {
            name = "Transfer Target UAH",
            type = "Checking",
            currency = "UAH"
        });
        targetResponse.EnsureSuccessStatusCode();
        var target = await targetResponse.ReadJsonAsync();
        var targetAccountId = target.GetProperty("id").GetInt64();

        var accountsBefore = await (await client.GetAsync("/api/accounts")).ReadJsonAsync();
        var source = accountsBefore.EnumerateArray()
            .First(account => account.GetProperty("currency").GetString() == "UAH" &&
                              account.GetProperty("balance").GetDecimal() > 500);
        var sourceAccountId = source.GetProperty("id").GetInt64();
        var sourceBalanceBefore = source.GetProperty("balance").GetDecimal();

        var transferResponse = await client.PostAsJsonAsync("/api/transfers", new
        {
            fromAccountId = sourceAccountId,
            toAccountId = targetAccountId,
            externalIban = (string?)null,
            amount = 500m,
            currency = "UAH",
            description = "Test internal transfer"
        });

        Assert.Equal(HttpStatusCode.Created, transferResponse.StatusCode);
        var transfer = await transferResponse.ReadJsonAsync();
        Assert.Equal("Completed", transfer.GetProperty("status").GetString());

        var accountsAfter = await (await client.GetAsync("/api/accounts")).ReadJsonAsync();
        var sourceAfter = accountsAfter.EnumerateArray().First(account => account.GetProperty("id").GetInt64() == sourceAccountId);
        var targetAfter = accountsAfter.EnumerateArray().First(account => account.GetProperty("id").GetInt64() == targetAccountId);

        Assert.Equal(sourceBalanceBefore - 500m, sourceAfter.GetProperty("balance").GetDecimal());
        Assert.Equal(500m, targetAfter.GetProperty("balance").GetDecimal());

        var transactions = await (await client.GetAsync("/api/transactions")).ReadJsonAsync();
        var transferOperations = transactions.EnumerateArray()
            .Count(item => item.GetProperty("description").GetString() == "Test internal transfer");
        Assert.Equal(2, transferOperations);
    }

    [Fact(DisplayName = "6.5 Перекази: система відхиляє переказ при недостатньому балансі")]
    public async Task Section_6_5_Transfers_RejectsTransferWhenBalanceIsInsufficient()
    {
        await using var factory = new FinCoreApiFactory();
        var client = await factory.CreateAuthorizedClientAsync("demo@fincore.local");
        var accounts = await (await client.GetAsync("/api/accounts")).ReadJsonAsync();
        var source = accounts.EnumerateArray().First(account => account.GetProperty("currency").GetString() == "UAH");

        var response = await client.PostAsJsonAsync("/api/transfers", new
        {
            fromAccountId = source.GetProperty("id").GetInt64(),
            toAccountId = (long?)null,
            externalIban = "UA993000010000999999999999999",
            amount = 999_999_999m,
            currency = "UAH",
            description = "Too large external transfer"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal("Insufficient funds.", json.GetProperty("message").GetString());
    }
}
