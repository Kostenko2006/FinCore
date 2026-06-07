using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Domain.Entities;

public class BankTransaction
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long AccountId { get; set; }
    public Account? Account { get; set; }
    public long? TransferId { get; set; }
    public Transfer? Transfer { get; set; }

    public TransactionType Type { get; set; }
    public TransactionDirection Direction { get; set; }
    public decimal Amount { get; set; }
    public CurrencyCode Currency { get; set; }
    public string Category { get; set; } = "General";
    public string Description { get; set; } = "";
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    public string? Counterparty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
