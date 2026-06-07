using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public UserRole Role { get; set; } = UserRole.Client;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Account> Accounts { get; set; } = [];
    public List<BankCard> Cards { get; set; } = [];
    public List<BankTransaction> Transactions { get; set; } = [];
}
