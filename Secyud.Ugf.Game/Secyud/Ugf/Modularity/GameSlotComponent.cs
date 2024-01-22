using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Modularity
{
    public class GameSlotComponent : MonoBehaviour
    {
        [SerializeField] private int _slotIndex;
        [SerializeField] private TextMeshProUGUI _slotNameText;

        public TextMeshProUGUI SlotNameText => _slotNameText;
        public int SlotIndex => _slotIndex;

        protected virtual void Awake()
        {
            if (_slotIndex < 0)
            {
                _slotIndex = transform.GetSiblingIndex();
            }

            InitSlotUi();
        }

        protected virtual void InitSlotUi()
        {
            if (_slotNameText) _slotNameText.text = $"{U.T["Slot"]} {_slotIndex}";
        }

        public virtual void EnsureSlot()
        {
            U.Get<IGameArchivingService>().EnterGameWithSlot(_slotIndex);
        }

        public virtual void DeleteSlot()
        {
        }
    }
}