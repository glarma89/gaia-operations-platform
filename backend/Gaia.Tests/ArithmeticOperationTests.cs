
using Gaia.Domain.Exceptions;
using Gaia.Domain.Operations.Arithmetic;

namespace Gaia.Tests;

public class ArithmeticOperationTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        var operation = new AddOperation();

        var result = operation.Execute("10", "5");

        Assert.Equal(15m, result.Value);
    }

    [Fact]
    public void Subtract_TwoNumbers_ReturnsDifference()
    {
        var operation = new SubtractOperation();

        var result = operation.Execute("10", "5");

        Assert.Equal(5m, result.Value);
    }

    [Fact]
    public void Multiply_TwoNumbers_ReturnsProduct()
    {
        var operation = new MultiplyOperation();

        var result = operation.Execute("10", "5");

        Assert.Equal(50m, result.Value);
    }

    [Fact]
    public void Divide_TwoNumbers_ReturnsQuotient()
    {
        var operation = new DivideOperation();

        var result = operation.Execute("10", "5");

        Assert.Equal(2m, result.Value);
    }

    [Fact]
    public void Divide_ByZero_ThrowsValidationException()
    {
        var operation = new DivideOperation();

        Assert.Throws<OperationValidationException>(
            () => operation.Execute("10", "0"));
    }

    [Fact]
    public void Add_InvalidInput_ThrowsValidationException()
    {
        var operation = new AddOperation();

        Assert.Throws<OperationValidationException>(
            () => operation.Execute("abc", "5"));
    }

    [Fact]
    public void Add_DecimalNumbers_ReturnsCorrectSum()
    {
        var operation = new AddOperation();

        var result = operation.Execute("0.1", "0.2");

        Assert.Equal(0.3m, result.Value);
    }


    [Fact]
    public void Add_Overflow_ThrowsValidationException()
    {
        var operation = new AddOperation();

        Assert.Throws<OperationValidationException>(
            () => operation.Execute(
                decimal.MaxValue.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                "1"));
    }


    [Fact]
    public void Subtract_Overflow_ThrowsValidationException()
    {
        var operation = new SubtractOperation();

        Assert.Throws<OperationValidationException>(
            () => operation.Execute(
                decimal.MinValue.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                "1"));
    }


    [Fact]
    public void Multiply_Overflow_ThrowsValidationException()
    {
        var operation = new MultiplyOperation();

        Assert.Throws<OperationValidationException>(
            () => operation.Execute(
                decimal.MaxValue.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                "2"));
    }


    [Fact]
    public void Divide_Overflow_ThrowsValidationException()
    {
        var operation = new DivideOperation();

        Assert.Throws<OperationValidationException>(
            () => operation.Execute(
                decimal.MaxValue.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                "0.1"));
    }




}
