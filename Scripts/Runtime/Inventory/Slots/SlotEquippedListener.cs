using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inventory.Items;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    public interface SlotEquippedListener
    {
        void OnItemEquipped(TPSCharacterCoordinator owner, Slot slot, Item item);
        void OnItemUnequipped(TPSCharacterCoordinator owner, Slot slot, Item item);
    }
}