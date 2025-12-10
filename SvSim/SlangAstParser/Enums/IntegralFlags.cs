namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum IntegralFlags
{
    Unsigned = 0,
    TwoState = Unsigned,
    Signed = 1,
    FourState = 2,
    Reg = 4 
}