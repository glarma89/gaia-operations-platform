
using Gaia.Domain.Abstractions;
using Gaia.Domain.Models;

namespace Gaia.Domain.Operations.Strings;

public sealed class EqualsOperation : IOperation
{
    public OperationDefinition Definition { get; } = new(
        "equals",
        "String Equality",
        "Compares two strings using ordinal comparison",
        "string"
    );

    public OperationResult Execute(string inputA, string inputB)
    {
        ArgumentNullException.ThrowIfNull(inputA);
        ArgumentNullException.ThrowIfNull(inputB);

        bool result = string.Equals(
            inputA,
            inputB,
            StringComparison.Ordinal
        );

        return new OperationResult(Definition.Id, result);
    }
}
