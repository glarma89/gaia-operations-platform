
using Gaia.Domain.Abstractions;
using Gaia.Domain.Exceptions;

namespace Gaia.Application.Services;

public sealed class OperationRegistry
{
    private readonly Dictionary<string, IOperation> _operations;

    public OperationRegistry(IEnumerable<IOperation> operations)
    {
        _operations = new Dictionary<string, IOperation>(
            StringComparer.OrdinalIgnoreCase
        );

        foreach (IOperation operation in operations)
        {
            string id = operation.Definition.Id;

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "Operation ID cannot be empty.");
            }

            if (!_operations.TryAdd(id, operation))
            {
                throw new InvalidOperationException(
                    $"Duplicate operation ID: '{id}'.");
            }
        }
    }

    public IOperation Get(string operationId)
    {
        if (string.IsNullOrWhiteSpace(operationId) ||
            !_operations.TryGetValue(operationId, out IOperation? operation))
        {
            throw new OperationNotFoundException(
                operationId ?? string.Empty);
        }

        return operation;
    }

    public IReadOnlyCollection<IOperation> GetAll()
    {
        return _operations.Values.ToArray();
    }
}
