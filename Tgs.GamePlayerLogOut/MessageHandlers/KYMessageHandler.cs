using System.Text.Json;
using Tgs.GamePlayerLogOut.Protos;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.MessageHandlers;

internal class KYMessageHandler(
	ILogger<KYMessageHandler> logger,
	ICommandService<LogOutCommand> service) : IMessageHandler<PlayerLogOutRequest>
{
	public ValueTask HandleAsync(string subject, PlayerLogOutRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var command = new LogOutCommand(
			GameProviderType.Kaiyuan,
			data.PlayerCode);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = JsonSerializer.SerializeToNode(command),
				Message = "This receive message handler."
			}));

		return service
			.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
