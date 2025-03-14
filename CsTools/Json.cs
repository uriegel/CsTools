using System.Text.Json;
using System.Text.Json.Serialization;

public static class Json
{
    public static JsonSerializerOptions Defaults { get; }
        = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
}