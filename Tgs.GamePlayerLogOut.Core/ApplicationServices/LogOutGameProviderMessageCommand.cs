namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

public record LogOutGameProviderMessageCommand(
    string PlayerCode,
    string GameProviderCode);
