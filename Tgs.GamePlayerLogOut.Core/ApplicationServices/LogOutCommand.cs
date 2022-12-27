namespace Tgs.GamePlayerLogOut.Core.ApplicationServices;

public record LogOutCommand(
    string GameProviderCode,
    string PlayerCode);
