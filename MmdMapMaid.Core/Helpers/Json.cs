using System.Text.Json;

namespace MmdMapMaid.Core.Helpers;

public static class Json
{
    public static async Task<T?> ToObjectAsync<T>(string value, JsonSerializerOptions? options = null) =>
        await Task.Run(() => JsonSerializer.Deserialize<T>(value, options));

    public static T? ToObject<T>(string value, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(value, options);

    public static async Task<string> StringifyAsync(object? value, JsonSerializerOptions? options = null) =>
        await Task.Run(() => JsonSerializer.Serialize(value, options));

    public static string Stringify(object? value, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(value, options);
}
