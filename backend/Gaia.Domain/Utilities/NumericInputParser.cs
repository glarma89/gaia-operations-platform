
using System.Globalization;
using Gaia.Domain.Exceptions;

namespace Gaia.Domain.Utilities;

public static class NumericInputParser
{
    public static decimal Parse(
        string input,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new OperationValidationException(
                $"Parameter {parameterName} is required.");
        }

        if (!decimal.TryParse(
            input,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out decimal result))
        {
            throw new OperationValidationException(
                $"Parameter {parameterName} must be a valid number.");
        }

        return result;
    }
}
