using Microsoft.Extensions.Options;
using Tgs.GamePlayerLogOut.Client;
using Tgs.GamePlayerLogOut.Protos;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGamePlayerLogOutClient(
        this IServiceCollection services,
        Action<GamePlayerLogOutClientOptions, IServiceProvider> configure)
        => services
        .AddOptions<GamePlayerLogOutClientOptions>()
        .Configure(configure)
        .Services
        .AddGrpcClient<GamePlayerLogOutService.GamePlayerLogOutServiceClient>((sp, configureClient) =>
        {
            var options = sp.GetRequiredService<IOptions<GamePlayerLogOutClientOptions>>().Value;
            configureClient.Address = options.BaseUrl;
        })
        .ConfigureChannel((sp, configureChannel) =>
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true
            };
            configureChannel.HttpHandler = handler;
        })
        .AddInterceptor<LoggingInterceptor>()
        .Services
        .AddSingleton<DiagnosticsInterceptorOptions>()
        .AddSingleton<LoggingInterceptor>()
        .AddTransient<IGamePlayerLogOutClient, GamePlayerLogOutClient>();
}
