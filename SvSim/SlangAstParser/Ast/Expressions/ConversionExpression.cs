using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConversionExpression : SvExpression
{
    public bool? IsImplicit;
    public SvExpression? Operand;
    public ConversionKind? ConversionKind;
};