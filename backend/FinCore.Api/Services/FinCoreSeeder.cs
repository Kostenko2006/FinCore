using FinCore.Api.Data;
using FinCore.Api.Domain.Entities;
using FinCore.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Services;

public class FinCoreSeeder(FinCoreDbContext db)
{
    public const string DemoEmail = "demo@fincore.local";
    public const string DemoPassword = "password123";

    public async Task SeedAsync()
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var demo = new User
        {
            Email = DemoEmail,
            FullName = "Demo Client",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
            Role = UserRole.Client
        };

        var admin = new User
        {
            Email = "admin@fincore.local",
            FullName = "FinCore Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
            Role = UserRole.Admin
        };

        db.Users.AddRange(demo, admin);
        await db.SaveChangesAsync();

        var main = CreateAccount(demo.Id, "Everyday UAH", AccountType.Checking, CurrencyCode.UAH, 84250.75m);
        var savings = CreateAccount(demo.Id, "Savings USD", AccountType.Savings, CurrencyCode.USD, 3200.00m);
        var euro = CreateAccount(demo.Id, "Travel EUR", AccountType.Savings, CurrencyCode.EUR, 1450.25m);

        db.Accounts.AddRange(main, savings, euro);
        await db.SaveChangesAsync();

        db.Cards.AddRange(
            CreateCard(demo.Id, main.Id, demo.FullName, "4829", CardType.Debit, 35000m),
            CreateCard(demo.Id, savings.Id, demo.FullName, "1944", CardType.Debit, 1500m));

        db.Transactions.AddRange(
            Tx(demo.Id, main.Id, TransactionType.Deposit, TransactionDirection.Income, 55000m, CurrencyCode.UAH, "Salary", "Monthly salary", "Unimates Money", -27),
            Tx(demo.Id, main.Id, TransactionType.CardPurchase, TransactionDirection.Expense, 1260.40m, CurrencyCode.UAH, "Groceries", "Supermarket payment", "Silpo", -24),
            Tx(demo.Id, main.Id, TransactionType.Payment, TransactionDirection.Expense, 2180m, CurrencyCode.UAH, "Utilities", "Electricity and water bill", "Kyiv Utilities", -20),
            Tx(demo.Id, main.Id, TransactionType.Transfer, TransactionDirection.Expense, 10000m, CurrencyCode.UAH, "Savings", "Transfer to USD savings", "Own account", -16),
            Tx(demo.Id, savings.Id, TransactionType.Transfer, TransactionDirection.Income, 270m, CurrencyCode.USD, "Savings", "Incoming own transfer", "Everyday UAH", -16),
            Tx(demo.Id, euro.Id, TransactionType.Deposit, TransactionDirection.Income, 450m, CurrencyCode.EUR, "Top up", "Travel budget top up", "Cash desk", -12),
            Tx(demo.Id, main.Id, TransactionType.CardPurchase, TransactionDirection.Expense, 899m, CurrencyCode.UAH, "Transport", "Railway tickets", "Ukrzaliznytsia", -8),
            Tx(demo.Id, main.Id, TransactionType.CardPurchase, TransactionDirection.Expense, 4299m, CurrencyCode.UAH, "Electronics", "Headphones purchase", "Rozetka", -6),
            Tx(demo.Id, main.Id, TransactionType.Payment, TransactionDirection.Expense, 750m, CurrencyCode.UAH, "Mobile", "Mobile plan", "Kyivstar", -2));

        await db.SaveChangesAsync();
    }

    private static Account CreateAccount(long userId, string name, AccountType type, CurrencyCode currency, decimal balance)
    {
        var suffix = Random.Shared.NextInt64(10_000_000_000, 99_999_999_999);
        return new Account
        {
            UserId = userId,
            Name = name,
            Type = type,
            Currency = currency,
            Balance = balance,
            AccountNumber = $"2600{suffix}",
            Iban = $"UA{Random.Shared.Next(10, 99)}3000010000{suffix}"
        };
    }

    private static BankCard CreateCard(long userId, long accountId, string holder, string last4, CardType type, decimal limit) =>
        new()
        {
            UserId = userId,
            AccountId = accountId,
            CardHolder = holder.ToUpperInvariant(),
            MaskedNumber = $"5168 **** **** {last4}",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.UtcNow.Year + 4,
            Type = type,
            DailyLimit = limit
        };

    private static BankTransaction Tx(long userId, long accountId, TransactionType type, TransactionDirection direction,
        decimal amount, CurrencyCode currency, string category, string description, string counterparty, int daysAgo) =>
        new()
        {
            UserId = userId,
            AccountId = accountId,
            Type = type,
            Direction = direction,
            Amount = amount,
            Currency = currency,
            Category = category,
            Description = description,
            Counterparty = counterparty,
            CreatedAt = DateTime.UtcNow.AddDays(daysAgo)
        };
}
