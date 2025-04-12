using CsTools.Extensions;
using CsTools.Functional;
using static CsTools.Functional.Memoization;

using static CsTools.Core;

namespace CsTools;

public static class Directory
{
    public static Func<string> GetHomeDir { get; }
    public static Func<string> GetDocumentsDir { get; }

    /// <summary>
    /// Ensures, that the given directory path exists, otherwise it creates the directory
    /// </summary>
    /// <param name="path"></param>
    /// <returns>path for chaining</returns>
    public static string EnsureDirectoryExists(this string path)
        => System.IO.Directory.Exists(path)
            ? path
            : path.SideEffect(p => System.IO.Directory.CreateDirectory(p));

    /// <summary>
    /// Ensures, that the given file's directory path exists, otherwise it creates the directory
    /// </summary>
    /// <param name="file"></param>
    /// <returns>file for chaining</returns>
    public static string EnsureFileDirectoryExists(this string file)
    {
        var info = new FileInfo(file);
        EnsureDirectoryExists(info.FullName);
        return file;
    }

    /// <summary>
    /// Checks if the given directory path exists, otherwise it creates the directory
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Result<string, DirectoryError> TryEnsureDirectoryExists(this string path)
    {
        try
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return path;
        }
        catch (UnauthorizedAccessException)
        {
            return Error<string, DirectoryError>(DirectoryError.AccessDenied);
        }
        catch (DirectoryNotFoundException)
        {
            return Error<string, DirectoryError>(DirectoryError.DirectoryNotFound);
        }
        catch (PathTooLongException)
        {
            return Error<string, DirectoryError>(DirectoryError.PathTooLong);
        }
        catch (NotSupportedException)
        {
            return Error<string, DirectoryError>(DirectoryError.NotSupported);
        }
        catch
        {
            return Error<string, DirectoryError>(DirectoryError.Unknown);
        }
    }        


    static Directory() 
    {
        GetHomeDir = Memoize(InitHomeDir);
        GetDocumentsDir = Memoize(InitDocumentsDir);
    } 
     
    static string InitHomeDir()
        => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    static string InitDocumentsDir()
        => Environment.GetFolderPath(Environment.SpecialFolder.Personal);
}
