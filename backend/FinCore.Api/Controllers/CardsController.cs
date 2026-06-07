using FinCore.Api.Contracts.Banking;
using FinCore.Api.Data;
using FinCore.Api.Domain.Entities;
using FinCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CardsController(FinCoreDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CardResponse>>> GetCards()
    {
        var userId = User.GetUserId();
        var cards = await db.Cards
            .Include(x => x.Account)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToResponse())
            .ToListAsync();

        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CardResponse>> CreateCard(CreateCardRequest request)
    {
        var userId = User.GetUserId();
        var account = await db.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountId && x.UserId == userId);
        if (account is null)
        {
            return NotFound(new { message = "Account not found." });
        }

        var user = await db.Users.FindAsync(userId);
        var card = new BankCard
        {
            UserId = userId,
            AccountId = account.Id,
            CardHolder = (user?.FullName ?? "FINCORE CLIENT").ToUpperInvariant(),
            MaskedNumber = $"5168 **** **** {Random.Shared.Next(1000, 9999)}",
            ExpiryMonth = DateTime.UtcNow.Month,
            ExpiryYear = DateTime.UtcNow.Year + 4,
            Type = request.Type,
            DailyLimit = request.DailyLimit,
            Account = account
        };

        db.Cards.Add(card);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCards), card.ToResponse());
    }
}
