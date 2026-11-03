
using Gaia.Domain.Abstractions;
using Gaia.Domain.Exceptions;
using Gaia.Domain.Models;
using Gaia.Domain.Utilities;

namespace Gaia.Domain.Operations.Arithmetic;

public sealed class DivideOperation : IOperation
{
    public OperationDefinition Definition { get; } = new(
        "divide",
        "Division",
        "Divides the first number by the second",
        "number"
    );

    public OperationResult Execute(string inputA, string inputB)
    {
        decimal a = NumericInputParser.Parse(inputA, "A");
        decimal b = NumericInputParser.Parse(inputB, "B");

        if (b == 0)
        {
            throw new OperationValidationException(
                "Division by zero is not allowed.");
        }


        try
        {
            decimal result = a / b;

            return new OperationResult(Definition.Id, result);
        }
        catch (OverflowException)
        {
            throw new OperationValidationException(
                "The result exceeds the supported numeric range.");
        }

    }
}
