using SvSim.Shared;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record UnbasedUnsizedIntegerLiteral : Expression
{
    public SvInt? Value;
};