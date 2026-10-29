
namespace Gaia.Domain.Models;

public sealed record OperationDefinition(
    string Id,
    string Name,
    string Description,
    string InputType
);
