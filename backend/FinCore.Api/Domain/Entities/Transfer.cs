using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Domain.Entities;

public class Transfer
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long FromAccountId { get; set; }
    public Account? FromAccount { get; set; }
    public long? ToAccountId { get; set; }
    public Account? ToAccount { get; set; }

    public string? ExternalIban { get; set; }
    public decimal Amount { get; set; }
    public CurrencyCode Currency { get; set; }
    public string Description { get; set; } = "";
    public TransferStatus Status { get; set; } = TransferStatus.Completed;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BankTransaction> Transactions { get; set; } = [];
}
