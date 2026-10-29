
using Gaia.Api.Extensions;
using Gaia.Application.Services;
using Gaia.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddOperationEngine_RegistersSixOperations()
    {
        var services = new ServiceCollection();

        services.AddOperationEngine();

        using var provider = services.BuildServiceProvider();

        var operations = provider
            .GetServices<IOperation>()
            .ToArray();

        Assert.Equal(6, operations.Length);
    }

    [Fact]
    public void AddOperationEngine_ResolvesOperationService()
    {
        var services = new ServiceCollection();

        services.AddOperationEngine();

        using var provider = services.BuildServiceProvider();

        var service = provider
            .GetRequiredService<OperationService>();

        Assert.NotNull(service);
    }

    [Fact]
    public void AddOperationEngine_ExecutesOperation()
    {
        var services = new ServiceCollection();

        services.AddOperationEngine();

        using var provider = services.BuildServiceProvider();

        var service = provider
            .GetRequiredService<OperationService>();

        var result = service.Execute("multiply", "6", "7");

        Assert.Equal(42m, result.Value);
    }

    [Fact]
    public void AddOperationEngine_RegistersServicesAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddOperationEngine();

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<OperationService>();
        var second = provider.GetRequiredService<OperationService>();

        Assert.Same(first, second);
    }
}
