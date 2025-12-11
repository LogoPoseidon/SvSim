using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ExpressionStatement : SvStatement
{
    public SvExpression? Expr;
};