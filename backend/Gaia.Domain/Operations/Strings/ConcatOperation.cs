
using Gaia.Domain.Abstractions;
using Gaia.Domain.Models;

namespace Gaia.Domain.Operations.Strings;

public sealed class ConcatOperation : IOperation
{
    public OperationDefinition Definition { get; } = new(
        "concat",
        "Concatenation",
        "Combines two strings",
        "string"
    );

    public OperationResult Execute(string inputA, string inputB)
    {
        ArgumentNullException.ThrowIfNull(inputA);
        ArgumentNullException.ThrowIfNull(inputB);

        string result = string.Concat(inputA, inputB);

        return new OperationResult(Definition.Id, result);
    }
}
