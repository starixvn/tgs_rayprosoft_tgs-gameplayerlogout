using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.MessageHandlers;

internal class BigGamingMessageHandler : IMessageHandler
{
    private readonly ILogger<BigGamingMessageHandler> _logger;
    private readonly ICommandService<LogOutCommand> _service;

    public BigGamingMessageHandler(
        ICommandService<LogOutCommand> service,
        ILogger<BigGamingMessageHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ValueTask HandleAsync(
        ReadOnlyMemory<byte> data,
        CancellationToken cancellationToken = default)
    {
        var request = PlayerLogOutRequest.Parser.ParseFrom(data.Span) ??
              throw new NullReferenceException("The request data resolves to a null value");

        var command = new LogOutCommand(
            GameProviderType.BigGaming,
            request.PlayerCode);

        _logger.LogInformation("LogOn:{logOn} Information Command:{command}", DateTime.Now.ToString("o"), command);

        return _service
            .ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
