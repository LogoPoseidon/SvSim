using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record RangeSelectSvExpression : SvExpression
{
    public SvExpression? Value;
    public SvExpression? Left;
    public SvExpression? Right;
    public RangeSelectionKind? SelectionKind;
    public bool? ConstantSelect;
};