using CsTools.Extensions;

namespace CsTools;

public static class ProcessCmd
{
    public static Task<string> RunAsync(string fileName, string args, InputParams inputParams)
        => RawRunAsync(fileName, args, inputParams)
            .MapException(e => new ProcessCmdException(e));

    public static Task<string> RunAsync(string fileName, string args)
        => RawRunAsync(fileName, args, null)
            .MapException(e => new ProcessCmdException(e));


    static async Task<string> RawRunAsync(string fileName, string args, InputParams? inputParams)
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

        if (inputParams?.ThrowOnStandardError == true)
        {
            var errorOutput = (await proc.StandardError.ReadToEndAsync()).WhiteSpaceToNull();    
            if (errorOutput != null)
                throw new ProcessCmdException(errorOutput, proc.ExitCode);    
        }
        if (inputParams?.ExitErrorOk.HasValue == true && inputParams.ExitErrorOk.Value != proc.ExitCode)
            throw new ProcessCmdException($"Error in running {fileName}, error code: {proc.ExitCode}", proc.ExitCode);
        return await proc.StandardOutput.ReadToEndAsync();
    }

    public record InputParams(int? ExitErrorOk, bool ThrowOnStandardError);
}


