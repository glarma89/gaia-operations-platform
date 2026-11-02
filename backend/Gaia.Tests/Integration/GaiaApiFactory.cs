
using Gaia.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gaia.Tests.Integration;

public sealed class GaiaApiFactory :
    WebApplicationFactory<Program>,
    IAsyncLifetime
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<GaiaDbContext>(options =>
                options.UseSqlServer(
                    "Server=localhost;" +
                    "Database=GaiaOperationsTestDb;" +
                    "Trusted_Connection=True;" +
                    "TrustServerCertificate=True;"));
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<GaiaDbContext>();

        if (dbContext.Database.GetDbConnection().Database
            != "GaiaOperationsTestDb")
        {
            throw new InvalidOperationException(
                "Refusing to clean a non-test database.");
        }

        await dbContext.OperationHistories.ExecuteDeleteAsync();
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
