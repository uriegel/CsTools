namespace CsTools;

/// <summary>
/// Stream wrapper to get a progress feedback when copying bytes to or from the underlying stream
/// </summary>
public class ProgressStream : Stream
{
    public ProgressStream(Stream innerStream, OnCopyProgress onProgress)
    {
        this.innerStream = innerStream;
        this.onProgress = onProgress;
    }

    public override void Close() => innerStream.Close();

    Stream innerStream;
    OnCopyProgress onProgress;

    #region Stream

    public override bool CanRead => innerStream.CanRead;

    public override bool CanSeek => innerStream.CanSeek;

    public override bool CanWrite => innerStream.CanWrite;

    public override long Length => innerStream.Length;

    public override long Position 
    { 
        get => innerStream.Position; 
        set => value = innerStream.Position; 
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

public delegate void OnCopyProgress(long total, long current);
