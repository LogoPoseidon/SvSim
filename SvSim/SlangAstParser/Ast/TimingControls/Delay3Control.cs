using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record Delay3Control : TimingControl
{
    public Expression? Expr1;
    public Expression? Expr2;
    public Expression? Expr3;
};