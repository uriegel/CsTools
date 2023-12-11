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

    public static string ByteCountToString(this long byteCounts, int decimalPlaces)
    {
        var gb = Math.Floor((double)byteCounts / (1024 * 1024 * 1024));
        var mb = byteCounts % (1024 * 1024 * 1024);
        if (gb >= 1.0)
            return $"{gb}{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}{mb.ToString()[0..decimalPlaces]} GB";
        var mb2 = Math.Floor((double)byteCounts / (1024 * 1024));
        var kb = byteCounts % (1024 * 1024);
        if (mb2 >= 1.0)
            return $"{mb2}{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}{kb.ToString()[0..decimalPlaces]} MB";
        var kb2 = Math.Floor((double)byteCounts / 1024);
        var b = byteCounts % 1024;
        if (kb2 >= 1.0)
            return $"{kb2}{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}{b.ToString()[0..decimalPlaces]} KB";
        else
            return $"{b} B";
    }

    public static string FormatSeconds(this int secsString)
    {
        // TODO hours
        var secs = secsString % 60;
        var min = Math.Floor((double)secsString / 60);
        return $"{min:00}:{secs:00}";
    }
}            


