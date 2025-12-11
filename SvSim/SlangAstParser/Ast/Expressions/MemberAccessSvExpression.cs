using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record MemberAccessSvExpression : SvExpression
{
    public SvExpression? Value;
    public SvSymbol? Member;
};