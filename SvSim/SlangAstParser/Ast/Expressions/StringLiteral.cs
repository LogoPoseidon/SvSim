namespace SvSim.SlangAstParser.Ast.Expressions;

public record StringLiteral : Expression
{
    public string? Value;
};