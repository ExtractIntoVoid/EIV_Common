using System.Diagnostics.CodeAnalysis;

namespace EIV_Common.Coroutines;

public struct CoroutineHandle : IEquatable<CoroutineHandle>, IEqualityComparer<CoroutineHandle>
{
    public int CoroutineHash;

    public CoroutineHandle(int hash)
    {
        CoroutineHash = hash;
    }

    public bool Equals(CoroutineHandle other)
    {
        return CoroutineHash == other.CoroutineHash;
    }

    public bool Equals(CoroutineHandle x, CoroutineHandle y)
    {
        return x.CoroutineHash == y.CoroutineHash;
    }

    public override int GetHashCode()
    {
        return CoroutineHash;
    }

    public int GetHashCode([DisallowNull] CoroutineHandle obj)
    {
        return obj.CoroutineHash;
    }

    public static implicit operator CoroutineHandle(int hash)
    {
        return new CoroutineHandle(hash);
    }

    public static implicit operator int(CoroutineHandle coroutineHandle)
    {
        return coroutineHandle.CoroutineHash;
    }

    public static implicit operator CoroutineHandle(Coroutine coroutine)
    {
        return new CoroutineHandle(coroutine.GetHashCode());
    }
}
