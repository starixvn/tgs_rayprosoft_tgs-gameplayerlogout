using System.Reflection;
using Tgs.GameProviders.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGamePlayerLogOutCore(
        this IServiceCollection services,
        Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddGameProvidersClient(gameProvidersClientOptions);
}
