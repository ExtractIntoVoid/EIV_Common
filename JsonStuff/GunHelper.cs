using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff
{
    public static class GunHelper
    {
        public static bool CheckMagazineCompatible(this IGun gun, string MagazineId)
        {
            if (gun == null)
                return false;

            var magazine = ItemMaker.CreateItem<IMagazine>(MagazineId);
            if (magazine == null)
                return false;

            return gun.MagazineSupport.Contains(MagazineId) || gun.MagazineSupport.Intersect(magazine.Tags).Any();
        }

        public static bool TryCreateMagazine(this IGun gun, string MagazineId)
        {
            if (gun == null)
                return false;

            if (!gun.CheckMagazineCompatible(MagazineId))
                return false;


            var magazine = ItemMaker.CreateItem<IMagazine>(MagazineId);
            if (magazine == null)
                return false;

            gun.Magazine = magazine;
            return true;
        }

        public static bool TryInsertMagazine(this IGun gun, string MagazineId, string AmmoId, uint AmmoToInsert)
        {
            if (gun == null)
                return false;

            if (!gun.CheckMagazineCompatible(MagazineId))
                return false;


            var magazine = ItemMaker.CreateItem<IMagazine>(MagazineId);
            if (magazine == null)
                return false;

            if (!magazine.TryInsertAmmos(AmmoId, AmmoToInsert))
                return false;
            gun.Magazine = magazine;
            return true;
        }

        public static bool TryInsertMagazine(this IGun gun, string MagazineId, string AmmoId)
        {
            if (gun == null)
                return false;

            if (!gun.CheckMagazineCompatible(MagazineId))
                return false;


            var magazine = ItemMaker.CreateItem<IMagazine>(MagazineId);
            if (magazine == null)
                return false;

            if (!magazine.TryInsertAmmo(AmmoId))
                return false;
            gun.Magazine = magazine;
            return true;
        }
    }
}
