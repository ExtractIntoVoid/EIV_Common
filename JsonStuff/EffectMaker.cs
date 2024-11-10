using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff;

public class EffectMaker
{
    public static IEffect? MakeNewEffect(string baseId)
    {
        if (!Storage.Effects.TryGetValue(baseId, out var item))
            return null;
        return (IEffect)item.Clone();
    }
}
