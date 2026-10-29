
using Gaia.Domain.Abstractions;
using Gaia.Domain.Models;
using Gaia.Domain.Utilities;

namespace Gaia.Domain.Operations.Arithmetic;

public sealed class AddOperation : IOperation
{
    public OperationDefinition Definition { get; } = new(
        "add",
        "Addition",
        "Adds two numbers",
        "number"
    );

    public OperationResult Execute(string inputA, string inputB)
    {
        decimal a = NumericInputParser.Parse(inputA, "A");
        decimal b = NumericInputParser.Parse(inputB, "B");

        decimal result = checked(a + b);

        return new OperationResult(Definition.Id, result);
    }
}
