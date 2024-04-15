using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.MessageHandlers;

internal class V8PokerMessageHandler(
    ILogger<V8PokerMessageHandler> logger,
    ICommandService<LogOutCommand> service) : IMessageHandler<PlayerLogOutRequest>
{
    public ValueTask HandleAsync(string subject, PlayerLogOutRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
    {
        var command = new LogOutCommand(
            GameProviderType.V8Poker,
            data.PlayerCode);

        logger.LogInformation("LogOn:{logOn} Subject:{subject} Command content:{command} Mesaage:This message has been received.",
            DateTime.Now.ToString("o"),
            subject,
            command);

        return service
            .ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
