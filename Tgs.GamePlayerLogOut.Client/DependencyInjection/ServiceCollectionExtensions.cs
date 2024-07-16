using Tgs.GamePlayerLogOut.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerLogOutClient(this IServiceCollection services, Action<IHttpClientBuilder> httpClientConfigure)
	{
		httpClientConfigure(services.AddHttpClient<IGamePlayerLogOutClient, GamePlayerLogOutClient>());

		return services;
	}
}
