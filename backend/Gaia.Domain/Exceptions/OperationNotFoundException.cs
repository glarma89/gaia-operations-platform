
namespace Gaia.Domain.Exceptions;

public sealed class OperationNotFoundException
    : Exception
{
    public OperationNotFoundException(string operationId)
        : base($"Operation '{operationId}' was not found.")
    {
    }
}
