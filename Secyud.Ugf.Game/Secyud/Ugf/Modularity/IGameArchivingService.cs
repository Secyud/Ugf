namespace Secyud.Ugf.Modularity
{
    public interface IGameArchivingService
    {
        int CurrentSlotIndex { get;  }
        void EnterGameWithSlot(int slotIndex);
        void SaveCurrentSlotMessage();
        void LoadSlotMessage(GameSlotComponent slot);
    }
}