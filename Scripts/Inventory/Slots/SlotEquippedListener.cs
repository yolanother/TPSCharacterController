using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inventory.Items;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    public interface SlotEquippedListener
    {
        void OnItemEquipped(AvatarAnimationController avatar, Slot slot, Item item);
        void OnItemUnequipped(AvatarAnimationController avatar, Slot slot, Item item);
    }
}