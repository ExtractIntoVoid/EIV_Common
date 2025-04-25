using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public static class MagazineHelper
{
    /// <summary>
    /// Checking the <paramref name="ammoId"/> is compatible in with the <see cref="Magazine"/>
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammoId">Id of the Ammo</param>
    /// <returns>False if <paramref name="ammoId"/> is not <see cref="Ammo"/>, and if SupportedAmmo doesnt contains either the BaseId or the Tags</returns>
    public static bool CheckAmmoCompatible(this Magazine magazine, string ammoId)
    {
        Ammo? ammo = ItemMaker.CreateItem<Ammo>(ammoId);
        if (ammo == null)
            return false;
        return magazine.SupportedAmmos.Contains(ammoId) || magazine.SupportedAmmos.Intersect(ammo.Tags).Any();
    }

    /// <summary>
    /// Inserting <see cref="Ammo"/> into the <see cref="Magazine"/>
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammoId">Id of the Ammo</param>
    /// <param name="ammoCount">The amount to insert with this Ammo</param>
    /// <returns>False if it couldn't insert, or full, or when its filled and wants to add more | True when successfully added all to magazine</returns>
    public static bool TryInsertAmmos(this Magazine magazine, string ammoId, uint ammoCount)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunitions.Count == magazine.MaxMagSize)
            return false;

        if (!magazine.CheckAmmoCompatible(ammoId))
            return false;

        Ammo? ammo = ItemMaker.CreateItem<Ammo>(ammoId);
        if (ammo == null)
            return false;

        for (int i = 0; i < ammoCount; i++)
        {
            magazine.Ammunitions.Add((Ammo)ammo.Clone());
            if (magazine.Ammunitions.Count == magazine.MaxMagSize)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Inserting <see cref="Ammo"/> into the <see cref="Magazine"/>
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammo">The Ammo type to insert</param>
    /// <param name="ammoCount">The amount to insert with this Ammo</param>
    /// <returns>False if it couldn't insert, or full, or when its filled and wants to add more | True when successfully added all to magazine</returns>
    public static bool TryInsertAmmos(this Magazine magazine, Ammo ammo, uint ammoCount)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunitions.Count == magazine.MaxMagSize)
            return false;

        if (!magazine.CheckAmmoCompatible(ammo.Id))
            return false;

        for (int i = 0; i < ammoCount; i++)
        {
            magazine.Ammunitions.Add((Ammo)ammo.Clone());
            if (magazine.Ammunitions.Count == magazine.MaxMagSize)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Inserting <see cref="Ammo"/> into the <see cref="Magazine"/>
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammos">The Ammos to insert</param>
    /// <returns>False if it couldn't insert, or full, or when its filled and wants to add more | True when successfully added all to magazine</returns>
    public static bool TryInsertAmmos(this Magazine magazine, List<Ammo> ammos)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunitions.Count == magazine.MaxMagSize)
            return false;

        foreach (var ammo in ammos)
        {
            if (!magazine.CheckAmmoCompatible(ammo.Id))
                continue;
            magazine.Ammunitions.Add(ammo);
            if (magazine.Ammunitions.Count == magazine.MaxMagSize)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Inserting an <see cref="Ammo"/> into the <see cref="Magazine"/>
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammoId">Id of the Ammo</param>
    /// <returns>False if it couldn't insert, or full, or when its filled | True when successfully added to magazine</returns>
    public static bool TryInsertAmmo(this Magazine magazine, string ammoId)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunitions.Count == magazine.MaxMagSize)
            return false;

        if (!magazine.CheckAmmoCompatible(ammoId))
            return false;

        Ammo? ammo = ItemMaker.CreateItem<Ammo>(ammoId);
        if (ammo == null)
            return false;

        magazine.Ammunitions.Add(ammo);
        return true;
    }

    /// <summary>
    /// Inserting an <see cref="Ammo"/> into the <see cref="Magazine"/>
    /// </summary>
    /// <param name="magazine">The Magazine</param>
    /// <param name="ammo">The Ammo to insert</param>
    /// <returns>False if it couldn't insert, or full, or when its filled | True when successfully added to magazine</returns>
    public static bool TryInsertAmmo(this Magazine magazine, Ammo ammo)
    {
        //  Magazine same as inside our mag, don't bother anything
        if (magazine.Ammunitions.Count == magazine.MaxMagSize)
            return false;

        if (!magazine.CheckAmmoCompatible(ammo.Id))
            return false;

        magazine.Ammunitions.Add(ammo);
        return true;
    }
}
