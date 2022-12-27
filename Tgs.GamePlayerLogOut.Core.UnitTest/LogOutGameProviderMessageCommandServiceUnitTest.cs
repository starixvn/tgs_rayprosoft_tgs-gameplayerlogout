using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.MessageQueue;

namespace Tgs.GamePlayerLogOut.Core.UnitTest;

[TestClass]
public class LogOutGameProviderMessageCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var command = new LogOutGameProviderMessageCommand(
            "AWC_SG",
            "test");

        var fakeLog = NullLogger<LogOutGameProviderMessageCommandService>.Instance;
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var service = new LogOutGameProviderMessageCommandService(
            fakeLog,
            fakeMessageSender);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        await fakeMessageSender
            .Received(1)
            .PublishAsync(
               Arg.Is($"gp.kickplayer.{command.GameProviderCode.Replace("_", ".")}".ToLower()),
               Arg.Any<ReadOnlyMemory<byte>>(),
               Arg.Any<CancellationToken>());
    }
}
