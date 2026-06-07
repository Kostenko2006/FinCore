using FinCore.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Data;

public class FinCoreDbContext(DbContextOptions<FinCoreDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<BankCard> Cards => Set<BankCard>();
    public DbSet<BankTransaction> Transactions => Set<BankTransaction>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(255).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(24).IsRequired();
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasIndex(x => x.AccountNumber).IsUnique();
            entity.HasIndex(x => x.Iban).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.Status });
            entity.Property(x => x.AccountNumber).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Iban).HasMaxLength(34).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Currency).HasConversion<string>().HasMaxLength(3).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Balance).HasPrecision(18, 2);
            entity.HasOne(x => x.User)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BankCard>(entity =>
        {
            entity.ToTable("cards");
            entity.HasIndex(x => new { x.UserId, x.Status });
            entity.Property(x => x.CardHolder).HasMaxLength(180).IsRequired();
            entity.Property(x => x.MaskedNumber).HasMaxLength(24).IsRequired();
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.DailyLimit).HasPrecision(18, 2);
            entity.HasOne(x => x.User)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Account)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasIndex(x => new { x.UserId, x.CreatedAt });
            entity.HasIndex(x => new { x.AccountId, x.CreatedAt });
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Direction).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Currency).HasConversion<string>().HasMaxLength(3).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Category).HasMaxLength(80);
            entity.Property(x => x.Description).HasMaxLength(280);
            entity.Property(x => x.Counterparty).HasMaxLength(180);
            entity.HasOne(x => x.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Account)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Transfer)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.TransferId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.ToTable("transfers");
            entity.HasIndex(x => new { x.UserId, x.CreatedAt });
            entity.Property(x => x.Currency).HasConversion<string>().HasMaxLength(3).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.ExternalIban).HasMaxLength(34);
            entity.Property(x => x.Description).HasMaxLength(280);
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.FromAccount)
                .WithMany()
                .HasForeignKey(x => x.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ToAccount)
                .WithMany()
                .HasForeignKey(x => x.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
