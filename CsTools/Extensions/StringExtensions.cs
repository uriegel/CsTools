using static LinqTools.Core;

namespace CsTools.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Returns a substring after a found char, not including the char
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startChar"></param>
    /// <returns></returns>
    public static string SubstringAfter(this string? str, char startChar)
    {
        var posStart = str?.IndexOf(startChar) + 1 ?? -1;
        return posStart != -1 && posStart < str!.Length - 1
        ? str.Substring(posStart)
        : "";
    }

    /// <summary>
    /// Returns a substring, until a char in the string is found, not including the found char
    /// </summary>
    /// <param name="str"></param>
    /// <param name="endChar"></param>
    /// <returns></returns>
    public static string SubstringUntil(this string? str, char endChar)
    {
        var posEnd = str?.IndexOf(endChar) ?? 0;
        return posEnd > 0
        ? str!.Substring(0, posEnd)
        : str ?? "";
    }

    /// <summary>
    /// Combination of 'SubstringAfter' and 'SubstringUntil', returning a substring embedded between 'startChar and 'endChar'
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startChar"></param>
    /// <param name="endChar"></param>
    /// <returns></returns>
    public static string StringBetween(this string? str, char startChar, char endChar)
        => str
                ?.SubstringAfter(startChar)
                ?.SubstringUntil(endChar)
                ?? "";

    /// <summary>
    /// Parses a string to get an int value, returning null if parsing not possible
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int? ParseInt(this string? str)
        => int.TryParse(str, out var val)
            ? val
            : null;

    /// <summary>
    /// Parses a string to get a long value, returning null if parsing not possible
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long? ParseLong(this string? str)
        => long.TryParse(str, out var val)
            ? val
            : null;

    /// <summary>
    /// Functional way of calling String.IsNullOrWhiteSpace
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string? WhiteSpaceToNull(this string? str)
        => string.IsNullOrWhiteSpace(str) ? null : str;
    
    /// <summary>
    /// Retrieving an environment variable using this string as the key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string? GetEnvironmentVariable(this string key)
        => ExceptionToNull(() => Environment.GetEnvironmentVariable(key) ?? throw new Exception());
}            

