using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record DisableStatement : SvStatement
{
    public SvExpression? Target;
};