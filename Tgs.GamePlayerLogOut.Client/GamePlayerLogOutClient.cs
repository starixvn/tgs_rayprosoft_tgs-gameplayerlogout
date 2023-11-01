using Google.Protobuf;
using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.Extensions;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.Client;

internal class GamePlayerLogOutClient : IGamePlayerLogOutClient
{
    private readonly IMessageSender _messageSender;

    public GamePlayerLogOutClient(IMessageSender messageSender)
    {
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public ValueTask LogOutAsync(string playerCode, CancellationToken cancellationToken = default)
    {
        var request = new PlayerLogOutRequest
        {
            PlayerCode = playerCode,
        };

        return _messageSender.PublishAsync(
            subject: "gp.player.logout",
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask LogOutAsync(
        string playerCode,
        string gameProviderCode,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerLogOutRequest
        {
            PlayerCode = playerCode,
        };

        return _messageSender.PublishAsync(
            subject: gameProviderCode.SubjectToString(),
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
