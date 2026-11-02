
using Microsoft.EntityFrameworkCore;

namespace Gaia.Infrastructure.Persistence;

public sealed class GaiaDbContext : DbContext
{
    public GaiaDbContext(DbContextOptions<GaiaDbContext> options)
        : base(options)
    {
    }

    public DbSet<OperationHistory> OperationHistories
        => Set<OperationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OperationHistory>(entity =>
        {
            entity.ToTable("OperationHistory");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OperationId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.InputA)
                .IsRequired();

            entity.Property(x => x.InputB)
                .IsRequired();

            entity.Property(x => x.ResultJson)
                .IsRequired();

            entity.Property(x => x.ExecutedAtUtc)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.OperationId,
                x.ExecutedAtUtc
            });
        });
    }
}
