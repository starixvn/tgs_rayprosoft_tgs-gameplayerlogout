using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

internal class LogOutMessageCommandService : ICommandService<LogOutMessageCommand>
{
    private readonly ILogger<LogOutMessageCommandService> _logger;
    private readonly IMessageSender _sender;

    public LogOutMessageCommandService(
        ILogger<LogOutMessageCommandService> logger,
        IMessageSender sender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(LogOutMessageCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("LogOn:{logOn} Information Command:{command}", DateTime.Now.ToString("o"), command);

        var request = new PlayerLogOutRequest
        {
            PlayerCode = command.PlayerCode,
        };

        return _sender.PublishAsync(
             subject: "gp.player.logout",
             data: request.ToByteArray(),
             cancellationToken: cancellationToken);
    }
}
