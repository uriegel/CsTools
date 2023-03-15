namespace CsTools;

public class ProcessCmdException : Exception
{
    public int? ExitCode{ get; }

    internal ProcessCmdException(string? error, int code) 
        : base(error)
        => ExitCode = code;

    internal ProcessCmdException(Exception e)
        : base(e.Message)
        => exception = e;

    public override string ToString()
    => exception?.ToString() ?? Message;

    Exception? exception;
}
