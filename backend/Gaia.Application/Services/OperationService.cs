
using Gaia.Domain.Models;

namespace Gaia.Application.Services;

public sealed class OperationService
{
    private readonly OperationRegistry _registry;

    public OperationService(OperationRegistry registry)
    {
        _registry = registry;
    }

    public OperationResult Execute(
        string operationId,
        string inputA,
        string inputB)
    {
        var operation = _registry.Get(operationId);

        return operation.Execute(inputA, inputB);
    }

    public IReadOnlyCollection<OperationDefinition> GetAvailableOperations()
    {
        return _registry
            .GetAll()
            .Select(operation => operation.Definition)
            .OrderBy(definition => definition.Name)
            .ToArray();
    }
}
