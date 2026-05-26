using SvSim.SlangAstParser.AstTree.Symbol.Type;

namespace SvSim.SlangAstParser.Serializer;

public static class SvTypeExtensions
{
    extension(ISvType type)
    {
        public ISvType GetCanonicalType()
        {
            var current = type;
            while (current is SvTypeAlias { ResolvedTarget: not null } alias)
            {
                current = alias.ResolvedTarget;
            }
            return current;
        }
    }
}