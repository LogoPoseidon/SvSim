using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Ast.Statements;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record ForeachConstraint : SvConstraint
{
    public SvExpression? ArrayRef;
    public LoopDim[] LoopDims = [];
    public SvConstraint? Body;
};