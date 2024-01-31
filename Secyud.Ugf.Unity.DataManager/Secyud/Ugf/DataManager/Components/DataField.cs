using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public abstract class DataField : MonoBehaviour, IFieldComponent
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _description;


        public virtual Transform Last => transform;
        public object Parent { get; private set; }
        public SAttribute SAttribute { get; private set; }

        public virtual void Bind(object parent, SAttribute sAttribute)
        {
            Parent = parent;
            SAttribute = sAttribute;
            string str = sAttribute.Info.Name;
            if (str.StartsWith('<'))
            {
                str = str[1..str.IndexOf('>')];
            }
            _label.text = U.T[str];
            DescriptionAttribute description = sAttribute
                .Info.GetCustomAttribute<DescriptionAttribute>();
            _description.text = U.T[description?.Description];
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