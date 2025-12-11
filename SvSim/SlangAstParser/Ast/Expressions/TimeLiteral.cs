using SvSim.Shared;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TimeLiteral : Expression
{
    public double? Value;
    public TimeScale? Scale;
};