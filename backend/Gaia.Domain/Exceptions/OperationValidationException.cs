
namespace Gaia.Domain.Exceptions;

public sealed class OperationValidationException
    : Exception
{
    public OperationValidationException(string message)
        : base(message)
    {
    }
}
