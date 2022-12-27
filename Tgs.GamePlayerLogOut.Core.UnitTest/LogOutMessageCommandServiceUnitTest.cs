using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.MessageQueue;

namespace Tgs.GamePlayerLogOut.Core.UnitTest;

[TestClass]
public class LogOutMessageCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var command = new LogOutMessageCommand(
            "test");

        var fakeLog = NullLogger<LogOutMessageCommandService>.Instance;
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var service = new LogOutMessageCommandService(
            fakeLog,
            fakeMessageSender);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        await fakeMessageSender
            .Received(1)
            .PublishAsync(
               Arg.Is("gp.kickplayer"),
               Arg.Any<ReadOnlyMemory<byte>>(),
               Arg.Any<CancellationToken>());
    }
}
