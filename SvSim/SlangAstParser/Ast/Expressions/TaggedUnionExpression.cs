using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TaggedUnionExpression : Expression
{
    public SvSymbol? Member;
    public Expression? ValueExpr;
};