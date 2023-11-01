using Microsoft.Extensions.Logging;
using Tgs.GameCommand.Client;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

internal class LogOutCommandService : ICommandService<LogOutCommand>
{
    private readonly IGameProvidersClient _client;
    private readonly IGameCommandClient _gameCommandClient;
    private readonly ILogger<LogOutCommandService> _logger;

    public LogOutCommandService(
        ILogger<LogOutCommandService> logger,
        IGameCommandClient gameCommandClient,
        IGameProvidersClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameCommandClient = gameCommandClient ?? throw new ArgumentNullException(nameof(gameCommandClient));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(LogOutCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await _gameCommandClient.PlayerGetAsync(
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This player({command.PlayerCode}) does not exist.");

            var gameProviderId = await _gameCommandClient.GameProviderGetGameProviderIdAsync(
                gameProviderCode: command.GameProviderCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This game provider({command.GameProviderCode}) does not exist.");

            _ = await _gameCommandClient.BrandGetGameProviderConfigurationIdAsync(
               brandId: player.BrandId,
               gameProviderId: gameProviderId,
               cancellationToken: cancellationToken)
               .ConfigureAwait(false)
               ?? throw new NullReferenceException($"This game provider({command.GameProviderCode}) is not configured on this platform({player.BrandCode}).");

            await _client.KickPlayerAsync(
                gameProviderCode: command.GameProviderCode,
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (NullReferenceException ex)
        {
            _logger.LogInformation(ex, "LogOn:{logOn} Information Command:{command} Message:{message}", DateTime.Now.ToString("o"), command, ex.Message);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "LogOn:{logOn} Information Command:{command} Message:{message}", DateTime.Now.ToString("o"), command, ex.Message);
        }
    }
}
