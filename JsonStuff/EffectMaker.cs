using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public class EffectMaker
{
    public static Effect? MakeNewEffect(string name)
    {
        if (!Storage.Effects.TryGetValue(name, out Effect? item))
            return null;
        return (Effect)item.Clone();
    }
}
