
using Gaia.Domain.Abstractions;
using Gaia.Domain.Models;
using Gaia.Domain.Utilities;

namespace Gaia.Domain.Operations.Arithmetic;

public sealed class MultiplyOperation : IOperation
{
    public OperationDefinition Definition { get; } = new(
        "multiply",
        "Multiplication",
        "Multiplies two numbers",
        "number"
    );

    public OperationResult Execute(string inputA, string inputB)
    {
        decimal a = NumericInputParser.Parse(inputA, "A");
        decimal b = NumericInputParser.Parse(inputB, "B");

        decimal result = checked(a * b);

        return new OperationResult(Definition.Id, result);
    }
}
