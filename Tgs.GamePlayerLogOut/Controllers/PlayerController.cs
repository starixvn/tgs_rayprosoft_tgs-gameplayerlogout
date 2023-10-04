using Microsoft.AspNetCore.Mvc;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerLogOut.Controllers;

[ApiController]
[Route("player")]
public class PlayerController : ControllerBase
{
    /// <summary>
    /// Logs the out asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Delete /player/4ucmqplxb029/logout
    /// </remarks>
    [HttpDelete("{playerCode}/logout", Name = "LogOut")]
    public ValueTask LogOutAsync(
        [FromServices] ICommandService<LogOutMessageCommand> service,
        string playerCode)
        => service.ExecuteAsync(
            command: new LogOutMessageCommand(
                PlayerCode: playerCode),
            cancellationToken: HttpContext.RequestAborted);

    /// <summary>
    /// Logs the out by game provider asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
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
        [FromServices] ICommandService<LogOutGameProviderMessageCommand> service,
        string playerCode,
        string gameProviderCode)
        => service.ExecuteAsync(
            command: new LogOutGameProviderMessageCommand(
                PlayerCode: playerCode,
                GameProviderCode: gameProviderCode),
            cancellationToken: HttpContext.RequestAborted);
}
