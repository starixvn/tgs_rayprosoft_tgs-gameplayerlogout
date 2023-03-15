using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Client;

namespace Tgs.GamePlayerLogOut.Core.UnitTest;

[TestClass]
public class LogOutCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var command = new LogOutCommand(
            "ky",
            "test");

        var fakeLog = NullLogger<LogOutCommandService>.Instance;
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var service = new LogOutCommandService(
            fakeLog,
            fakeGameProvidersClient);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        await fakeGameProvidersClient
            .Received(1)
            .KickPlayerAsync(
               Arg.Is(command.GameProviderCode),
               Arg.Is(command.PlayerCode),
               Arg.Any<CancellationToken>());
    }
}
