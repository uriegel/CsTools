using System.Reflection;

namespace CsTools.Extensions;

/// <summary>
/// 
/// </summary>Extensio Methods for retrieving resources
public static class Resources
{
    /// <summary>
    /// Get a stream from resource
    /// </summary>
    /// <param name="assembly">Assembly containing the desired resource</param>
    /// <param name="path">Path to the resource</param>
    /// <returns>A resource stream</returns>
    public static Stream Get(this Assembly assembly, string path)
        => assembly.GetManifestResourceStream(path);

    public static Stream Get(string path)
        => Assembly
            .GetEntryAssembly()
            .GetManifestResourceStream(path);

    public static Stream GetFromThis(string path)
        => Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream(path);
}