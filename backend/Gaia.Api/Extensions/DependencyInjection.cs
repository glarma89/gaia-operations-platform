
using Gaia.Application.Services;
using Gaia.Domain.Abstractions;
using Gaia.Domain.Operations.Arithmetic;
using Gaia.Domain.Operations.Strings;

namespace Gaia.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddOperationEngine(
        this IServiceCollection services)
    {
        services.AddSingleton<IOperation, AddOperation>();
        services.AddSingleton<IOperation, SubtractOperation>();
        services.AddSingleton<IOperation, MultiplyOperation>();
        services.AddSingleton<IOperation, DivideOperation>();

        services.AddSingleton<IOperation, ConcatOperation>();
        services.AddSingleton<IOperation, EqualsOperation>();

        services.AddSingleton<OperationRegistry>();
        services.AddSingleton<OperationService>();

        return services;
    }
}
