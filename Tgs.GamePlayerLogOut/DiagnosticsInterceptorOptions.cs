using System.Diagnostics;

namespace Tgs.GamePlayerLogOut;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GamePlayerLogOut.DiagnosticsInterceptor");
}
