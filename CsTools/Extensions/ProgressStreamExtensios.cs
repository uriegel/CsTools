namespace CsTools.Extensions;

public static class ProgressStreamExtensions
{
    public static ProgressStream WithProgress(this Stream stream, OnCopyProgress onProgress)
        => stream switch
        {
            LengthStream ls => new ProgressStream(ls, onProgress),
            Stream s => new ProgressStream(s, onProgress)
        };
}
