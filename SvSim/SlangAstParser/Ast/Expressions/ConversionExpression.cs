using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConversionExpression : Expression
{
    public bool? IsImplicit;
    public Expression? Operand;
    public ConversionKind? ConversionKind;
};