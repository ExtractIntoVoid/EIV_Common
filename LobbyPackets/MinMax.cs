using System.Numerics;

namespace EIV_Common.MODAPI.LobbyPackets;

public class MinMax<T> where T : IMinMaxValue<T>
{
    public MinMax()
    {
        Min = T.MinValue;
        Max = T.MaxValue;
    }

    public MinMax(T min, T max)
    {
        Min = min; 
        Max = max;
    }

    public T Min { get; set; }
    public T Max { get; set; }

}