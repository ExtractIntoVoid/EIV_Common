﻿using EIV_JsonLib;
using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public static class ItemHelper
{
    public static bool HasProperty(this ItemBase item, string value)
    {
        return item.GetType().GetProperty(value) != null;
    }

    public static T? GetProperty<T>(this ItemBase item, string value)
    {
        if (item.HasProperty(value))
        {
            if (item.GetType().GetProperty(value)!.PropertyType == typeof(T))
                return (T?)item.GetType().GetProperty(value)!.GetValue(item);
        }
        return default;
    }

    public static bool ChangeProperty(this ItemBase item, string valueName, KVChange kv)
    {
        if (!item.HasProperty(valueName))
            return false;
        System.Reflection.PropertyInfo prop = item.GetType().GetProperty(valueName)!;
        if (!prop.CanWrite)
            return false;
        switch (kv.AvailableTypeName)
        {
            case TypeName.String:
                if (prop.PropertyType != typeof(string))
                    return false;
                prop.SetValue(item, kv.StringValue);
                break;
            case TypeName.Int:
                if (prop.PropertyType != typeof(int))
                    return false;
                prop.SetValue(item, kv.IntValue);
                break;
            case TypeName.UInt:
                if (prop.PropertyType != typeof(uint))
                    return false;
                prop.SetValue(item, kv.UIntValue);
                break;
            case TypeName.Decimal:
                if (prop.PropertyType != typeof(decimal))
                    return false;
                prop.SetValue(item, kv.DecimalValue);
                break;
            case TypeName.List_String:
                if (prop.PropertyType != typeof(List<string>))
                    return false;
                prop.SetValue(item, kv.ListStringValue);
                break;
            default:
                break;
        }
        return true;
    }
}
