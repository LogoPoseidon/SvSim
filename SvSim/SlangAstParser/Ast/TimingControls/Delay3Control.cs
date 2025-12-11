using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record Delay3Control : TimingControl
{
    public SvExpression? Expr1;
    public SvExpression? Expr2;
    public SvExpression? Expr3;
};