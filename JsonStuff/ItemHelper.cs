using EIV_JsonLib.Classes;
using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff
{
    public static class ItemHelper
    {
        public static bool HasProperty(this IItem item, string value)
        {
            return item.GetType().GetProperty(value) != null;
        }

        public static T? GetProperty<T>(this IItem item, string value)
        {
            if (item.HasProperty(value))
            {
                if (item.GetType().GetProperty(value)!.PropertyType == typeof(T))
                    return (T?)item.GetType().GetProperty(value)!.GetValue(item);
            }
            return default;
        }

        public static bool ChangeProperty(this IItem item, string valueName, ItemRecreator.KVChange kv)
        {
            if (!item.HasProperty(valueName))
                return false;
            var prop = item.GetType().GetProperty(valueName)!;
            Console.WriteLine(prop.PropertyType);
            if (!prop.CanWrite)
                return false;
            switch (kv.AvailableTypeName)
            {
                case TypeName.String:
                    if (prop.PropertyType != typeof(string))
                        return false;
                    prop.SetValue(item, kv.StringValue);
                    break;
                case TypeName.Int32:
                    if (prop.PropertyType != typeof(int))
                        return false;
                    prop.SetValue(item, kv.IntValue);
                    break;
                case TypeName.UInt32:
                    if (prop.PropertyType != typeof(uint))
                        return false;
                    prop.SetValue(item, kv.UIntValue);
                    break;
                case TypeName.Decimal:
                    if (prop.PropertyType != typeof(decimal))
                        return false;
                    prop.SetValue(item, kv.DecimalValue);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
