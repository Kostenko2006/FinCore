using System.Data.Common;
using FinCore.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FinCore.Api.Tests;

public sealed class FinCoreApiFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;

    public FinCoreApiFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Error);
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "FinCore.Bank.Tests",
                ["Jwt:Audience"] = "FinCore.Client.Tests",
                ["Jwt:Key"] = "FinCoreBankTestingSecretKey2026.MustBeLongEnough",
                ["Jwt:ExpiresMinutes"] = "60",
                ["Cors:AllowedOrigins:0"] = "http://localhost:5173"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<FinCoreDbContext>>();
            services.AddDbContext<FinCoreDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
