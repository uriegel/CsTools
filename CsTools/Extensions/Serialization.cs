using System.Text.Json;

namespace CsTools.Extensions;

public static class SerializationExtensions
{
    public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null) 
        => JsonSerializer.Deserialize<T>(json, options);

    public static string Serialize<T>(this T json, JsonSerializerOptions? options = null) 
        => JsonSerializer.Serialize(json, options);
}