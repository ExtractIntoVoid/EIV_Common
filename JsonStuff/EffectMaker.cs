using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public class EffectMaker
{
    public static Effect? MakeNewEffect(string baseId)
    {
        if (!Storage.Effects.TryGetValue(baseId, out var item))
            return null;
        return (Effect)item.Clone();
    }
}
