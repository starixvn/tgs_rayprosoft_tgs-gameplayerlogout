using Grpc.Core;
using Tgs.GamePlayerLogOut.Core.ApplicationServices;
using Tgs.GamePlayerLogOut.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerLogOut.GrpcServices;

public class GamePlayerLogOutGrpcService : GamePlayerLogOutService.GamePlayerLogOutServiceBase
{
    private readonly ICommandService<LogOutGameProviderMessageCommand> _logOutGameProviderService;
    private readonly ICommandService<LogOutMessageCommand> _logOutService;

    public GamePlayerLogOutGrpcService(
        ICommandService<LogOutMessageCommand> logOutService,
        ICommandService<LogOutGameProviderMessageCommand> logOutGameProviderService
        )
    {
        _logOutService = logOutService ?? throw new ArgumentNullException(nameof(logOutService));
        _logOutGameProviderService = logOutGameProviderService ?? throw new ArgumentNullException(nameof(logOutGameProviderService));
    }

    public override async Task<BaseReply> LogOut(LogOutRequest request, ServerCallContext context)
    {
        var command = new LogOutMessageCommand(
            PlayerCode: request.PlayerCode);

        await _logOutService.ExecuteAsync(
            command: command,
             cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            Success = true
        };
    }

    public override async Task<BaseReply> LogOutByGameProvider(LogOutByGameProviderRequest request, ServerCallContext context)
    {
        var command = new LogOutGameProviderMessageCommand(
                PlayerCode: request.PlayerCode,
                GameProviderCode: request.GameProviderCode);

        await _logOutGameProviderService.ExecuteAsync(
            command: command,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            Success = true
        };
    }
}
