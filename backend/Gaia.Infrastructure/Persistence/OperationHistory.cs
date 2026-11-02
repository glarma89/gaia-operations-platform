
namespace Gaia.Infrastructure.Persistence;

public sealed class OperationHistory
{
    public long Id { get; set; }

    public string OperationId { get; set; } = string.Empty;

    public string InputA { get; set; } = string.Empty;

    public string InputB { get; set; } = string.Empty;

    public string ResultJson { get; set; } = string.Empty;

    public DateTime ExecutedAtUtc { get; set; }
}
