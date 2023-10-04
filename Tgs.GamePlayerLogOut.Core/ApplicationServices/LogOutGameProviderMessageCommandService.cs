using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.Extensions;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

internal class LogOutGameProviderMessageCommandService : ICommandService<LogOutGameProviderMessageCommand>
{
    private readonly ILogger<LogOutGameProviderMessageCommandService> _logger;
    private readonly IMessageSender _sender;

    public LogOutGameProviderMessageCommandService(
        ILogger<LogOutGameProviderMessageCommandService> logger,
        IMessageSender sender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(LogOutGameProviderMessageCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("LogOn:{logOn} Information Command:{command}", DateTime.Now.ToString("o"), command);

        var request = new PlayerLogOutRequest
        {
            PlayerCode = command.PlayerCode,
        };
        return _sender.PublishAsync(
            subject: command.GameProviderCode.SubjectToString(),
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
