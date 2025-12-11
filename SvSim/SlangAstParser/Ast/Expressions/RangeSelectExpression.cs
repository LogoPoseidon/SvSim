using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record RangeSelectExpression : Expression
{
    public Expression? Value;
    public Expression? Left;
    public Expression? Right;
    public RangeSelectionKind? SelectionKind;
    public bool? ConstantSelect;
};