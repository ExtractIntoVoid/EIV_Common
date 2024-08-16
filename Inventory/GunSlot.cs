using EIV_JsonLib.Interfaces;

namespace EIV_Common.Inventory;

public class GunSlot : ISlot<IGun>
{
    public IGun SlotItem { get; set; }

    public List<IAttachment> Attachments { get; set; } = [];

    public GunSlot(IGun gun)
    { 
        SlotItem = gun;
    }
}
