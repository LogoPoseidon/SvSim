using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record ExpressionConstraint : SvConstraint
{
    public SvExpression? Expr;
    public bool? IsSoft;
};