using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.Scope;
using SvSim.SlangAstParser.AstTree.Symbol;

namespace SvSim.SlangAstParser;

public record TopLevel
{
    [JsonPropertyName("design")]public required ISvScope Design { get; init; }
    [JsonPropertyName("definitions")]public required ISvSymbol[] Definitions { get; init; }
};