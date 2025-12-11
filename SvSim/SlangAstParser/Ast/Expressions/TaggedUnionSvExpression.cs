using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TaggedUnionSvExpression : SvExpression
{
    public SvSymbol? Member;
    public SvExpression? ValueExpr;
};