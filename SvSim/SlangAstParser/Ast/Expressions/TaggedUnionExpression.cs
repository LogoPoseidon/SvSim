using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TaggedUnionExpression : SvExpression
{
    public SvSymbol? Member;
    public SvExpression? ValueExpr;
};