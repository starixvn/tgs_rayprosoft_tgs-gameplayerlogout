using Microsoft.Extensions.Logging;
using Tgs.GameCommand.Client;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

internal class LogOutCommandService(
    ILogger<LogOutCommandService> logger,
    IGameCommandClient gameCommandClient,
    IGameProvidersClient client) : ICommandService<LogOutCommand>
{
    public async ValueTask ExecuteAsync(LogOutCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await gameCommandClient.PlayerGetAsync(
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This player({command.PlayerCode}) does not exist.");

            var gameProviderId = await gameCommandClient.GameProviderGetGameProviderIdAsync(
                gameProviderCode: command.GameProviderCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This game provider({command.GameProviderCode}) does not exist.");

            _ = await gameCommandClient.BrandGetGameProviderConfigurationIdAsync(
               brandId: player.BrandId,
               gameProviderId: gameProviderId,
               cancellationToken: cancellationToken)
               .ConfigureAwait(false)
               ?? throw new NullReferenceException($"This game provider({command.GameProviderCode}) is not configured on this platform({player.BrandCode}).");

            var gameProvider = await gameCommandClient.GameProviderGetAsync(
                gameProviderId: gameProviderId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This game provider({command.GameProviderCode}) does not exist.");

            if (gameProvider.AllowKickPlayer)
            {
                await client.PlayerLogoutAsync(
                    gameProviderCode: command.GameProviderCode,
                    playerCode: command.PlayerCode,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (NullReferenceException ex)
        {
            logger.LogInformation(ex, "LogOn:{logOn} Command content:{command} Message:{message}", DateTime.Now.ToString("o"), command, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LogOn:{logOn} Command content:{command} Message:{message}", DateTime.Now.ToString("o"), command, ex.Message);
        }
    }
}
