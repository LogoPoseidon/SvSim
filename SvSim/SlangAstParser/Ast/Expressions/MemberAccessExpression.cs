using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record MemberAccessExpression : Expression
{
    public Expression? Value;
    public SvSymbol? Member;
};