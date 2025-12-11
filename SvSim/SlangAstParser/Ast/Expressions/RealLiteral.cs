namespace SvSim.SlangAstParser.Ast.Expressions;

public record RealLiteral : SvExpression
{
    public double? Value;
};