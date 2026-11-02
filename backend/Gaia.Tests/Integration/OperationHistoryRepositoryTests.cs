
using Gaia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.Tests.Integration;

[Collection("DatabaseIntegration")]
public sealed class OperationHistoryRepositoryTests
{
    private readonly GaiaApiFactory _factory;

    public OperationHistoryRepositoryTests(GaiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMonthlyCount_ExcludesRecordsOutsideMonth()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<GaiaDbContext>();

        var repository = new OperationHistoryRepository(dbContext);

        var monthStart = new DateTime(
            2026, 11, 1, 0, 0, 0, DateTimeKind.Utc);

        var nextMonthStart = monthStart.AddMonths(1);

        var records = new[]
        {
            new OperationHistory
            {
                OperationId = "monthly-boundary-test",
                InputA = "1",
                InputB = "2",
                ResultJson = "3",
                ExecutedAtUtc = monthStart.AddTicks(-1)
            },
            new OperationHistory
            {
                OperationId = "monthly-boundary-test",
                InputA = "1",
                InputB = "2",
                ResultJson = "3",
                ExecutedAtUtc = monthStart
            },
            new OperationHistory
            {
                OperationId = "monthly-boundary-test",
                InputA = "1",
                InputB = "2",
                ResultJson = "3",
                ExecutedAtUtc = nextMonthStart.AddTicks(-1)
            },
            new OperationHistory
            {
                OperationId = "monthly-boundary-test",
                InputA = "1",
                InputB = "2",
                ResultJson = "3",
                ExecutedAtUtc = nextMonthStart
            }
        };

        dbContext.OperationHistories.AddRange(records);
        await dbContext.SaveChangesAsync();

        // Act
        var count = await repository.GetMonthlyCountAsync(
            "monthly-boundary-test",
            monthStart,
            nextMonthStart);

        // Assert
        Assert.Equal(2, count);
    }
}
