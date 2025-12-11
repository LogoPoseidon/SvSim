using SvSim.Shared;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record IntegerLiteral : Expression
{
    public SvInt? GetValue;
    public bool? IsDeclaredUnsized;
};