namespace SvSim.SlangAstParser.Ast.Expressions;

public record RealLiteral : Expression
{
    public double? Value;
};