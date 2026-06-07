using FinCore.Api.Contracts.Auth;
using FinCore.Api.Contracts.Banking;
using FinCore.Api.Data;
using FinCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin")]
public class AdminController(FinCoreDbContext db) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> GetUsers()
    {
        var users = await db.Users
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToResponse())
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("accounts")]
    public async Task<ActionResult<IReadOnlyCollection<AccountResponse>>> GetAccounts()
    {
        var accounts = await db.Accounts
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToResponse())
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<IReadOnlyCollection<TransactionResponse>>> GetTransactions()
    {
        var transactions = await db.Transactions
            .Include(x => x.Account)
            .OrderByDescending(x => x.CreatedAt)
            .Take(200)
            .Select(x => x.ToResponse())
            .ToListAsync();

        return Ok(transactions);
    }
}
