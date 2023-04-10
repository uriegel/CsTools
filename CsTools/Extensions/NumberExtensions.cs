namespace CsTools.Extensions;

public static class NumberExtensions
{
    /// <summary>
    /// Converts a Unix timestamp in milliseconds to a local DateTime value
    /// </summary>
    /// <param name="unixTimeInMilliseconds"></param>
    /// <returns></returns>
    public static DateTime FromUnixTime(this long unixTimeInMilliseconds)
        => DateTimeOffset
            .FromUnixTimeMilliseconds(unixTimeInMilliseconds)
            .LocalDateTime;
}

