using Tgs.GamePlayerLogOut.Core.Extensions;
using Valhalla.MessageQueue.Nats.Configuration;

namespace Tgs.GamePlayerLogOut.Extensions;

public static class NatsMessageQueueConfigurationExtensions
{
	public static NatsMessageQueueConfiguration AddLogoutHandlerWithSubjectSuffix<THandler>(this NatsMessageQueueConfiguration configuration, string suffix)
	{
		var subject = suffix.ToGamePlayerLogOutSubject();
		return configuration
			.AddHandler<THandler>("gp.player.logout")
			.AddHandler<THandler>(subject, subject);
	}
}
