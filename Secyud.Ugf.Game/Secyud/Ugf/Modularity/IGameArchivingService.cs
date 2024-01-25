using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    /// <summary>
    /// To use archiving system, this service need
    /// to be implemented.
    /// </summary>
    public interface IGameArchivingService : IRegistry
    {
        /// <summary>
        /// For game loading and saving.
        /// </summary>
        int CurrentSlotIndex { get; }
        /// <summary>
        /// The slot is selected and ensured.
        /// Enter game and set the slot index.
        /// </summary>
        /// <param name="slotIndex"></param>
        void EnterGameWithSlot(int slotIndex);
        /// <summary>
        /// Delete the slot file.
        /// </summary>
        /// <param name="slotIndex"></param>
        void TryDeleteSlot(int slotIndex);
        /// <summary>
        /// Save game slot data to current slot.
        /// </summary>
        void SaveCurrentSlotMessage();
        /// <summary>
        /// Fill the preview of slot component.
        /// </summary>
        /// <param name="slot"></param>
        void LoadSlotMessage(GameSlotComponent slot);
    }
}