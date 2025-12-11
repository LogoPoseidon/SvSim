using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ProceduralAssignStatement : SvStatement
{
    public SvExpression? Assignment;
    public bool? IsForced;
};