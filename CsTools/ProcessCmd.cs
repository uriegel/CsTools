using CsTools.Extensions;

namespace CsTools;

public static class ProcessCmd
{

    public static Task<string> RunAsync(string fileName, string args)
        => RawRunAsync(fileName, args)
            .MapException(e => new ProcessCmdException(e));


    static async Task<string> RawRunAsync(string fileName, string args)
    {
        var proc = await new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = fileName,
                Arguments = args,
                CreateNoWindow = true
            }
        }
            .SideEffect(p => p.Start())
            .SideEffectAsync(p => p.WaitForExitAsync());
        
        return 
            (await proc.StandardOutput.ReadToEndAsync()).WhiteSpaceToNull()
            ?? throw new ProcessCmdException((await proc.StandardError.ReadToEndAsync()).WhiteSpaceToNull(), proc.ExitCode);
    }
}
