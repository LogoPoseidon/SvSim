using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvPorts;
using SvSim.SlangAstParser.AstTree.SvScope;

namespace SvSim.SlangAstParser.AstTree;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvDefinition), "Definition")]
[JsonDerivedType(typeof(SvPrimitive), "Primitive")]
[JsonDerivedType(typeof(SvPrimitivePort), "PrimitivePort")]
[JsonDerivedType(typeof(SvCompilationUnit), "CompilationUnit")]
public interface IKind
{
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("addr")] public long Address { get; init; }
}