using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.Core.UnitTest;

public class LogOutGameProviderMessageCommandServiceUnitTest
{
    [Fact]
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

        await service.ExecuteAsync(command);

        await fakeMessageSender
            .Received(1)
            .PublishAsync(
               Arg.Is($"gp.player.logout.{command.GameProviderCode.Replace("_", ".").ToLower()}"),
               Arg.Any<ReadOnlyMemory<byte>>(),
               Arg.Any<CancellationToken>());
    }
}
