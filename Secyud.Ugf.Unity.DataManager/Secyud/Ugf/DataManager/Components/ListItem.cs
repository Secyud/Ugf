using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public class ListItem : MonoBehaviour, IFieldComponent
    {
        [SerializeField] private TextMeshProUGUI _indexText;
        private int _index;
        public virtual Transform Last => transform;

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                _indexText.text = value.ToString();
            }
        }

        public ListField Parent { get; private set; }

        public virtual void Bind(ListField parent, int index)
        {
            Index = index;
            Parent = parent;
        }

        public void RemoveSelf()
        {
            Parent.RemoveAt(Index);
        }

        public virtual void SetVisibility(bool visibility, bool root = false)
        {
            if (gameObject.activeSelf != visibility)
                gameObject.SetActive(visibility);
        }

        public virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}