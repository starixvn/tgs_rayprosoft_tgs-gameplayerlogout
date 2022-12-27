using System.Diagnostics;

namespace Tgs.GamePlayerLogOut.Client;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GamePlayerLogOut.Client.DiagnosticsInterceptor");
}
