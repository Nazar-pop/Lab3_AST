using System.Text.Json;
using NUnit.Framework;
using RestSharp;

namespace RestApiTests.Support;

/// <summary>Допоміжні методи для розбору JSON-відповідей з зрозумілими повідомленнями про помилки.</summary>
public static class JsonHelper
{
    public static JsonDocument Parse(RestResponse response)
    {
        var content = response.Content;
        if (string.IsNullOrWhiteSpace(content))
            throw new AssertionException($"Тіло відповіді порожнє (HTTP {(int)response.StatusCode}).");
        try
        {
            return JsonDocument.Parse(content);
        }
        catch (JsonException)
        {
            throw new AssertionException($"Відповідь не є JSON (HTTP {(int)response.StatusCode}): {content}");
        }
    }

    public static JsonElement Property(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(name, out var value))
            throw new AssertionException($"У відповіді немає поля \"{name}\": {element}");
        return value;
    }

    public static string String(JsonElement element, string name) =>
        Property(element, name).ToString();
}
