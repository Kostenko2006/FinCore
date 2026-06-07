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
public class TransactionsController(FinCoreDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TransactionResponse>>> GetTransactions(
        [FromQuery] long? accountId,
        [FromQuery] TransactionDirection? direction,
        [FromQuery] TransactionStatus? status)
    {
        var userId = User.GetUserId();
        var query = db.Transactions.Include(x => x.Account).Where(x => x.UserId == userId);

        if (accountId.HasValue)
        {
            query = query.Where(x => x.AccountId == accountId.Value);
        }

        if (direction.HasValue)
        {
            query = query.Where(x => x.Direction == direction.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var transactions = await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(100)
            .Select(x => x.ToResponse())
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TransactionResponse>> GetTransaction(long id)
    {
        var userId = User.GetUserId();
        var transaction = await db.Transactions
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        return transaction is null ? NotFound(new { message = "Transaction not found." }) : Ok(transaction.ToResponse());
    }

    [HttpGet("stats")]
    public async Task<ActionResult<TransactionStatsResponse>> GetStats()
    {
        var userId = User.GetUserId();
        var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var transactions = await db.Transactions
            .Where(x => x.UserId == userId && x.CreatedAt >= start && x.Status == TransactionStatus.Completed)
            .ToListAsync();

        var expenses = transactions.Where(x => x.Direction == TransactionDirection.Expense).ToList();

        return Ok(new TransactionStatsResponse(
            Income: transactions.Where(x => x.Direction == TransactionDirection.Income).Sum(x => x.Amount),
            Expense: expenses.Sum(x => x.Amount),
            CompletedCount: transactions.Count,
            CategoryExpenses: expenses
                .GroupBy(x => x.Category)
                .Select(x => new CategoryExpenseResponse(x.Key, x.Sum(t => t.Amount)))
                .OrderByDescending(x => x.Amount)
                .ToList()));
    }
}
