using CsTools.Extensions;

using var stream = Resources.Get("text/README");
using var file = File.Create("./test.md");
stream.CopyTo(file);
