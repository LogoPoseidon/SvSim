using SvSim.Shared;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TimeLiteral : SvExpression
{
    public double? Value;
    public TimeScale? Scale;
};