using System.Globalization;

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

    public static string ByteCountToString(this long byteCount, int decimalPlaces)
    {
        string format = "0." + new string('#', decimalPlaces);

        if (byteCount >= 1024L * 1024 * 1024)
            return $"{((double)byteCount / (1024 * 1024 * 1024)).ToString(format)} GB";

        if (byteCount >= 1024L * 1024)
            return $"{((double)byteCount / (1024 * 1024)).ToString(format)} MB";

        if (byteCount >= 1024)
            return $"{((double)byteCount / 1024).ToString(format)} KB";

        return $"{byteCount} B";
    }

    public static string FormatSeconds(this int secsString)
    {
        // TODO hours
        var secs = secsString % 60;
        var min = Math.Floor((double)secsString / 60);
        return $"{min:00}:{secs:00}";
    }
}            


