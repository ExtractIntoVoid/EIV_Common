using EIV_JsonLib.Interfaces;

namespace EIV_Common.Inventory;

public interface ISlot<T> where T : IItem
{
    public T SlotItem { get; set; }
}
