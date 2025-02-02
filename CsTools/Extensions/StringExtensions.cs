using System.Globalization;
using CsTools.Functional;

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
        ? str[posStart..]
        : "";
    }

    /// <summary>
    /// Returns a substring after a found substring, not including the substring
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startStr"></param>
    /// <returns></returns>
    public static string SubstringAfter(this string? str, string startStr)
    {
        var posStart = str?.IndexOf(startStr) + startStr.Length ?? -1;
        return posStart != -1 && posStart < str!.Length - 1
        ? str[posStart..]
        : "";
    }

    /// <summary>
    /// Returns a substring after a found last char, not including the char
    /// </summary>
    /// <param name="str"></param>
    /// <param name="lastStartChar"></param>
    /// <returns></returns>
    public static string SubstringAfterLast(this string? str, char lastStartChar)
    {
        var posStart = str?.LastIndexOf(lastStartChar) + 1 ?? -1;
        return posStart != -1 && posStart < str!.Length - 1
        ? str[posStart..]
        : "";
    }

    /// <summary>
    /// Returns a substring after a found last substring, not including the substring
    /// </summary>
    /// <param name="str"></param>
    /// <param name="lastStartStr"></param>
    /// <returns></returns>
    public static string SubstringAfterLast(this string? str, string lastStartStr)
    {
        var posStart = str?.LastIndexOf(lastStartStr) + lastStartStr.Length ?? -1;
        return posStart != -1 && posStart < str!.Length - 1
        ? str[posStart..]
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
        ? str![..posEnd]
        : str ?? "";
    }

    /// <summary>
    /// Returns a substring, until a substring in the string is found, not including the found substring
    /// </summary>
    /// <param name="str"></param>
    /// <param name="endStr"></param>
    /// <returns></returns>
    public static string SubstringUntil(this string? str, string endStr)
    {
        var posEnd = str?.IndexOf(endStr) ?? 0;
        return posEnd > 0
        ? str![..posEnd]
        : str ?? "";
    }

    /// <summary>
    /// Returns a substring, until a char in the string is found, not including the found char
    /// </summary>
    /// <param name="str"></param>
    /// <param name="lastEndChar"></param>
    /// <returns></returns>
    public static string SubstringUntilLast(this string? str, char lastEndChar)
    {
        var posEnd = str?.LastIndexOf(lastEndChar) ?? 0;
        return posEnd > 0
        ? str![..posEnd]
        : str ?? "";
    }

    /// <summary>
    /// Returns a substring, until a substring in the string is found, not including the found substring
    /// </summary>
    /// <param name="str"></param>
    /// <param name="lastEndStr"></param>
    /// <returns></returns>
    public static string SubstringUntilLast(this string? str, string lastEndStr)
    {
        var posEnd = str?.LastIndexOf(lastEndStr) ?? 0;
        return posEnd > 0
        ? str![..posEnd]
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
    /// Combination of 'SubstringAfter' and 'SubstringUntil', returning a substring embedded between 'startStr and 'endStr'
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startStr"></param>
    /// <param name="endStr"></param>
    /// <returns></returns>
    public static string StringBetween(this string? str, string startStr, string endStr)
        => str
                ?.SubstringAfter(startStr)
                ?.SubstringUntil(endStr)
                ?? "";

    /// <summary>
    /// Parses a string to get an int value, returning None if parsing is not possible
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int? ParseInt(this string str)
        => int.TryParse(str, out var val)
            ? val
            : null;

    /// <summary>
    /// Parses a string to get a long value, returning None if parsing is not possible
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long? ParseLong(this string str)
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
    public static string? GetEnvironmentVariable(this string key)
    {
        try
        {
            return Environment.GetEnvironmentVariable(key);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Appends the given subpath to this path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="subPath"></param>
    /// <returns></returns>
    public static string AppendPath(this string? path, string? subPath)
        => path != null
                ? subPath != null
                    ? Path.Combine(path, subPath)
                    : path
                : subPath ?? "";

    /// <summary>
    /// Creates a file from this path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Stream CreateFile(this string path)
        => File.Create(path);

    /// <summary>
    /// Opens a file readonly from this path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Stream OpenFile(this string path)
        => File.OpenRead(path);

    /// <summary>
    /// Reads all text from a text file (Utf8)
    /// </summary>
    /// <param name="path">Full path to the text file</param>
    /// <returns>Containing text</returns>
    public static string? ReadAllTextFromFilePath(this string path)
        => File.Exists(path)
            ? new StreamReader(File.OpenRead(path))
                .Use(f => f.ReadToEnd())
            : null;

    /// <summary>
    /// Writes all text from to a file (Utf8)
    /// </summary>
    /// <param name="path">Full path to the text file</param>
    /// <param name="text">Text to write</param>
    public static void WriteAllTextToFilePath(this string path, string text)
        => File
            .CreateText(path)
            .Use(str => str.Write(text));

    /// <summary>
    /// Converting a DateTime string to a DateTime value if possible, otherwise returns null
    /// </summary>
    /// <param name="dateTimeStr"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static DateTime? ToDateTime(this string dateTimeStr, string format)
        => ToDateTime(dateTimeStr, format, CultureInfo.InvariantCulture);

    /// <summary>
    /// Converting a DateTime string to a DateTime value if possible, otherwise returns null
    /// </summary>
    /// <param name="dateTimeStr"></param>
    /// <param name="format"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static  DateTime? ToDateTime(this string dateTimeStr, string format, CultureInfo culture)
        => DateTime.TryParseExact(dateTimeStr?.TrimEnd('\0') ?? "", format, 
                culture, DateTimeStyles.None, out var dt)
            ? dt
            : null;

    /// <summary>
    /// Gets the extension of a file path, returning null if it doesn't contain one
    /// </summary>
    /// <param name="file">The path to the file</param>
    /// <returns>The extension or null</returns>
    public static string? GetFileExtension(this string file)
    {
        var posStart = file.LastIndexOf('.');
        return posStart != -1 
        ? file[posStart..]
        : null;
    }

    /// <summary>
    /// Gets the MIME Type of a file extension, or null, if it could not be determined
    /// </summary>
    /// <param name="extension">The file extension, with or without leading '.'</param>
    /// <returns>null</returns>
    public static string? ToMimeType(this string extension)
        => MimeType.Get(extension);
}            

