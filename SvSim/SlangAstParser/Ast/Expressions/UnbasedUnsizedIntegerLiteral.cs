using SvSim.Shared;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record UnbasedUnsizedIntegerLiteral : SvExpression
{
    public SvInt? Value;
};