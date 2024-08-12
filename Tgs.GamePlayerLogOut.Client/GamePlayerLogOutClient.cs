namespace Tgs.GamePlayerLogOut.Client;

internal class GamePlayerLogOutClient(HttpClient httpClient) : IGamePlayerLogOutClient
{
	public async ValueTask LogOutAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient
			.DeleteAsync(
			   requestUri: $"player/{playerCode}/logout",
			   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask LogOutAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient
			.DeleteAsync(
			   requestUri: $"player/{playerCode}/{gameProviderCode}/logout",
			   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
}
