
namespace Gaia.Domain.Models;

public sealed record OperationResult(
    string OperationId,
    object Value
);
