using System.Text.Json;
using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree;
using SvSim.SlangAstParser.AstTree.SvType;

namespace SvSim.SlangAstParser.Serializer;

public class TypeConverter : JsonConverter<IType>
{
    public override IType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("kind", out var kindProp))
            throw new JsonException("Missing 'kind' property");

        var kind = kindProp.GetString();

        return kind switch
        {
            "ScalarType"    => root.Deserialize<SvScalarType>(options),
            _               => throw new NotImplementedException($"Unknown kind, is not implemented yet: {kind}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IType value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}