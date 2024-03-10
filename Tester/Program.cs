using CsTools;
using CsTools.Extensions;
using CsTools.HttpRequest;
using System.Net.Http.Json;

using static System.Console;
using static CsTools.Functional.Tree;
using static CsTools.ProcessCmd;
using static CsTools.HttpRequest.Core;
using CsTools.Functional;

using static CsTools.Core;
using System.Text.Json;

JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Case-insensitive (camelCase)
        };

// ===================== test android server

for (var i = 0; i < 30; i++)
{
    var msgp = await Request.RunAsync(DefaultSettings with
                {
                    Method = HttpMethod.Post,
                    // BaseUrl = "http://localhost:2000",
                    // Url = "/file/post",
                    BaseUrl = "http://192.168.178.74:8080",
                    Url = "/postfile/Pictures/affe.jpg",
                    AddContent = () => new StreamContent(File.OpenRead("/home/uwe/Urlaub/20230911_141054.jpg"), 8100)
                }, false);
    WriteLine($"Der Code: {msgp.StatusCode}");
}


var jsonPostRequest = new JsonRequest("http://192.168.178.74:8080");

// ===================== test android server

var jsonGetRequest = new JsonRequest("http://192.168.178.74:8080");
var result2 = jsonGetRequest.Get<FileType[]>("getfiles");
var res22 = await result2.ToResult();

var settings = DefaultSettings with
        {
            Method = HttpMethod.Post,
            BaseUrl = $"http://192.168.178.74:8080",
            Url = "/remote/getfile",
            AddContent = () => JsonContent.Create(new 
                                { 
                                    Path = "/Download/VID_20230919_162741.mp4"
                                })
        };

var httpmsg = await Request.Run(settings, true)
    .SideEffectWhenError(e => WriteLine($"Request error: {e}"))
    .BindAwait(msg => msg.UseAwait(
        msg => 
            msg.Pipe(m => m.Content.Headers.ContentLength)
                .Pipe(len => 
            File
                .Create("targetFilename")
                .WithProgress((t, c) => WriteLine($"Copy progress: {c}/{len}"))
                .UseAwait(
            target => msg.CopyToStream(target)))))
    .ToResult();

ReadLine();


Settings PostFile(Stream streamToPost, DateTime lastWriteTime)
    => DefaultSettings with
        {
            Method = HttpMethod.Post,
            BaseUrl = $"http://192.168.178.74:8080",
            Url = "/remote/postfile?path=/Download/test.mp4",
            Timeout = 100_000_000,
            AddContent = () => new StreamContent(streamToPost, 8100)
                                    .SideEffect(n => n  
                                                        .Headers
                                                        .TryAddWithoutValidation(
                                                            "x-file-date", 
                                                            new DateTimeOffset(lastWriteTime).ToUnixTimeMilliseconds().ToString()))
        };

var httpRes = await Request.Run(PostFile(
    File
        .OpenRead("/speicher/Videos/Ali.mkv")
        .WithProgress((t, c) => WriteLine($"Copy progress: {c}/{t}")), 
    DateTime.Now - TimeSpan.FromDays(7)), true)
    .SideEffectWhenError(e => WriteLine($"Request error: {e}"))
    .ToResult();

ReadLine();

var pipeRes = 
    2
        .Pipe(n => n + 8)
        .Pipe(n => n / 2)
        .Pipe(n => n.ToString())
        .Pipe(s => "Das Ergebnis: " + s);

var t = 7.And(
    n => n < 16,
    n => n % 2 == 1,
    n => n > 4);

t = 4.And(
    n => n < 16,
    n => n % 2 == 1,
    n => n > 4);

t = 3.And(
    n => n < 16,
    n => n % 2 == 1,
    n => n > 4);

var resOk = Ok<string, int>("Das ist das Gute");
var resErr = Error<string, int>(9876);

Result<ResultType, string> Transform(Result<string, int> input)
    => input
        .SelectError(err => $"Error occurred: {err}")
        .Select(ok => new ResultType(ok, 999));

Transform(resOk).Match(
    ok => WriteLine($"Ok: {ok}"),
    err => WriteLine($"Err: {err}"));
Transform(resErr).Match(
    ok => WriteLine($"Ok: {ok}"),
    err => WriteLine($"Err: {err}"));

var jsonRequest = new JsonRequest("http://localhost:2000/requests");
var res = await jsonRequest
                .Post<Request2, ResultType>(new("req2", new("Uwe Riegel", 9865)))
                .ToResult();
ShowResult("Without connection", res);

var jsonRequestU = new JsonRequest("http://unknownhost:2000/requests");
res = await jsonRequestU
                .Post<Request2, ResultType>(new("req2", new("Uwe Riegel", 9865)))
                .ToResult();
ShowResult("Unknown host", res);

WriteLine("Please start program 'Tester' from 'https://github.com/uriegel/AspNetExtensions', then press 'enter'");
ReadLine();

var sse1 = new SseClient("http://localhost:2000/sse/test", WriteLine);
var sse2 = new SseClient<Event>("http://localhost:2000/sse/test", msg => WriteLine(msg.Content));

res = await jsonRequest
                .Post<Request2, ResultType>(new("req2", new("Uwe Riegel", 9865)))
                .ToResult();
ShowResult("With connection", res);

res = await jsonRequest
                .Post<RequestWrong, ResultType>(new("req2", new("Uwe Riegel", true)))
                .ToResult();
ShowResult("Wrong type", res);

var res2 = await jsonRequest
                .Post<Request2, WrongResultType>(new("req2", new("Uwe Riegel", 9865)))
                .ToResult();
ShowResult("Wrong target type", res);

res = await jsonRequest
                .Post<Request2, ResultType>(new("wrongreq", new("Uwe Riegel", 1234)))
                .ToResult();
ShowResult("Wrong request", res);

void ShowResult<T>(string text, Result<T, RequestError> result)
    where T: notnull
    => res.Match(
        ok => WriteLine($"Ok {text}: {ok}"),
        err => WriteLine($"Error {text}: {err.Status} {err.StatusText}"));

ReadLine();

sse1.Dispose();
sse2.Dispose();

ReadLine();

int Divide(int a, int d)
    => a / d;

var home = CsTools.Directory.GetHomeDir();
var docs = CsTools.Directory.GetDocumentsDir();

var dict = new Dictionary<string, int>
    {
        { "111", 1 },
        { "112", 1234 },
        { "113", 4321 }
    };

var val = dict.TryGetValue("112");
var val2 = dict.TryGetValue("115");

var dict2 = new Dictionary<int, string>
    {
        { 111, "1" },
        { 112, "1234" },
        { 113, "4321" }
    };

var val3= dict2.GetValue(112);
var val4 = dict2.GetValue(115);

#pragma warning disable 1998
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

ReadLine();

record FileItem(string Path, long Size);

record FileType(string Name, bool IsDirectory, long Size, bool IsHidden, long Time);

record Request2(string Name, int Id);
record RequestWrong(string NoName, bool Id);
record ResultType(string Result, int Id);
record WrongResultType(string NoResult, DateTime Id);

record Event(string Content);