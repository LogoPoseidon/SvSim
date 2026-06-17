using System.Text.Json;
using System.Text.Json.Serialization;

namespace SvAstParser.Serializer;

public class PooledStringConverter : JsonConverter<string>
{
    private readonly Dictionary<string, string> _pool = new();

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str == null) return null;

        if (_pool.TryGetValue(str, out var pooledStr))
        {
            return pooledStr;
        }

        _pool[str] = str;
        return str;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}