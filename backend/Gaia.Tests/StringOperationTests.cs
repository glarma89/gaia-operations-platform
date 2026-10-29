
using Gaia.Domain.Operations.Strings;

namespace Gaia.Tests;

public class StringOperationTests
{
    [Fact]
    public void Concat_TwoStrings_ReturnsCombinedString()
    {
        var operation = new ConcatOperation();

        var result = operation.Execute("Hello", "World");

        Assert.Equal("HelloWorld", result.Value);
    }

    [Fact]
    public void Concat_WithEmptyString_ReturnsOtherString()
    {
        var operation = new ConcatOperation();

        var result = operation.Execute("Hello", "");

        Assert.Equal("Hello", result.Value);
    }

    [Fact]
    public void Concat_TwoEmptyStrings_ReturnsEmptyString()
    {
        var operation = new ConcatOperation();

        var result = operation.Execute("", "");

        Assert.Equal("", result.Value);
    }

    [Fact]
    public void Equals_IdenticalStrings_ReturnsTrue()
    {
        var operation = new EqualsOperation();

        var result = operation.Execute("Hello", "Hello");

        Assert.Equal(true, result.Value);
    }

    [Fact]
    public void Equals_DifferentStrings_ReturnsFalse()
    {
        var operation = new EqualsOperation();

        var result = operation.Execute("Hello", "World");

        Assert.Equal(false, result.Value);
    }

    [Fact]
    public void Equals_DifferentCase_ReturnsFalse()
    {
        var operation = new EqualsOperation();

        var result = operation.Execute("Hello", "hello");

        Assert.Equal(false, result.Value);
    }

    [Fact]
    public void Equals_TwoEmptyStrings_ReturnsTrue()
    {
        var operation = new EqualsOperation();

        var result = operation.Execute("", "");

        Assert.Equal(true, result.Value);
    }
}
