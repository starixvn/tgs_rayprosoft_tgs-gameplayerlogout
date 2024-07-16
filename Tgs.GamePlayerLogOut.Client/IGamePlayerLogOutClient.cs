namespace Tgs.GamePlayerLogOut.Client;

public interface IGamePlayerLogOutClient
{
	ValueTask LogOutAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask LogOutAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);
}
