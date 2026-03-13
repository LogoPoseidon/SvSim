
namespace SvSim.Simulation.Signal;

public class UnpackedArray<TVar> where TVar : class
{
    public readonly TVar[] Elements;

    public UnpackedArray(int size, Func<int, TVar> elementFactory)
    {
        Elements = new TVar[size];
        for (var i = 0; i < size; i++)
        {
            Elements[i] = elementFactory(i);
        }
    }
}