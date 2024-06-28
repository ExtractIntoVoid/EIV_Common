using EIV_JsonLib.Classes;
using EIV_JsonLib.Interfaces;

namespace EIV_Common.ItemStuff
{
    public static class ItemHelper
    {
        public static bool HasProperty(this IItem item, string value)
        {
            return item.GetType().GetProperty(value) != null;
        }

        public static bool ChangeProperty(this IItem item, string valueName, ItemRecreator.KVChange kv)
        {
            if (!item.HasProperty(valueName))
                return false;
            var prop = item.GetType().GetProperty(valueName)!;
            switch (kv.AvailableTypeName)
            {
                case TypeName.String:
                    prop.SetValue(item, kv.StringValue);
                    break;
                case TypeName.Int32:
                    prop.SetValue(item, kv.IntValue);
                    break;
                case TypeName.UInt32:
                    prop.SetValue(item, kv.UIntValue);
                    break;
                case TypeName.Decimal:
                    prop.SetValue(item, kv.DecimalValue);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
