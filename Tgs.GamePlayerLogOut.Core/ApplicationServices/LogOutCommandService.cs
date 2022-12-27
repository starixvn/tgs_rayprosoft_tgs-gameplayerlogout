using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

internal class LogOutCommandService : ICommandService<LogOutCommand>
{
    private readonly IGameProvidersClient _client;
    private readonly ILogger<LogOutCommandService> _logger;

    public LogOutCommandService(ILogger<LogOutCommandService> logger, IGameProvidersClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(LogOutCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            return _client.KickPlayerAsync(
                gameProviderCode: command.GameProviderCode,
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Information Command:{command}", DateTime.Now.ToString("o"), command);
            return ValueTask.CompletedTask;
        }
    }
}
