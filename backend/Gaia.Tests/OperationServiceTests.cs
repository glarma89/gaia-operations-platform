
using Gaia.Application.Services;
using Gaia.Domain.Abstractions;
using Gaia.Domain.Exceptions;
using Gaia.Domain.Operations.Arithmetic;
using Gaia.Domain.Operations.Strings;

namespace Gaia.Tests;

public class OperationServiceTests
{
    private static OperationService CreateService()
    {
        IOperation[] operations =
        [
            new AddOperation(),
            new SubtractOperation(),
            new MultiplyOperation(),
            new DivideOperation(),
            new ConcatOperation(),
            new EqualsOperation()
        ];

        var registry = new OperationRegistry(operations);

        return new OperationService(registry);
    }

    [Fact]
    public void Execute_Add_ReturnsCorrectResult()
    {
        var service = CreateService();

        var result = service.Execute("add", "10", "5");

        Assert.Equal(15m, result.Value);
        Assert.Equal("add", result.OperationId);
    }

    [Fact]
    public void Execute_Concat_ReturnsCorrectResult()
    {
        var service = CreateService();

        var result = service.Execute(
            "concat", "Hello", "World");

        Assert.Equal("HelloWorld", result.Value);
    }

    [Fact]
    public void Execute_Equals_ReturnsBooleanResult()
    {
        var service = CreateService();

        var result = service.Execute(
            "equals", "Hello", "Hello");

        Assert.Equal(true, result.Value);
    }

    [Fact]
    public void Execute_UnknownOperation_ThrowsException()
    {
        var service = CreateService();

        Assert.Throws<OperationNotFoundException>(
            () => service.Execute("unknown", "1", "2"));
    }

    [Fact]
    public void GetAvailableOperations_ReturnsSixOperations()
    {
        var service = CreateService();

        var operations = service.GetAvailableOperations();

        Assert.Equal(6, operations.Count);
    }
}
