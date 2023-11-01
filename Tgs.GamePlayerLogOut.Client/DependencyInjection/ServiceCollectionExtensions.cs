using Tgs.GamePlayerLogOut.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGamePlayerLogOutClient(
        this IServiceCollection services)
        => services
        .AddTransient<IGamePlayerLogOutClient, GamePlayerLogOutClient>();
}
