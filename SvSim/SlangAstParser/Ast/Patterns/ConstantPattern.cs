using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Patterns;

public record ConstantPattern : SvPattern
{
    public SvExpression? Expr;
};