
using Microsoft.EntityFrameworkCore;

namespace Gaia.Infrastructure.Persistence;

public sealed class OperationHistoryRepository
{
    private readonly GaiaDbContext _dbContext;

    public OperationHistoryRepository(GaiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        OperationHistory history,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.OperationHistories.AddAsync(
            history,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OperationHistory>> GetLatestAsync(
        string operationId,
        int count = 3,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.OperationHistories
            .AsNoTracking()
            .Where(x => x.OperationId == operationId)
            .OrderByDescending(x => x.ExecutedAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMonthlyCountAsync(
        string operationId,
        DateTime monthStartUtc,
        DateTime nextMonthStartUtc,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.OperationHistories
            .AsNoTracking()
            .CountAsync(
                x => x.OperationId == operationId &&
                     x.ExecutedAtUtc >= monthStartUtc &&
                     x.ExecutedAtUtc < nextMonthStartUtc,
                cancellationToken);
    }
}
