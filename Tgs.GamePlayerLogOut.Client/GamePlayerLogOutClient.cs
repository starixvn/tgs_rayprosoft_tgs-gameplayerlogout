using Tgs.GamePlayerLogOut.Protos;

namespace Tgs.GamePlayerLogOut.Client;

internal class GamePlayerLogOutClient : IGamePlayerLogOutClient
{
    private readonly GamePlayerLogOutService.GamePlayerLogOutServiceClient _client;

    public GamePlayerLogOutClient(GamePlayerLogOutService.GamePlayerLogOutServiceClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<bool> LogOutAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
        => (await _client
        .LogOutAsync(
            request: new LogOutRequest
            {
                PlayerCode = playerCode
            },
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)).Success;

    public async ValueTask<bool> LogOutByGameProviderAsync(
        string playerCode,
        string gameProviderCode,
        CancellationToken cancellationToken = default)
        => (await _client
        .LogOutByGameProviderAsync(
            request: new LogOutByGameProviderRequest
            {
                PlayerCode = playerCode,
                GameProviderCode = gameProviderCode
            },
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)).Success;
}
