namespace CsTools;

/// <summary>
/// Stream wrapper of a stream without Length to a stream with Length property set
/// </summary>
public class LengthStream(Stream innerStream, long length) : Stream
{
    public override void Close() => innerStream.Close();

    internal Stream innerStream = innerStream;

    #region Stream

    public override bool CanRead => innerStream.CanRead;

    public override bool CanSeek => innerStream.CanSeek;

    public override bool CanWrite => innerStream.CanWrite;

    public override long Length { get; } = length;

    public override long Position
    {
        get => innerStream.Position;
        set { }
    }

    public override void Flush() => innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => innerStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);

    public override void SetLength(long value) => innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => innerStream.Write(buffer, offset, count);

    #endregion
}

public static class LengthStreamExtensions
{
    public static Stream WithLength(this Stream stream, long length)
        => new LengthStream(stream, length);
}