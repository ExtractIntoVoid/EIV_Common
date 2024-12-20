using System.Numerics;

namespace EIV_Common.Common;

public class MinMax<T>(T min, T max)
    where T : IMinMaxValue<T>
{
    public MinMax() : this(T.MinValue, T.MaxValue)
    {
    }

    public T Min { get; set; } = min;
    public T Max { get; set; } = max;
}