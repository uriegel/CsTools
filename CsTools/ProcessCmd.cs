using LinqTools;
using CsTools.Extensions;

using static LinqTools.Core;
using static CsTools.Core;

namespace CsTools;

public static class ProcessCmd
{

    public static Task<Result<string, ProcessCmdException>> RunAsync(string fileName, string args)
        => Try(async () =>
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
                    .SideEffect(p => p.WaitForExitAsync());
                var responseString =
                    from n in await proc.StandardOutput.ReadToEndAsync()
                    select n.WhiteSpaceToNull();
                return responseString
                    ?? Error<string, ProcessCmdException>(new ProcessCmdException((await proc.StandardError.ReadToEndAsync()).WhiteSpaceToNull(), proc.ExitCode));
            }
        , e => Error<string, ProcessCmdException>(new ProcessCmdException(e)));
}