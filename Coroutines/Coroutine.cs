using System.Diagnostics.CodeAnalysis;

namespace EIV_Common.Coroutines
{
    public struct Coroutine : IEquatable<Coroutine>, IEqualityComparer<Coroutine>
    {
        public IEnumerator<double> Enumerator;
        public bool IsRunning;
        public bool ShouldKill;
        public bool ShouldPause;
        public bool IsSuccess;
        public CoroutineType CoroutineType;
        IEnumerator<double> BaseEnumerator;
        public Coroutine(IEnumerator<double> enumerator, CoroutineType type)
        {
            Enumerator = enumerator;
            BaseEnumerator = enumerator;
            IsRunning = true;
            ShouldKill = false;
            CoroutineType = type;
            IsSuccess = false;
        }

        public override int GetHashCode()
        {
            return BaseEnumerator != null ? BaseEnumerator.GetHashCode() : 0;
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
            return $"{this.GetHashCode()} IsRunning: {IsRunning}, ShouldKill {ShouldKill}, ShouldPause: {ShouldPause}, IsSuccess: {IsSuccess}, CoroutineType: {CoroutineType}";
        }
    }
}
