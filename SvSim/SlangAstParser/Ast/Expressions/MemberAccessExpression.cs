using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record MemberAccessExpression : SvExpression
{
    public SvExpression? Value;
    public SvSymbol? Member;
};