#if DEBUG
namespace QuoteCraft.Helpers;

/// <summary>
/// Debug-only command-line hooks for the screenshot matrix (docs/evidence): <c>--qc-data=&lt;folder&gt;</c>,
/// <c>--qc-seed=sample</c>, <c>--qc-theme=light|dark</c>, <c>--qc-route=&lt;route&gt;[:detail]</c>,
/// <c>--qc-focus=log</c> and <c>--qc-fault=load</c>.
/// Release builds never read them.
/// </summary>
public static class VerificationArgs
{
    public static string? Get(string name) => Environment.GetCommandLineArgs()
        .FirstOrDefault(a => a.StartsWith($"--qc-{name}=", StringComparison.OrdinalIgnoreCase))?
        .Split('=', 2)[1];

    public static bool Any => Environment.GetCommandLineArgs().Any(a => a.StartsWith("--qc-", StringComparison.OrdinalIgnoreCase));

    public static void Log(string message) => Console.Error.WriteLine($"VERIFY {message}");
}
#endif
