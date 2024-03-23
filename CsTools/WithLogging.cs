namespace CsTools;

using CsTools.Extensions;
using static CsTools.Functional.Memoization;

public static partial class WithLogging
{
    static Func<string, string?, string?> Init {get; } 
        = (key, _) => key.GetEnvironmentVariableWithLogging();

    /// <summary>
    /// Gets an environment variable and otherwise then string.GetEnvironmentVariable() logs key and value to the console
    /// </summary>
    public static Func<string, string?> GetEnvironmentVariable { get; }
        = Memoize(Init, false);

    static string? GetEnvironmentVariableWithLogging(this string key)
        => key
            .GetEnvironmentVariable()
            ?.SideEffect(v => Console.WriteLine($"Reading environment {key}: {v}"));
}
