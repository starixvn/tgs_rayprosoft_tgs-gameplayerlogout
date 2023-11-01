using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Client;

namespace Tgs.GamePlayerLogOut.Core.UnitTest;

public class LogOutCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var command = new LogOutCommand(
            "KY",
            "test");

        var fakeLog = NullLogger<LogOutCommandService>.Instance;
        var fakeGameCommandClient = Substitute.For<IGameCommandClient>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "HKD");
        _ = fakeGameCommandClient.PlayerGetAsync(
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameCommandClient.GameProviderGetGameProviderIdAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        _ = fakeGameCommandClient.BrandGetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());

        var service = new LogOutCommandService(
            fakeLog,
            fakeGameCommandClient,
            fakeGameProvidersClient);

        await service.ExecuteAsync(command);

        await fakeGameProvidersClient
            .Received(1)
            .KickPlayerAsync(
               Arg.Is(command.GameProviderCode),
               Arg.Is(command.PlayerCode),
               Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task This_game_provider_does_not_exist()
    {
        var command = new LogOutCommand(
            "KY",
            "test");

        var fakeLog = NullLogger<LogOutCommandService>.Instance;
        var fakeGameCommandClient = Substitute.For<IGameCommandClient>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "HKD");
        _ = fakeGameCommandClient.PlayerGetAsync(
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        _ = fakeGameCommandClient.GameProviderGetGameProviderIdAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var service = new LogOutCommandService(
            fakeLog,
            fakeGameCommandClient,
            fakeGameProvidersClient);

        await service.ExecuteAsync(command);

        await fakeGameProvidersClient
            .Received(0)
            .KickPlayerAsync(
               Arg.Is(command.GameProviderCode),
               Arg.Is(command.PlayerCode),
               Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task This_game_provider_is_not_configured_on_this_platform()
    {
        var command = new LogOutCommand(
            "KY",
            "test");

        var fakeLog = NullLogger<LogOutCommandService>.Instance;
        var fakeGameCommandClient = Substitute.For<IGameCommandClient>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "HKD");
        _ = fakeGameCommandClient.PlayerGetAsync(
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameCommandClient.GameProviderGetGameProviderIdAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        _ = fakeGameCommandClient.BrandGetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var service = new LogOutCommandService(
            fakeLog,
            fakeGameCommandClient,
            fakeGameProvidersClient);

        await service.ExecuteAsync(command);

        await fakeGameProvidersClient
            .Received(0)
            .KickPlayerAsync(
               Arg.Is(command.GameProviderCode),
               Arg.Is(command.PlayerCode),
               Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task This_player_does_not_exist()
    {
        var command = new LogOutCommand(
            "KY",
            "test");

        var fakeLog = NullLogger<LogOutCommandService>.Instance;
        var fakeGameCommandClient = Substitute.For<IGameCommandClient>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        _ = fakeGameCommandClient.PlayerGetAsync(
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<PlayerInfo?>(null));

        var service = new LogOutCommandService(
            fakeLog,
            fakeGameCommandClient,
            fakeGameProvidersClient);

        await service.ExecuteAsync(command);

        await fakeGameProvidersClient
            .Received(0)
            .KickPlayerAsync(
               Arg.Is(command.GameProviderCode),
               Arg.Is(command.PlayerCode),
               Arg.Any<CancellationToken>());
    }
}
