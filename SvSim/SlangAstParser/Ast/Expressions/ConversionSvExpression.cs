using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConversionSvExpression : SvExpression
{
    public bool? IsImplicit;
    public SvExpression? Operand;
    public ConversionKind? ConversionKind;
};