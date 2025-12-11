using SvSim.Shared;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record IntegerLiteral : SvExpression
{
    public SvInt? GetValue;
    public bool? IsDeclaredUnsized;
};