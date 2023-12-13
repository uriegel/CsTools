using CsTools;
using CsTools.Extensions;
using CsTools.HttpRequest;
using static CsTools.ProcessCmd;
using static CsTools.HttpRequest.Core;
using System.Net.Http.Json;

using static CsTools.Functional.Tree;
using LinqTools;

int Divide(int a, int d)
    => a / d;

var home = CsTools.Directory.GetHomeDir();
var docs = CsTools.Directory.GetDocumentsDir();

async Task<int> MakeAsync(Func<int> syncFunc)
    => syncFunc();

var testOk = await MakeAsync(() => Divide(16, 4))
                    .Catch(e => 0.SideEffect(_ => Console.WriteLine($"catched: {e}")));    

var testE = await MakeAsync(() => Divide(16, 0))
                    .Catch(e => 0.SideEffect(_ => Console.WriteLine($"catched: {e}")));    

var test = new[] { "iexplore.exe", "de-DE", "en-US", "ieinstal.exe" }
            .FlattenTree(Resolver, CreateFileItem, IsSubtree, null, AppendPath, @"C:\Program Files\Internet Explorer");

bool IsSubtree(string path, string? subPath)
    => (File.GetAttributes(subPath.AppendPath(path)) & FileAttributes.Directory) == FileAttributes.Directory;

(IEnumerable<string>, string?) Resolver(string item, string? subPath)
{
    var path = subPath?.AppendPath(item) ?? item;
    var dirItems = GetSafe(() => new DirectoryInfo(path).GetDirectories().Select(n => n.Name));
    var fileItems = GetSafe(() => new DirectoryInfo(path).GetFiles().Select(n => n.Name));
    var all = fileItems.Concat(dirItems);
    return (all, item);
}

IEnumerable<string> GetSafe(Func<IEnumerable<string>> func)
{
    try 
    {
        return func();
    }   
    catch 
    {
        return Enumerable.Empty<string>();
    }
}

string AppendPath(string? initialPath, string? subPath)
    => initialPath.AppendPath(subPath);

FileItem CreateFileItem(string path, string? subPath)
{
    var info = new FileInfo(subPath!.AppendPath(path));
    return new(info.FullName, info.Length);
}

var test2 = new[] { @"C:\Users\urieg\Neu" }
            .FlattenTree(Resolver, CreateFileItem, IsSubtree, null, AppendPath, (string?)null);

var cts = new CancellationTokenSource(1200);
var test3 = new[] { @"C:\windows\system32" }
            .FlattenTree(Resolver, CreateFileItem, IsSubtree, cts.Token, AppendPath, (string?)null);


var test4 = new[] { @"C:\windows\system32" }
            .FlattenTree(Resolver, CreateFileItem, IsSubtree, null, AppendPath, (string?)null);

using var stream = Resources.Get("text/README");
using var file = File.Create("./test.md");
stream?.CopyTo(file);

var result = await RunAsync("lsblk", "--bytes --output SIZE,NAME,LABEL,MOUNTPOINT,FSTYPE");
result = await RunAsync("lsblk", "--nothing -bytes -output SIZE,NAME,LABEL,MOUNTPOINT,FSTYPE")
            .Catch(e => e.ToString());
result = await RunAsync("lsblk", "--bytes --output SIZE,NAME,LABEL,MOUNTPOINT,FSTYPE");

using var testerFile = new ProgressStream(
    File.OpenRead(Environment.CurrentDirectory.AppendPath(@"Tester/bin/Debug/net6.0/Tester")), 
    (total, current) => Console.WriteLine($"Total: {total}, current: {current}"));
using var testerCopy = File.Create(Environment.CurrentDirectory.AppendPath(@"TesterCopy"));
testerFile.CopyTo(testerCopy);

using var msg = await Request.RunAsync(DefaultSettings with
    {
        Method = HttpMethod.Post,
        BaseUrl = $"http://192.168.178.74:8080",
        Url = "/getfile",
        AddContent = () => JsonContent.Create(new { Path = "/DCIM/Camera/20230210_170241.mp4" })
    }, true);
using var targetFile = File.Create("/home/uwe/test.mp4")
    .WithProgress((t, c) => Console.WriteLine($"{c}, {msg.Content.Headers.ContentLength}"));

await msg 
    .Content
    .ReadAsStream()
    .CopyToAsync(targetFile);

Console.ReadLine();

record FileItem(string Path, long Size);