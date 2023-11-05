using CsTools;
using CsTools.Extensions;
using CsTools.HttpRequest;
using static CsTools.ProcessCmd;
using static CsTools.HttpRequest.Core;
using System.Net.Http.Json;
using LinqTools;

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