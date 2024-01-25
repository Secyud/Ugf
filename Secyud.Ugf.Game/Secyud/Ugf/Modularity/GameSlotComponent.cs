using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Modularity
{
    public class GameSlotComponent : MonoBehaviour
    {
        [field: SerializeField] public int SlotIndex { get; private set; } = -1;

        protected IGameArchivingService Service;
        
        protected virtual void Awake()
        {
            if (SlotIndex < 0)
            {
                SlotIndex = transform.GetSiblingIndex();
            }

            Service = U.Get<IGameArchivingService>();
            Service.LoadSlotMessage(this);
        }

        public virtual void EnsureSlot()
        {
            Service.EnterGameWithSlot(SlotIndex);
        }

        public virtual void DeleteSlot()
        {
            Service.TryDeleteSlot(SlotIndex);
            Service.LoadSlotMessage(this);
        }
    }
}