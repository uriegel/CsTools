namespace CsTools;

using CsTools.Extensions;
using static CsTools.Functional.Memoization;

public static partial class Core
{
    static Func<string, string?, string?> Init {get; } 
        = (key, _) => Logging.GetEnvironmentVariable 
            ? key.GetEnvironmentVariableWithLogging()
            : key.GetEnvironmentVariable();

    public static Func<string, string?> GetEnvironmentVariable { get; }
        = Memoize(Init, false);

    static string? GetEnvironmentVariableWithLogging(this string key)
        => key
            .GetEnvironmentVariable()
            ?.SideEffect(v => Console.WriteLine($"Reading environment {key}: {v}"));
}
