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
public class AccountsController(FinCoreDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AccountResponse>>> GetAccounts()
    {
        var userId = User.GetUserId();
        var accounts = await db.Accounts
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToResponse())
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AccountResponse>> GetAccount(long id)
    {
        var userId = User.GetUserId();
        var account = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        return account is null ? NotFound(new { message = "Account not found." }) : Ok(account.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> CreateAccount(CreateAccountRequest request)
    {
        var userId = User.GetUserId();
        var account = new Account
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Type = request.Type,
            Currency = request.Currency,
            Balance = 0,
            AccountNumber = $"2600{Random.Shared.NextInt64(10_000_000_000, 99_999_999_999)}",
            Iban = $"UA{Random.Shared.Next(10, 99)}3000010000{Random.Shared.NextInt64(10_000_000_000, 99_999_999_999)}",
            Status = AccountStatus.Active
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account.ToResponse());
    }
}
