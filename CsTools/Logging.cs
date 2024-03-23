namespace CsTools;

public enum LoggingKind
{
    GetEnvironmentVariable
}

public static class Logging
{
    public static void Use(LoggingKind loggingKind)
    {
        switch (loggingKind)
        {
            case LoggingKind.GetEnvironmentVariable:
                GetEnvironmentVariable = true;
                break;
        }
    }

    internal static bool GetEnvironmentVariable { get; private set; }
}