
using Gaia.Domain.Abstractions;
using Gaia.Domain.Models;
using Gaia.Domain.Utilities;

namespace Gaia.Domain.Operations.Arithmetic;

public sealed class SubtractOperation : IOperation
{
    public OperationDefinition Definition { get; } = new(
        "subtract",
        "Subtraction",
        "Subtracts the second number from the first",
        "number"
    );

    public OperationResult Execute(string inputA, string inputB)
    {
        decimal a = NumericInputParser.Parse(inputA, "A");
        decimal b = NumericInputParser.Parse(inputB, "B");

        decimal result = checked(a - b);

        return new OperationResult(Definition.Id, result);
    }
}
