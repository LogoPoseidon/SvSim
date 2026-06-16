namespace SvDesSim.Simulation.Signal;

public class UnpackedArray<TVar> where TVar : class
{
    private readonly TVar[] _elements;

    public TVar this[int index] => _elements[index];

    public UnpackedArray(int size, Func<int, TVar> elementFactory)
    {
        _elements = new TVar[size];
        for (var i = 0; i < size; i++)
        {
            _elements[i] = elementFactory(i);
        }
    }
}