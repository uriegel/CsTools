using System.Text.Json;

using static CsTools.HttpRequest.Core;

namespace CsTools.HttpRequest;

class CustomJsonContent : HttpContent
{
    public Type ObjectType { get; }
    JsonSerializerOptions? JsonSerializerOptions { get; }
    public object Value { get; private set; }

    public CustomJsonContent(Type type, object value, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        ObjectType = type;
        JsonSerializerOptions = jsonSerializerOptions;
        Value = value;

        JsonSerializer.Serialize(stream, Value, ObjectType, JsonSerializerOptions);
        stream.Position = 0;
        Headers.ContentType = new("application/json");
    }

    protected override Task SerializeToStreamAsync(Stream stream, System.Net.TransportContext? context)
    {
        stream.CopyTo(stream);
        return Task.FromResult(0);
    }

    protected override bool TryComputeLength(out long length)
    {
        length = stream.Length;
        return true;
    }

    readonly MemoryStream stream = new();
}
