using System.ComponentModel.DataAnnotations;
using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Contracts.Banking;

public record AccountResponse(
    long Id,
    string AccountNumber,
    string Iban,
    string Name,
    AccountType Type,
    CurrencyCode Currency,
    decimal Balance,
    AccountStatus Status,
    DateTime CreatedAt);

public record CreateAccountRequest(
    [Required, MaxLength(120)] string Name,
    AccountType Type,
    CurrencyCode Currency);

public record CardResponse(
    long Id,
    long AccountId,
    string AccountName,
    string CardHolder,
    string MaskedNumber,
    int ExpiryMonth,
    int ExpiryYear,
    CardType Type,
    CardStatus Status,
    decimal DailyLimit,
    DateTime CreatedAt);

public record CreateCardRequest(
    [Required] long AccountId,
    CardType Type,
    [Range(1, 1_000_000)] decimal DailyLimit);

public record TransactionResponse(
    long Id,
    long AccountId,
    string AccountName,
    TransactionType Type,
    TransactionDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    string Category,
    string Description,
    TransactionStatus Status,
    string? Counterparty,
    DateTime CreatedAt);

public record TransferRequest(
    [Required] long FromAccountId,
    long? ToAccountId,
    [MaxLength(34)] string? ExternalIban,
    [Range(0.01, 1_000_000_000)] decimal Amount,
    [Required] CurrencyCode Currency,
    [MaxLength(280)] string? Description);

public record TransferResponse(
    long Id,
    long FromAccountId,
    long? ToAccountId,
    string? ExternalIban,
    decimal Amount,
    CurrencyCode Currency,
    string Description,
    TransferStatus Status,
    DateTime CreatedAt);

public record CurrencyBalanceResponse(CurrencyCode Currency, decimal Balance);

public record MonthlyFlowResponse(string Month, decimal Income, decimal Expense);

public record CategoryExpenseResponse(string Category, decimal Amount);

public record DashboardSummaryResponse(
    IReadOnlyCollection<CurrencyBalanceResponse> Balances,
    decimal TotalUahEquivalent,
    int ActiveAccounts,
    int ActiveCards,
    decimal MonthlyIncome,
    decimal MonthlyExpense,
    IReadOnlyCollection<TransactionResponse> RecentTransactions,
    IReadOnlyCollection<MonthlyFlowResponse> MonthlyFlow,
    IReadOnlyCollection<CategoryExpenseResponse> CategoryExpenses);

public record TransactionStatsResponse(
    decimal Income,
    decimal Expense,
    int CompletedCount,
    IReadOnlyCollection<CategoryExpenseResponse> CategoryExpenses);
