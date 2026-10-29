
using Gaia.Domain.Models;

namespace Gaia.Domain.Abstractions;

public interface IOperation
{
    OperationDefinition Definition { get; }

    OperationResult Execute(
        string inputA,
        string inputB
    );
}
