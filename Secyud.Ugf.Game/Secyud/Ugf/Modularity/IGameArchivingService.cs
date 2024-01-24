using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    /// <summary>
    /// To use archiving system, this service need
    /// to be implemented.
    /// </summary>
    public interface IGameArchivingService : IRegistry
    {
        int CurrentSlotIndex { get; }
        void EnterGameWithSlot(int slotIndex);
        void TryDeleteSlot(int slotIndex);
        void SaveCurrentSlotMessage();
        void LoadSlotMessage(GameSlotComponent slot);
    }
}