using FinCore.Api.Contracts.Banking;
using FinCore.Api.Data;
using FinCore.Api.Domain.Entities;
using FinCore.Api.Domain.Enums;
using FinCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TransfersController(FinCoreDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TransferResponse>> CreateTransfer(TransferRequest request)
    {
        var userId = User.GetUserId();

        if (request.ToAccountId is null && string.IsNullOrWhiteSpace(request.ExternalIban))
        {
            return BadRequest(new { message = "Provide target account or external IBAN." });
        }

        await using var tx = await db.Database.BeginTransactionAsync();

        var fromAccount = await db.Accounts.FirstOrDefaultAsync(x => x.Id == request.FromAccountId && x.UserId == userId);
        if (fromAccount is null)
        {
            return NotFound(new { message = "Source account not found." });
        }

        if (fromAccount.Status != AccountStatus.Active)
        {
            return BadRequest(new { message = "Source account is not active." });
        }

        if (fromAccount.Currency != request.Currency)
        {
            return BadRequest(new { message = "Transfer currency must match source account currency." });
        }

        if (fromAccount.Balance < request.Amount)
        {
            return BadRequest(new { message = "Insufficient funds." });
        }

        Account? toAccount = null;
        if (request.ToAccountId.HasValue)
        {
            toAccount = await db.Accounts.FirstOrDefaultAsync(x => x.Id == request.ToAccountId && x.UserId == userId);
            if (toAccount is null)
            {
                return NotFound(new { message = "Target account not found." });
            }

            if (toAccount.Currency != request.Currency)
            {
                return BadRequest(new { message = "Target account currency must match transfer currency." });
            }
        }

        var description = string.IsNullOrWhiteSpace(request.Description) ? "Transfer" : request.Description.Trim();
        var transfer = new Transfer
        {
            UserId = userId,
            FromAccountId = fromAccount.Id,
            ToAccountId = toAccount?.Id,
            ExternalIban = request.ExternalIban?.Trim(),
            Amount = request.Amount,
            Currency = request.Currency,
            Description = description,
            Status = TransferStatus.Completed
        };

        fromAccount.Balance -= request.Amount;
        if (toAccount is not null)
        {
            toAccount.Balance += request.Amount;
        }

        db.Transfers.Add(transfer);
        await db.SaveChangesAsync();

        db.Transactions.Add(new BankTransaction
        {
            UserId = userId,
            AccountId = fromAccount.Id,
            TransferId = transfer.Id,
            Type = TransactionType.Transfer,
            Direction = TransactionDirection.Expense,
            Amount = request.Amount,
            Currency = request.Currency,
            Category = "Transfer",
            Description = description,
            Counterparty = toAccount?.Name ?? request.ExternalIban,
            Status = TransactionStatus.Completed
        });

        if (toAccount is not null)
        {
            db.Transactions.Add(new BankTransaction
            {
                UserId = userId,
                AccountId = toAccount.Id,
                TransferId = transfer.Id,
                Type = TransactionType.Transfer,
                Direction = TransactionDirection.Income,
                Amount = request.Amount,
                Currency = request.Currency,
                Category = "Transfer",
                Description = description,
                Counterparty = fromAccount.Name,
                Status = TransactionStatus.Completed
            });
        }

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return CreatedAtAction(nameof(CreateTransfer), transfer.ToResponse());
    }
}
