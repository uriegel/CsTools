using System.Security;
using CsTools.Extensions;

using static System.Console;

namespace CsTools;

public static class Password
{
    public static SecureString ReadPassword()
    {
        static char[] ReadKey(char[] charList)
            => Console.ReadKey(true).KeyChar switch
            {
                '\r' or '\n' => charList.SideEffect(_ => WriteLine()),
                '\b' or '\u007f' => charList.Length > 0
                                    ? Unit.Value.SideEffect(_ =>
                                        {
                                            SetCursorPosition(CursorLeft - 1, CursorTop);
                                            Write(" ");
                                            SetCursorPosition(CursorLeft - 1, CursorTop);
                                        })
                                        .Pipe(_ => ReadKey(charList.Take(charList.Length - 1)
                                                    .ToArray()))
                                    : ReadKey([]),
                char chr => ReadKey([.. charList, chr.SideEffect(_ => Write('*'))])
            };

        var secStr = new SecureString();
        ReadKey([])
            .ForEach(secStr.AppendChar);
        return secStr;
    }
}