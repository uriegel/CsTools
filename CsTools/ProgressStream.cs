namespace CsTools;

/// <summary>
/// Stream wrapper to get a progress feedback when copying bytes to or from the underlying stream
/// </summary>
public class ProgressStream(Stream innerStream, OnCopyProgress onProgress) : Stream
{
    public override void Close() => innerStream.Close();

    readonly Stream innerStream = innerStream;
    readonly OnCopyProgress onProgress = onProgress;

    #region Stream

    public override bool CanRead => innerStream.CanRead;

    public override bool CanSeek => innerStream.CanSeek;

    public override bool CanWrite => innerStream.CanWrite;

    public override long Length => innerStream.Length;

    public override long Position 
    { 
        get => innerStream.Position; 
        set {}
    }

    public override void Flush() => innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var pos = innerStream.Position;
        var read = innerStream.Read(buffer, offset, count);
        onProgress(innerStream.Length, pos + read);
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);

    public override void SetLength(long value) => innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        var pos = innerStream.Position;
        innerStream.Write(buffer, offset, count);
        onProgress(innerStream.Length, pos + count);
    }

    #endregion
}

public static class ProgressStreamExtensions
{
    public static ProgressStream WithProgress(this Stream stream, OnCopyProgress onProgress)
        => stream switch
        {
            LengthStream ls => new ProgressStream(ls, onProgress),
            Stream s => new ProgressStream(s, onProgress)
        };
}

public delegate void OnCopyProgress(long total, long current);
