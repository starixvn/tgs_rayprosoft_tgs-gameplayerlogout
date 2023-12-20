using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.MessageHandlers;

internal class AwcVenusMessageHandler(
    ILogger<AwcVenusMessageHandler> logger,
    ICommandService<LogOutCommand> service) : IMessageHandler
{
    public ValueTask HandleAsync(
        ReadOnlyMemory<byte> data,
        CancellationToken cancellationToken = default)
    {
        var request = PlayerLogOutRequest.Parser.ParseFrom(data.Span) ??
              throw new NullReferenceException("The request data resolves to a null value");

        var command = new LogOutCommand(
            GameProviderType.AWC_VENUS,
            request.PlayerCode);

        logger.LogInformation("LogOn:{logOn} Command content:{command}", DateTime.Now.ToString("o"), command);

        return service
            .ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
