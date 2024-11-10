using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff;

public static class MagazineHelper
{
    /// <summary>
    /// Checking if the AmmoType is compatible in with the Magazine
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammoId">BaseId of the Ammo</param>
    /// <returns>False if AmmoId is not an IAmmo, and if SupportedAmmo doesnt contains either the BaseId or the Tags</returns>
    public static bool CheckAmmoCompatible(this IMagazine magazine, string ammoId)
    {
        var ammo = ItemMaker.CreateItem<IAmmo>(ammoId);
        if (ammo == null)
            return false;
        return magazine.SupportedAmmo.Contains(ammoId) || magazine.SupportedAmmo.Intersect(ammo.Tags).Any();
    }

    /// <summary>
    /// Inserting Ammos into the Magazine
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammoId">BaseId of the Ammo</param>
    /// <param name="ammoCount">The amount to insert with this Type</param>
    /// <returns>False if it couldn't insert, or full, or when its filled and wants to add more | True when successfully added all to magazine</returns>
    public static bool TryInsertAmmos(this IMagazine magazine, string ammoId, uint ammoCount)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunition.Count == magazine.MaxMagSize)
            return false;

        if (!magazine.CheckAmmoCompatible(ammoId))
            return false;

        // This here prevent to accidentally make or insert ammo that not exists
        var ammo = ItemMaker.CreateItem<IAmmo>(ammoId);
        if (ammo == null)
            return false;

        for (int i = 0; i < ammoCount; i++)
        {
            magazine.Ammunition.Add(ammoId);
            if (magazine.Ammunition.Count == magazine.MaxMagSize)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Inserting 1 Ammo into the Magazine
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammoId">BaseId of the Ammo</param>
    /// <returns>False if it couldn't insert, or full, or when its filled | True when successfully added to magazine</returns>
    public static bool TryInsertAmmo(this IMagazine magazine, string ammoId)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunition.Count == magazine.MaxMagSize)
            return false;

        if (!magazine.CheckAmmoCompatible(ammoId))
            return false;

        // This here prevent to accidentally make or instert ammo that not exists
        var ammo = ItemMaker.CreateItem<IAmmo>(ammoId);
        if (ammo == null)
            return false;

        magazine.Ammunition.Add(ammoId);
        return true;
    }
}
