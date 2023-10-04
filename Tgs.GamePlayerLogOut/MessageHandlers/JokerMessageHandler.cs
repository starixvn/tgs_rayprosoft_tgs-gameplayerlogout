using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.MessageHandlers;

public class JokerMessageHandler : IMessageHandler
{
    private readonly ILogger<JokerMessageHandler> _logger;
    private readonly ICommandService<LogOutCommand> _service;

    public JokerMessageHandler(
        ICommandService<LogOutCommand> service,
        ILogger<JokerMessageHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask HandleAsync(
        ReadOnlyMemory<byte> data,
        CancellationToken cancellationToken = default)
    {
        var request = PlayerLogOutRequest.Parser.ParseFrom(data.Span) ??
              throw new NullReferenceException("The request data resolves to a null value");

        var command = new LogOutCommand(
            GameProviderType.Joker,
            request.PlayerCode);

        _logger.LogInformation("LogOn:{logOn} Information Command:{command}", DateTime.Now.ToString("o"), command);

        return _service
            .ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
