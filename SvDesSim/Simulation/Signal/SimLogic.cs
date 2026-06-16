using System.Numerics;
using System.Runtime.CompilerServices;

namespace SvDesSim.Simulation.Signal;

public readonly struct SimLogic<T>(T value, T unknown) :
    IBitwiseOperators<SimLogic<T>, SimLogic<T>, SimLogic<T>>,
    IAdditionOperators<SimLogic<T>, SimLogic<T>, SimLogic<T>>,
    IShiftOperators<SimLogic<T>, int, SimLogic<T>>,
    IEquatable<SimLogic<T>>
    where T : IBinaryInteger<T>
{
    public readonly T Value = value;
    public readonly T Unknown = unknown;

    // Encoding rule:
    // 0: Value=0, Unknown=0
    // 1: Value=1, Unknown=0
    // Z: Value=0, Unknown=1
    // X: Value=1, Unknown=1

    private SimLogic(T value) : this(value, T.Zero) { }

    private bool HasUnknown => Unknown != T.Zero;

    // ==========================================
    // BITWISE OPERATORS
    // ==========================================

    public static SimLogic<T> operator &(SimLogic<T> a, SimLogic<T> b)
    {
        var a0 = ~a.Value & ~a.Unknown;
        var b0 = ~b.Value & ~b.Unknown;
        var a1 = a.Value & ~a.Unknown;
        var b1 = b.Value & ~b.Unknown;

        var res0 = a0 | b0;
        var res1 = a1 & b1;
        var resX = ~(res0 | res1);

        return new SimLogic<T>(res1 | resX, resX);
    }

    public static SimLogic<T> operator |(SimLogic<T> a, SimLogic<T> b)
    {
        var a1 = a.Value & ~a.Unknown;
        var b1 = b.Value & ~b.Unknown;
        var a0 = ~a.Value & ~a.Unknown;
        var b0 = ~b.Value & ~b.Unknown;

        var res1 = a1 | b1;
        var res0 = a0 & b0;
        var resX = ~(res1 | res0);

        return new SimLogic<T>(res1 | resX, resX);
    }

    public static SimLogic<T> operator ^(SimLogic<T> a, SimLogic<T> b)
    {
        var resX = a.Unknown | b.Unknown;
        var resV = (a.Value ^ b.Value) | resX;
        return new SimLogic<T>(resV, resX);
    }

    public static SimLogic<T> operator ~(SimLogic<T> a)
    {
        var res1 = ~a.Value & ~a.Unknown;
        var resX = a.Unknown;
        return new SimLogic<T>(res1 | resX, resX);
    }

    // ==========================================
    // ARITHMETIC OPERATORS
    // ==========================================

    public static SimLogic<T> operator +(SimLogic<T> a, SimLogic<T> b)
    {
        if (a.HasUnknown || b.HasUnknown)
        {
            return new SimLogic<T>(T.AllBitsSet, T.AllBitsSet);
        }

        return new SimLogic<T>(a.Value + b.Value, T.Zero);
    }
    
    public static SimLogic<T> operator -(SimLogic<T> a, SimLogic<T> b)
    {
        if (a.HasUnknown || b.HasUnknown) return new SimLogic<T>(T.AllBitsSet, T.AllBitsSet);
        return new SimLogic<T>(a.Value - b.Value, T.Zero);
    }

    public static SimLogic<T> operator *(SimLogic<T> a, SimLogic<T> b)
    {
        if (a.HasUnknown || b.HasUnknown) return new SimLogic<T>(T.AllBitsSet, T.AllBitsSet);
        return new SimLogic<T>(a.Value * b.Value, T.Zero);
    }

    public static implicit operator SimLogic<T>(T val) => new(val);

    // ==========================================
    // SHIFT OPERATORS
    // ==========================================

    public static SimLogic<T> operator <<(SimLogic<T> value, int shiftAmount)
    {
        var newVal = value.Value << shiftAmount;
        var newUnk = value.Unknown << shiftAmount;
        return new SimLogic<T>(newVal, newUnk);
    }

    public static SimLogic<T> operator >>(SimLogic<T> value, int shiftAmount)
    {
        var newVal = value.Value >>> shiftAmount;
        var newUnk = value.Unknown >>> shiftAmount;
        return new SimLogic<T>(newVal, newUnk);
    }

    public static SimLogic<T> operator >>>(SimLogic<T> value, int shiftAmount)
    {
        return value >> shiftAmount;
    }

    // ==========================================
    // SYSTEMVERILOG SEMANTIC HELPERS
    // ==========================================


    /// <summary>
    /// Executes a SystemVerilog Arithmetic Right Shift (SV's `>>>` operator on signed types).
    /// Duplicates the semantic Sign Bit into the newly shifted spaces.
    /// </summary>
    public SimLogic<T> ArithmeticRightShift(int shiftAmount, int svWidth)
    {
        var newVal = Value >>> shiftAmount;
        var newUnk = Unknown >>> shiftAmount;

        var signBitMask = T.One << (svWidth - 1);
        var valSign = (Value & signBitMask) != T.Zero;
        var unkSign = (Unknown & signBitMask) != T.Zero;

        if (!valSign && !unkSign) return new SimLogic<T>(newVal, newUnk);

        var bitsToFill = Math.Min(shiftAmount, svWidth);
        
        var fillMask = SvMath.GetMask<T>(bitsToFill) << (svWidth - bitsToFill);

        if (unkSign) 
        {
            newVal |= fillMask;
            newUnk |= fillMask;
        }
        else if (valSign) 
        {
            newVal |= fillMask;
        }

        return new SimLogic<T>(newVal, newUnk);
    }

    /// <summary>
    /// Sign-extends a vector up to the container's max width based on the SV semantic width.
    /// Uses pure generic bitwise math. No casting required!
    /// </summary>
    public static SimLogic<T> SignExtend(SimLogic<T> input, int svWidth)
    {
        var signBitMask = T.One << (svWidth - 1);

        var valSign = (input.Value & signBitMask) != T.Zero;
        var unkSign = (input.Unknown & signBitMask) != T.Zero;

        var extensionMask = T.AllBitsSet << svWidth;

        var newVal = input.Value;
        var newUnk = input.Unknown;

        if (unkSign) {
            newVal |= extensionMask;
            newUnk |= extensionMask;
        }
        else if (valSign) {
            newVal |= extensionMask;
        }

        return new SimLogic<T>(newVal, newUnk);
    }

    // ==========================================
    // EQUALITY IMPLEMENTATION
    // ==========================================
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(SimLogic<T> other) => Value == other.Value && Unknown == other.Unknown;

    public override bool Equals(object? obj) => obj is SimLogic<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, Unknown);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(SimLogic<T> left, SimLogic<T> right) => left.Equals(right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(SimLogic<T> left, SimLogic<T> right) => !left.Equals(right);
}