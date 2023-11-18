using LinqTools;

using static CsTools.Functional.Memoization;

namespace CsTools;

public static class Directory
{
    public static Func<string> GetHomeDir { get; }

    public static string EnsureDirectoryExists(this string path)
        => System.IO.Directory.Exists(path)
            ? path
            : path.SideEffect(p => System.IO.Directory.CreateDirectory(p));

    static Directory()
        => GetHomeDir = Memoize(InitHomeDir);

    static string InitHomeDir()
        => Environment.GetFolderPath(Environment.SpecialFolder.Personal);
}
