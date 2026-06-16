using System.Text.Json.Serialization;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol.TempVarSymbol;

namespace SvAstParser.AstTree.Pattern;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvInvalid), nameof(SvPatternKind.Invalid))]
[JsonDerivedType(typeof(SvConstant), nameof(SvPatternKind.Constant))]
[JsonDerivedType(typeof(SvWildcard), nameof(SvPatternKind.Wildcard))]
[JsonDerivedType(typeof(SvTagged), nameof(SvPatternKind.Tagged))]
[JsonDerivedType(typeof(SvVariable), nameof(SvPatternKind.Variable))]
[JsonDerivedType(typeof(SvStructure), nameof(SvPatternKind.Structure))]

public interface ISvPattern : ISvAstNode { }
public record SvConstant : ISvPattern
{
    public required ISvExpression Expr { get; init; }
    public string? Kind { get; init; }
}

public record SvInvalid : ISvPattern
{
    public required ISvPattern Child { get; init; }
    public string? Kind { get; init; }
}

public record SvStructure : ISvPattern
{
    public required StructurePatternField[] Patterns { get; init; }
    public string? Kind { get; init; }
}
public record StructurePatternField(string Field, ISvPattern Pattern);

public record SvTagged : ISvPattern
{
    public required string Member { get; init; }
    public required ISvPattern ValuePattern { get; init; }
    public string? Kind { get; init; }
}
public record SvVariable : ISvPattern
{
    public required SvPatternVar Variable { get; init; }
    public string? Kind { get; init; }
}
public record SvWildcard : ISvPattern
{
    public string? Kind { get; init; }
}




