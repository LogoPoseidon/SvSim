using System.Text.Json;
using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree;
using SvSim.SlangAstParser.AstTree.SvPorts;
using SvSim.SlangAstParser.AstTree.SvScope;

namespace SvSim.SlangAstParser.Serializer;

public class KindConverter : JsonConverter<IKind>
{
    public override IKind? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("kind", out var kindProp))
            throw new JsonException("Missing 'kind' property");

        var kind = kindProp.GetString();

        return kind switch
        {
            "Definition"    => root.Deserialize<SvDefinition>(options),
            "Primitive"     => root.Deserialize<SvPrimitive>(options),
            "PrimitivePort"  => root.Deserialize<SvPrimitivePort>(options),
            "CompilationUnit" => root.Deserialize<SvCompilationUnit>(options),
            _               => throw new NotImplementedException($"Unknown kind, is not implemented yet: {kind}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IKind value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}