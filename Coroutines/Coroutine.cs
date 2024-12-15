using System.Diagnostics.CodeAnalysis;

namespace EIV_Common.Coroutines;

public struct Coroutine(IEnumerator<double> enumerator, CoroutineType type, string tag = "")
    : IEquatable<Coroutine>, IEqualityComparer<Coroutine>
{
    public IEnumerator<double> Enumerator = enumerator;
    public bool IsRunning = true;
    public bool ShouldKill = false;
    public bool ShouldPause;
    public bool IsSuccess = false;
    public readonly string Tag = tag;
    public readonly CoroutineType CoroutineType = type;
    private readonly IEnumerator<double> _baseEnumerator = enumerator;

    public override int GetHashCode()
    {
        return _baseEnumerator != null ? _baseEnumerator.GetHashCode() : 0;
    }

    public bool Equals(Coroutine other)
    {
        return this.GetHashCode() == other.GetHashCode();
    }

    public bool Equals(Coroutine x, Coroutine y)
    {
        return x.GetHashCode() == y.GetHashCode();
    }

    public int GetHashCode([DisallowNull] Coroutine obj)
    {
        return obj.GetHashCode();
    }

    public override string ToString()
    {
        return $"{this.GetHashCode()} IsRunning: {IsRunning}, ShouldKill {ShouldKill}, ShouldPause: {ShouldPause}, IsSuccess: {IsSuccess}, CoroutineType: {CoroutineType} Tag: {Tag}";
    }
}
