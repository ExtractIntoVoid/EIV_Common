using System.Security.Cryptography;

namespace EIV_Common.Extensions;

public static class ListExt
{
    public static T? GetRandom<T>(this IList<T> list) => list.Count != 0 ? list[RandomNumberGenerator.GetInt32(0, list.Count - 1)] : default;

    public static T? GetRandom<T>(this T[] array) => array.Length != 0 ? array[RandomNumberGenerator.GetInt32(0, array.Length - 1)] : default;
}
