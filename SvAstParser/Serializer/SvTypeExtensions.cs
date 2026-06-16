using SvAstParser.AstTree.Symbol.Type;

namespace SvAstParser.Serializer;

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