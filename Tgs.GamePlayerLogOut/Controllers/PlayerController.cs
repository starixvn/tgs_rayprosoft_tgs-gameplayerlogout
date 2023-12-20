using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Tgs.GamePlayerLogOut.Common.Protos;
using Tgs.GamePlayerLogOut.Core.Extensions;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerLogOut.Controllers;

[ApiController]
[Route("player")]
public class PlayerController : ControllerBase
{
    /// <summary>
    /// Logs the out asynchronous.
    /// </summary>
    /// <param name="messageSender">The message sender.</param>
    /// <param name="playerCode">The player code.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Delete /player/4ucmqplxb029/logout
    /// </remarks>
    [HttpDelete("{playerCode}/logout", Name = "LogOut")]
    public ValueTask LogOutAsync(
        [FromServices] IMessageSender messageSender,
        string playerCode)
    {
        var request = new PlayerLogOutRequest
        {
            PlayerCode = playerCode,
        };

        return messageSender.PublishAsync(
            subject: "gp.player.logout",
            data: request.ToByteArray(),
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Logs the out by game provider asynchronous.
    /// </summary>
    /// <param name="messageSender">The message sender.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="gameProviderCode">The game provider code.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Delete /player/4ucmqplxb029/KY/logout
    /// </remarks>
    [HttpDelete("{playerCode}/{gameProviderCode}/logout", Name = "LogOutByGameProvider")]
    public ValueTask LogOutByGameProviderAsync(
        [FromServices] IMessageSender messageSender,
        string playerCode,
        string gameProviderCode)
    {
        var request = new PlayerLogOutRequest
        {
            PlayerCode = playerCode,
        };

        return messageSender.PublishAsync(
            subject: gameProviderCode.ToGamePlayerLogOutSubject(),
            data: request.ToByteArray(),
            cancellationToken: HttpContext.RequestAborted);
    }
}
