using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Domain.Entities;

public class BankCard
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long AccountId { get; set; }
    public Account? Account { get; set; }

    public required string CardHolder { get; set; }
    public required string MaskedNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public CardType Type { get; set; }
    public CardStatus Status { get; set; } = CardStatus.Active;
    public decimal DailyLimit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
