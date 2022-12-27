namespace Tgs.GamePlayerLogOut.Client;

public interface IGamePlayerLogOutClient
{
    ValueTask<bool> LogOutAsync(string playerCode, CancellationToken cancellationToken = default);

    ValueTask<bool> LogOutByGameProviderAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);
}
