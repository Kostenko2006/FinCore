using FinCore.Api.Contracts.Banking;
using FinCore.Api.Data;
using FinCore.Api.Domain.Enums;
using FinCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController(FinCoreDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
    {
        var userId = User.GetUserId();
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var flowStart = DateTime.UtcNow.AddMonths(-5);

        var accounts = await db.Accounts
            .Where(x => x.UserId == userId)
            .ToListAsync();

        var cardsCount = await db.Cards.CountAsync(x => x.UserId == userId && x.Status == CardStatus.Active);

        var recent = await db.Transactions
            .Include(x => x.Account)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(8)
            .Select(x => x.ToResponse())
            .ToListAsync();

        var monthTransactions = await db.Transactions
            .Where(x => x.UserId == userId && x.CreatedAt >= monthStart && x.Status == TransactionStatus.Completed)
            .ToListAsync();

        var flowTransactions = await db.Transactions
            .Where(x => x.UserId == userId && x.CreatedAt >= flowStart && x.Status == TransactionStatus.Completed)
            .ToListAsync();

        var balances = accounts
            .GroupBy(x => x.Currency)
            .Select(x => new CurrencyBalanceResponse(x.Key, x.Sum(a => a.Balance)))
            .OrderBy(x => x.Currency)
            .ToList();

        var monthlyFlow = flowTransactions
            .GroupBy(x => x.CreatedAt.ToString("yyyy-MM"))
            .Select(x => new MonthlyFlowResponse(
                x.Key,
                x.Where(t => t.Direction == TransactionDirection.Income).Sum(t => t.Amount),
                x.Where(t => t.Direction == TransactionDirection.Expense).Sum(t => t.Amount)))
            .OrderBy(x => x.Month)
            .ToList();

        var expenses = monthTransactions.Where(x => x.Direction == TransactionDirection.Expense).ToList();

        return Ok(new DashboardSummaryResponse(
            Balances: balances,
            TotalUahEquivalent: accounts.Sum(x => ToUah(x.Balance, x.Currency)),
            ActiveAccounts: accounts.Count(x => x.Status == AccountStatus.Active),
            ActiveCards: cardsCount,
            MonthlyIncome: monthTransactions.Where(x => x.Direction == TransactionDirection.Income).Sum(x => x.Amount),
            MonthlyExpense: expenses.Sum(x => x.Amount),
            RecentTransactions: recent,
            MonthlyFlow: monthlyFlow,
            CategoryExpenses: expenses
                .GroupBy(x => x.Category)
                .Select(x => new CategoryExpenseResponse(x.Key, x.Sum(t => t.Amount)))
                .OrderByDescending(x => x.Amount)
                .ToList()));
    }

    private static decimal ToUah(decimal amount, CurrencyCode currency) =>
        currency switch
        {
            CurrencyCode.USD => amount * 39.5m,
            CurrencyCode.EUR => amount * 42.2m,
            _ => amount
        };
}
