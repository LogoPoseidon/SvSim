using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ProceduralDeassignStatement() : SvStatement
{
    public SvExpression? LValue;
    public bool? IsRelease;
};