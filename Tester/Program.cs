using CsTools;
using CsTools.Extensions;

using static CsTools.ProcessCmd;

using var stream = Resources.Get("text/README");
using var file = File.Create("./test.md");
stream?.CopyTo(file);

var result = await RunAsync("lsblkd", "--bytes --output SIZE,NAME,LABEL,MOUNTPOINT,FSTYPE");
result = await RunAsync("lsblk", "--nothing -bytes -output SIZE,NAME,LABEL,MOUNTPOINT,FSTYPE");
result = await RunAsync("lsblk", "--bytes --output SIZE,NAME,LABEL,MOUNTPOINT,FSTYPE");

using var testerFile = new ProgressStream(
    File.OpenRead(Environment.CurrentDirectory.AppendPath(@"Tester/bin/Debug/net6.0/Tester")), 
    (total, current) => Console.WriteLine($"Total: {total}, current: {current}"));
using var testerCopy = File.Create(Environment.CurrentDirectory.AppendPath(@"TesterCopy"));
testerFile.CopyTo(testerCopy);



Console.ReadLine();