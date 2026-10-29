
using Gaia.Application.Services;
using Gaia.Domain.Abstractions;
using Gaia.Domain.Exceptions;
using Gaia.Domain.Operations.Arithmetic;
using Gaia.Domain.Operations.Strings;

namespace Gaia.Tests;

public class OperationRegistryTests
{
    [Fact]
    public void Get_ExistingOperation_ReturnsCorrectOperation()
    {
        IOperation[] operations =
        [
            new AddOperation(),
            new DivideOperation()
        ];

        var registry = new OperationRegistry(operations);

        var result = registry.Get("add");

        Assert.IsType<AddOperation>(result);
    }

    [Fact]
    public void Get_OperationIdIsCaseInsensitive()
    {
        IOperation[] operations =
        [
            new AddOperation()
        ];

        var registry = new OperationRegistry(operations);

        var result = registry.Get("ADD");

        Assert.IsType<AddOperation>(result);
    }

    [Fact]
    public void Get_UnknownOperation_ThrowsException()
    {
        var registry = new OperationRegistry(
            Array.Empty<IOperation>());

        Assert.Throws<OperationNotFoundException>(
            () => registry.Get("unknown"));
    }

    [Fact]
    public void Constructor_DuplicateIds_ThrowsException()
    {
        IOperation[] operations =
        [
            new AddOperation(),
            new AddOperation()
        ];

        Assert.Throws<InvalidOperationException>(
            () => new OperationRegistry(operations));
    }

    [Fact]
    public void GetAll_ReturnsAllRegisteredOperations()
    {
        IOperation[] operations =
        [
            new AddOperation(),
            new DivideOperation(),
            new ConcatOperation()
        ];

        var registry = new OperationRegistry(operations);

        Assert.Equal(3, registry.GetAll().Count);
    }
}
