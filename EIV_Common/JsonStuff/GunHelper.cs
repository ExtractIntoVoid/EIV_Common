using EIV_JsonLib;
using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public static class GunHelper
{
    /// <summary>
    /// Checking if the Magazine is compatible in with the Gun
    /// </summary>
    /// <param name="gun">The Gun</param>
    /// <param name="magazineId">A Magazine</param>
    /// <returns>False if <paramref name="magazineId"/> is not <see cref="Magazine"/>, and if <see cref="Gun.MagazineSupport"/> doesnt contains either the <paramref name="magazineId"/> or the <see cref="CoreItem.Tags"/></returns>
    public static bool CheckMagazineCompatible(this Gun gun, string magazineId)
    {
        Magazine? magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        return gun.MagazineSupport.Contains(magazineId) || gun.MagazineSupport.Intersect(magazine.Tags).Any();
    }

    /// <summary>
    /// Creating a <see cref="Magazine"/> from <paramref name="magazineId"/> and put into <see cref="Gun.Magazine"/>
    /// </summary>
    /// <param name="gun">The Gun</param>
    /// <param name="magazineId">A Magazine</param>
    /// <returns>True if Success adding into Magazine, otherwise false</returns>
    public static bool TryCreateMagazine(this Gun gun, string magazineId)
    {
        if (!gun.CheckMagazineCompatible(magazineId))
            return false;

        Magazine? magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        gun.Magazine = magazine;
        return true;
    }


    public static bool TryInsertMagazine(this Gun gun, string magazineId, string ammoId, uint ammoToInsert)
    {
        if (!gun.CheckMagazineCompatible(magazineId))
            return false;

        Magazine? magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        if (!magazine.TryInsertAmmos(ammoId, ammoToInsert))
            return false;
        gun.Magazine = magazine;
        return true;
    }

    public static bool TryInsertMagazine(this Gun gun, string magazineId, string ammoId)
    {
        if (!gun.CheckMagazineCompatible(magazineId))
            return false;

        Magazine? magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        if (!magazine.TryInsertAmmo(ammoId))
            return false;
        gun.Magazine = magazine;
        return true;
    }
}
