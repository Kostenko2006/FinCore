using FinCore.Api.Contracts.Auth;
using FinCore.Api.Contracts.Banking;
using FinCore.Api.Domain.Entities;

namespace FinCore.Api.Services;

public static class BankingMapper
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.Email, user.FullName, user.Role, user.CreatedAt);

    public static AccountResponse ToResponse(this Account account) =>
        new(account.Id, account.AccountNumber, account.Iban, account.Name, account.Type, account.Currency,
            account.Balance, account.Status, account.CreatedAt);

    public static CardResponse ToResponse(this BankCard card) =>
        new(card.Id, card.AccountId, card.Account?.Name ?? "Account", card.CardHolder, card.MaskedNumber,
            card.ExpiryMonth, card.ExpiryYear, card.Type, card.Status, card.DailyLimit, card.CreatedAt);

    public static TransactionResponse ToResponse(this BankTransaction transaction) =>
        new(transaction.Id, transaction.AccountId, transaction.Account?.Name ?? "Account", transaction.Type,
            transaction.Direction, transaction.Amount, transaction.Currency, transaction.Category,
            transaction.Description, transaction.Status, transaction.Counterparty, transaction.CreatedAt);

    public static TransferResponse ToResponse(this Transfer transfer) =>
        new(transfer.Id, transfer.FromAccountId, transfer.ToAccountId, transfer.ExternalIban, transfer.Amount,
            transfer.Currency, transfer.Description, transfer.Status, transfer.CreatedAt);
}
