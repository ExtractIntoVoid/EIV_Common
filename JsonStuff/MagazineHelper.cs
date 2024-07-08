using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff
{
    public static class MagazineHelper
    {
        /// <summary>
        /// Checking if the AmmoType is compatible in with the Magazine
        /// </summary>
        /// <param name="magazine">The Magazine</param>
        /// <param name="AmmoId">BaseId of the Ammo</param>
        /// <returns>False if AmmoId is not an IAmmo, and if SupportedAmmo doesnt contains either the BaseId or the Tags</returns>
        public static bool CheckAmmoCompatible(this IMagazine magazine, string AmmoId)
        {
            if (magazine == null)
                return false;
            var ammo = ItemMaker.CreateItem<IAmmo>(AmmoId);
            if (ammo == null)
                return false;
            return magazine.SupportedAmmo.Contains(AmmoId) || magazine.SupportedAmmo.Intersect(ammo.Tags).Any();
        }

        /// <summary>
        /// Inserting Ammos into the Magazine
        /// </summary>
        /// <param name="magazine">The Magazine</param>
        /// <param name="AmmoId">BaseId of the Ammo</param>
        /// <param name="AmmoCount">The amount to insert with this Type</param>
        /// <returns>False if could inserted, or full, or when its filled and wants to add more | True when successfully added all to magazine</returns>
        public static bool TryInsertAmmos(this IMagazine magazine, string AmmoId, uint AmmoCount)
        {
            if (magazine == null)
                return false;

            //  Magazine same as inside our mag, dont bother anything
            if (magazine.Ammunition.Count == magazine.MaxMagSize)
                return false;

            if (!magazine.CheckAmmoCompatible(AmmoId))
                return false;

            // This here prevent to accidentally make or instert ammo that not exists
            var ammo = ItemMaker.CreateItem<IAmmo>(AmmoId);
            if (ammo == null)
                return false;

            for (int i = 0; i < AmmoCount; i++)
            {
                magazine.Ammunition.Add(AmmoId);
                if (magazine.Ammunition.Count == magazine.MaxMagSize)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Inserting 1 Ammo into the Magazine
        /// </summary>
        /// <param name="magazine">The Magazine</param>
        /// <param name="AmmoId">BaseId of the Ammo</param>
        /// <returns>False if could inserted, or full, or when its filled | True when successfully added to magazine</returns>
        public static bool TryInsertAmmo(this IMagazine magazine, string AmmoId)
        {
            if (magazine == null)
                return false;

            //  Magazine same as inside our mag, dont bother anything
            if (magazine.Ammunition.Count == magazine.MaxMagSize)
                return false;

            if (!magazine.CheckAmmoCompatible(AmmoId))
                return false;

            // This here prevent to accidentally make or instert ammo that not exists
            var ammo = ItemMaker.CreateItem<IAmmo>(AmmoId);
            if (ammo == null)
                return false;

            magazine.Ammunition.Add(AmmoId);
            return true;
        }
    }
}
