using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public static class GunHelper
{
    public static bool CheckMagazineCompatible(this Gun gun, string magazineId)
    {
        var magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        return gun.MagazineSupport.Contains(magazineId) || gun.MagazineSupport.Intersect(magazine.Tags).Any();
    }

    public static bool TryCreateMagazine(this Gun gun, string magazineId)
    {
        if (!gun.CheckMagazineCompatible(magazineId))
            return false;


        var magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        gun.Magazine = magazine;
        return true;
    }

    public static bool TryInsertMagazine(this Gun gun, string magazineId, string ammoId, uint ammoToInsert)
    {
        if (!gun.CheckMagazineCompatible(magazineId))
            return false;


        var magazine = ItemMaker.CreateItem<Magazine>(magazineId);
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


        var magazine = ItemMaker.CreateItem<Magazine>(magazineId);
        if (magazine == null)
            return false;

        if (!magazine.TryInsertAmmo(ammoId))
            return false;
        gun.Magazine = magazine;
        return true;
    }
}
