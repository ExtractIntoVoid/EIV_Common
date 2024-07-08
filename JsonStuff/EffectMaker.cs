using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff
{
    public class EffectMaker
    {
        public static IEffect? MakeNewEffect(string BaseId)
        {
            if (!Storage.Effects.TryGetValue(BaseId, out IEffect? item))
                return null;
            if (item == null)
                return null;
            return (IEffect)item.Clone();
        }
    }
}
