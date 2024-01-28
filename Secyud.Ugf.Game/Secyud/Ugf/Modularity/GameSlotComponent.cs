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
        }

        private void Start()
        {
            InitSlot();
        }

        public virtual void EnsureSlot()
        {
            Service.SelectSlot(SlotIndex);
        }

        public virtual void DeleteSlot()
        {
            Service.TryDeleteSlot(SlotIndex);
            InitSlot();
        }

        public virtual void InitSlot()
        {
            Service.LoadSlotMessage(this);
        }
    }
}