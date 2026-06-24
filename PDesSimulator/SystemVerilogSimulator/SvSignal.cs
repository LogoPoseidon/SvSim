using System.Numerics;
using System.Text;
using PDesSimulator.Simulator;

namespace PDesSimulator.SystemVerilogSimulator;

public class SvSignal : ISignal
{
    private BigInteger _value;
    private BigInteger _newValue;

    public string Name { get; }
    public int Width { get; }
    public bool IsSigned { get; }
    
    public BigInteger OldValue { get; private set; }
    public bool PosEdge { get; private set; }
    public bool NegEdge { get; private set; }
    public bool Changed { get; private set; }

    public SvSignal(string name, int width, bool isSigned, BigInteger initialValue)
    {
        Name = name;
        Width = width;
        IsSigned = isSigned;
        _value = Mask(initialValue);
        _newValue = _value;
        OldValue = _value;
    }

    public BigInteger Read() => _value;

    public void Write(BigInteger val)
    {
        var masked = Mask(val);
        if (masked == _newValue) return;
        _newValue = masked;
        Kernel.Instance.UpdateRequest(this);
    }

    public void WriteImmediate(BigInteger val)
    {
        Write(val);
    }

    public void Update()
    {
        Changed = _value != _newValue;
        PosEdge = _value == 0 && _newValue != 0;
        NegEdge = _value != 0 && _newValue == 0;
        
        OldValue = _value;
        _value = _newValue;
    }

    private BigInteger Mask(BigInteger val)
    {
        var mask = (BigInteger.One << Width) - 1;
        var res = val & mask;

        if (!IsSigned) return res;
        var signBit = BigInteger.One << (Width - 1);
        if ((res & signBit) != 0)
        {
            res -= BigInteger.One << Width;
        }
        return res;
    }

    public override string ToString()
    {
        if (Width == 1) return _value == 0 ? "0" : "1";

        var sb = new StringBuilder();
        var temp = _value;
        for (var i = Width - 1; i >= 0; i--)
        {
            sb.Append(((temp >> i) & 1) == 1 ? "1" : "0");
        }
        return $"b{sb} ";
    }
}