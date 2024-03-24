using System.Runtime.InteropServices;
using System.Security;

namespace CsTools.Extensions;

public static class SecureStringExtensions
{
    public static string ReadSecureString(this SecureString secstr) 
    {
        var valuePtr = IntPtr.Zero;
        try 
        {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secstr);
            return Marshal.PtrToStringUni(valuePtr) ?? "";
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
        }
    }
}