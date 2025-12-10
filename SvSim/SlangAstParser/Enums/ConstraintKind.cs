namespace SvSim.SlangAstParser.Enums;

public enum ConstraintKind
{
    Invalid,
    List,
    Expression,
    Implication,
    Conditional,
    Uniqueness,
    DisableSoft,
    SolveBefore,
    Foreach
}