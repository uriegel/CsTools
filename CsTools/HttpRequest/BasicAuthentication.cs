using System.Text;
using CsTools.HttpRequest;

namespace CsTools.Functional;

public static class BasicAuthentication
{
    public static Header From(string name, string password)
        => new ("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(name + ":" + password))}");
}


