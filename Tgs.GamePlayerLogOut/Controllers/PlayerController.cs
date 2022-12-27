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
    ///     Delete /4ucmqplxb029
    /// </remarks>
    [HttpDelete("{playerCode}", Name = "LogOut")]
    public ValueTask LogOutAsync(
        [FromServices] ICommandService<LogOutMessageCommand> service,
        string playerCode)
        => service.ExecuteAsync(
            new LogOutMessageCommand(
                playerCode),
            HttpContext.RequestAborted);

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
    ///     Delete /4ucmqplxb029/AWC_SG
    /// </remarks>
    [HttpDelete("{playerCode}/{gameProviderCode}", Name = "LogOutByGameProvider")]
    public ValueTask LogOutByGameProviderAsync(
        [FromServices] ICommandService<LogOutGameProviderMessageCommand> service,
        string playerCode,
        string gameProviderCode)
        => service.ExecuteAsync(
            new LogOutGameProviderMessageCommand(
                playerCode,
                gameProviderCode),
            HttpContext.RequestAborted);
}
