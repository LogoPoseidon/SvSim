namespace SvSim.SlangAstParser.Ast.Expressions;

public record StringLiteral : SvExpression
{
    public string? Value;
};