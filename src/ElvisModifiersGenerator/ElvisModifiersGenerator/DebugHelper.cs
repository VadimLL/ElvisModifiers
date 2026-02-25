//#define FORCE_DEBUG

using System.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class DebugHelper
{
    /// <summary>
    /// Just for debug purposes
    /// </summary>
#if !FORCE_DEBUG
    [Conditional("DEBUG")]
#endif
    public static void Beep(int frequency = 500, int duration = 100)
    {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
        System.Console.Beep(frequency, duration);
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
    }

#if !FORCE_DEBUG
    [Conditional("DEBUG")]
#endif
    public static void Report(SourceProductionContext productionContext, string message)
    {
        //Debug.WriteLine(message);
        productionContext.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                "DebugReport",
                "DebugReport",
                message,
                "",
                DiagnosticSeverity.Warning,
                true),
            Location.None)
        );
    }
}
