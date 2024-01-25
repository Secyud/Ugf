using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Modularity
{
    public class GameSlotComponent : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI SlotNameText { get; private set; }
        [field: SerializeField] public int SlotIndex { get; private set; }

        protected virtual void Awake()
        {
            if (SlotIndex < 0)
            {
                SlotIndex = transform.GetSiblingIndex();
            }

            InitSlotUi();
        }

        protected virtual void InitSlotUi()
        {
            if (SlotNameText) SlotNameText.text = $"{U.T["Slot"]} {SlotIndex}";
        }

        public virtual void EnsureSlot()
        {
            U.Get<IGameArchivingService>().EnterGameWithSlot(SlotIndex);
        }

        public virtual void DeleteSlot()
        {
            U.Get<IGameArchivingService>().TryDeleteSlot(SlotIndex);
            InitSlotUi();
        }
    }
}