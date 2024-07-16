using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePlayerLogOut.Client;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.IntegrationTest;
public partial class GamePlayerLogOutTest
{
	[Fact]
	public async Task LogOut()
	{
		var playerCode = "4ucmqplxb029";

		var web = new GamePlayerOnlineApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(sp =>
		{
			var fakeMessageSender = Substitute.For<IMessageSender>();

			_ = fakeMessageSender.PublishAsync(
					Arg.Is("gp.player.logout"),
					Arg.Is(new Protos.PlayerLogOutRequest
					{
						PlayerCode = playerCode
					}),
					Arg.Any<CancellationToken>());

			return fakeMessageSender;
		})));

		var httpClient = web.CreateClient();

		var sut = new GamePlayerLogOutClient(httpClient);

		await sut.LogOutAsync(playerCode);
	}

	[Fact]
	public async Task LogOut_GameProviderCode()
	{
		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var web = new GamePlayerOnlineApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(sp =>
		{
			var fakeMessageSender = Substitute.For<IMessageSender>();

			_ = fakeMessageSender.PublishAsync(
					Arg.Is("gp.player.logout.ky"),
					Arg.Is(new Protos.PlayerLogOutRequest
					{
						PlayerCode = playerCode
					}),
					Arg.Any<CancellationToken>());

			return fakeMessageSender;
		})));

		var httpClient = web.CreateClient();

		var sut = new GamePlayerLogOutClient(httpClient);

		await sut.LogOutAsync( playerCode, gameProviderCode);
	}
}
