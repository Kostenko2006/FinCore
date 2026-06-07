using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Domain.Entities;

public class Account
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }

    public required string AccountNumber { get; set; }
    public required string Iban { get; set; }
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public CurrencyCode Currency { get; set; }
    public decimal Balance { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BankCard> Cards { get; set; } = [];
    public List<BankTransaction> Transactions { get; set; } = [];
}
