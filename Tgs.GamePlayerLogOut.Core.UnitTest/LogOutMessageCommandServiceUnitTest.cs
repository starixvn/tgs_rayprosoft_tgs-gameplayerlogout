using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.Core.UnitTest;

public class LogOutMessageCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var command = new LogOutMessageCommand(
            "test");

        var fakeLog = NullLogger<LogOutMessageCommandService>.Instance;
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var service = new LogOutMessageCommandService(
            fakeLog,
            fakeMessageSender);

        await service.ExecuteAsync(command);

        await fakeMessageSender
            .Received(1)
            .PublishAsync(
               Arg.Is("gp.player.logout"),
               Arg.Any<ReadOnlyMemory<byte>>(),
               Arg.Any<CancellationToken>());
    }
}
